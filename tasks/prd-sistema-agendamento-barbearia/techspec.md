# Especificação Técnica — Sistema de Agendamento de Barbearia

## Resumo Executivo

O Sistema de Agendamento de Barbearia será composto por uma aplicação web React (Vite + TypeScript) e uma API ASP.NET Core 8 estruturada em camadas (Domain, Application, Infrastructure, API) com SQL Server como repositório de dados. O design privilegia modularização por contexto (Catálogo, Agenda, Operações Financeiras, Notificações) e uso de Logto como provedor externo de identidade via OIDC, emitindo tokens JWT consumidos pelo front-end. Eventos operacionais (lembretes, confirmações, bloqueios) serão tratados por uma camada de background jobs baseada em `IHostedService` e filas internas persistidas em banco.

A arquitetura prioriza simplicidade e escalabilidade vertical para o volume estimado (<50 usuários simultâneos). As decisões incluem reuse de Shadcn UI + Tailwind conforme padrões internos, React Query para integração HTTP e SignalR para atualizações em tempo real da agenda. O plano de implementação contempla criação de padrões de código e testes definidos nesta Tech Spec, garantindo aderência às regras de projeto e preparando evolução futura (multiunidade, mensageria externa) sem acoplamento excessivo.

## Arquitetura do Sistema

### Visão Geral dos Componentes

- **Client Web (React/TypeScript)**: SPA/PWA responsiva com Vite, Tailwind e Shadcn UI; organiza-se por módulos (`modules/scheduling`, `modules/admin`, `modules/finance`) e utiliza React Query para consumo da API e Zustand para estados locais não assíncronos. Implementa autenticação Logto via pacote `@logto/react` com hooks customizados (`useAuth`).
- **API Gateway (.NET 8 / ASP.NET Core)**: Projeto principal (`BarberShop.Api`) expõe endpoints RESTful seguindo `rules/http.md`, valida payloads com FluentValidation e orquestra casos de uso da camada Application. Serve SignalR Hub (`/hubs/scheduling`) para push de eventos de agenda.
- **Application Layer**: Contém use cases (`CreateAppointmentUseCase`, `CancelAppointmentUseCase`, etc.), portas (`IAppointmentRepository`, `INotificationDispatcher`), DTOs e manipuladores de comando/consulta. Todos os casos de uso recebem `IUnitOfWork` para consistência transacional.
- **Domain Layer**: Entidades agregadas (`Client`, `Barber`, `Service`, `Appointment`, `PaymentRecord`) e value objects (e.g. `TimeSlot`, `Money`). Implementa validações de negócio puras.
- **Infrastructure Layer**: Implementações concretas de repositórios (EF Core), Unit of Work, integrações Logto (via OIDC introspection), serviço de notificações (SendGrid REST + Twilio Conversations API), providers de templates de email/WhatsApp, persistência de jobs em `notification_jobs`.
- **Database (SQL Server)**: Esquema `barbershop` com tabelas normalizadas seguindo `rules/sql.md`. Migrations gerenciadas via `dotnet ef migrations`. Índices por `appointments.barber_id + start_at` e `payments.completed_at`.
- **Background Services**: `NotificationDispatcherHostedService` processa filas para envio de lembretes; `DailyMetricsAggregationJob` (opcional) gera cache para dashboard.
- **Observability Stack**: Serilog para logging estruturado (sink Application Insights), OpenTelemetry para traços, métricas de API via Prometheus exporter integrado ao ASP.NET.

Fluxo de dados: usuário autentica no Logto → front recebe token → requests para API via `Authorization: Bearer` → API valida token no middleware (chamada Logto / JWKS) → casos de uso interagem com EF Core + UoW → em eventos relevantes, registros adicionados à tabela de notificações → hosted service envia mensagens por integrações externas → SignalR notifica clientes conectados.

## Design de Implementação

### Interfaces Principais

```csharp
public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(Guid appointmentId, CancellationToken ct);
    Task<IReadOnlyList<Appointment>> SearchAsync(AppointmentFilter filter, CancellationToken ct);
    Task AddAsync(Appointment appointment, CancellationToken ct);
    Task UpdateAsync(Appointment appointment, CancellationToken ct);
}
```

```csharp
public interface INotificationDispatcher
{
    Task EnqueueAsync(NotificationMessage message, CancellationToken ct);
    Task MarkAsSentAsync(Guid notificationId, NotificationChannel channel, CancellationToken ct);
}
```

```csharp
public interface IAuthService
{
    Task<UserContext> ValidateTokenAsync(string token, CancellationToken ct);
    Task<bool> HasRoleAsync(UserContext context, params string[] roles);
}
```

```typescript
export interface SchedulingApiClient {
  listAvailableSlots(input: ListSlotsParams): Promise<SlotDto[]>;
  createAppointment(payload: CreateAppointmentPayload): Promise<AppointmentDto>;
  cancelAppointment(id: string): Promise<void>;
}
```

### Modelos de Dados

**Entidades (Domain/EF Core)**

```csharp
public class Appointment : AggregateRoot
{
    public Guid AppointmentId { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid BarberId { get; private set; }
    public DateTime StartAt { get; private set; }
    public DateTime EndAt { get; private set; }
    public decimal TotalAmount { get; private set; }
    public AppointmentStatus Status { get; private set; }
    private readonly List<AppointmentService> _services = new();
    public IReadOnlyCollection<AppointmentService> Services => _services;
    public string? Notes { get; private set; }
}
```

```sql
CREATE TABLE barbershop.appointments (
    appointment_id UNIQUEIDENTIFIER PRIMARY KEY,
    client_id UNIQUEIDENTIFIER NOT NULL,
    barber_id UNIQUEIDENTIFIER NOT NULL,
    start_at DATETIME2 NOT NULL,
    end_at DATETIME2 NOT NULL,
    total_amount NUMERIC(10,2) NOT NULL,
    status VARCHAR(20) NOT NULL,
    notes TEXT NULL,
    created_at DATETIME2 NOT NULL,
    updated_at DATETIME2 NOT NULL,
    CONSTRAINT fk_appointments_clients FOREIGN KEY (client_id) REFERENCES barbershop.clients(client_id),
    CONSTRAINT fk_appointments_barbers FOREIGN KEY (barber_id) REFERENCES barbershop.barbers(barber_id)
);
CREATE INDEX idx_appointments_barber_start ON barbershop.appointments (barber_id, start_at);
```

**DTOs/API Contracts**

```csharp
public record CreateAppointmentRequest
(
    Guid ClientId,
    Guid BarberId,
    ICollection<Guid> ServiceIds,
    DateTime StartAt,
    string? Notes
);
```

```csharp
public record AppointmentResponse
(
    Guid AppointmentId,
    Guid ClientId,
    Guid BarberId,
    DateTime StartAt,
    DateTime EndAt,
    decimal TotalAmount,
    string Status,
    IEnumerable<ServiceSummary> Services
);
```

**Front-end Types**

```typescript
export type AppointmentDto = {
  appointmentId: string;
  client: ClientSummary;
  barber: BarberSummary;
  startAt: string;
  endAt: string;
  totalAmount: number;
  status: 'scheduled' | 'completed' | 'cancelled' | 'no_show';
};
```

### Endpoints de API

- `POST /api/v1/auth/logto/callback`: troca código OIDC por tokens, cria sessão interna.
- `GET /api/v1/services`: lista serviços ativos com paginação (`limit`, `offset`), partial response via `fields`.
- `POST /api/v1/appointments`: cria agendamento, retorna `201` com body `AppointmentResponse`.
- `GET /api/v1/appointments`: filtra agendamentos (cliente, barbeiro, intervalo, status) com paginação; suporta query `status=scheduled&barberId=`.
- `POST /api/v1/appointments/{appointmentId}/cancel`: executa cancelamento com validação de antecedência.
- `POST /api/v1/appointments/{appointmentId}/reschedule`: recebe novo `startAt`.
- `GET /api/v1/barbers`: lista profissionais, inclui disponibilidade se `includeAvailability=true`.
- `POST /api/v1/barbers`: CRUD de barbeiros (ação restrita a `admin`).
- `POST /api/v1/services`: manutenção de catálogo.
- `GET /api/v1/dashboard/daily-metrics`: retorna métricas agregadas para data alvo.
- `POST /api/v1/payments`: registra pagamento, `201`.
- `GET /api/v1/payments/report`: filtra por período e exporta CSV (`Accept: text/csv`).
- `GET /api/v1/notifications/pending`: uso interno por hosted service para fetch em batch.

Todas as respostas de erro seguem `rules/http.md` (`422` para violações de negócio, `400` para payload inválido). Documentação OpenAPI gerada com Swashbuckle (`/swagger/v1/swagger.json`).

## Pontos de Integração

- **Logto (OIDC)**: integração via pacote `Logto.AspNetCore`. Configuração inclui `Authority`, `ClientId`, `ClientSecret`, `Scopes = ["openid","profile","email"]`. Middleware `AddAuthentication().AddJwtBearer()` com validação de issuer, audience e cache de JWKS. Em falhas de introspecção, retornar `401` e registrar log nível WARN.
- **SendGrid Email API**: usado para confirmações e relatórios. `INotificationDispatcher` gera payload JSON (`personalizations`, `template_id`). Retry com política Polly (3 tentativas, backoff exponencial). Erro definitivo → marcar `notification_jobs.status = 'failed'` e log ERROR.
- **Twilio Conversations / WhatsApp Business**: envio de mensagens WhatsApp. Configurar `AccountSid`, `AuthToken`, `From` (número WhatsApp). Payload `POST /2010-04-01/Accounts/{AccountSid}/Messages.json`. Validar respostas 2xx; em `429` aplicar retry com jitter. Como canal oficial não é obrigatório, fallback para SMS (opcional) via Twilio.
- **Application Insights**: `Serilog` sink com chave de instrumentação; logs estruturados `Context`, `CorrelationId`.

## Análise de Impacto

| Componente Afetado | Tipo de Impacto | Descrição & Nível de Risco | Ação Requerida |
| ------------------ | --------------- | -------------------------- | -------------- |
| Client Web (novo) | Novo componente | SPA React com autenticação OIDC. Risco baixo, depende de configuração correta do Logto. | Provisionar projeto Vite, configurar Tailwind/Shadcn, pipelines CI. |
| BarberShop.Api | Novo serviço | API REST .NET 8, Clean Architecture. Risco médio (múltiplas integrações). | Definir infraestrutura, provisionar pipelines, configurar secret storage. |
| SQL Server Schema | Novo esquema | Criação de tabelas e índices (appointments, payments). Risco baixo, segue padrões. | Criar migrations iniciais, validar naming. |
| Notification Dispatcher | Novo serviço background | Envio de email/WhatsApp, sensível a falhas externas. Risco médio. | Implementar política de retry + DLQ em tabela, monitorar métricas. |
| Observability Stack | Configuração | Integrar Serilog/Otel com App Insights. Risco baixo. | Habilitar dashboards pré-definidos. |

Não há sistemas legados coexistentes; a implantação inicial não exige migrações de dados.

## Abordagem de Testes

### Testes Unitários

- **Domain Layer**: xUnit + FluentAssertions. Cobrir regras de `Appointment` (validação de janelas, múltiplos serviços, cancelamento com antecedência). Simular relógio com `ITestClock` para repetibilidade.
- **Application Layer**: xUnit com Moq para repositórios. Testar fluxos principais e alternativos (ex.: tentativa de cancelamento tardio → `BusinessRuleException`).
- **Infrastructure Helpers**: validar adaptadores (mapping DTO ↔ Entity), políticas de retry (usando `PolicyRegistry`).
- **Front-end**: Jest + React Testing Library; testes unitários para componentes críticos (`ScheduleForm`, `AppointmentList`). Mock de React Query via MSW.

### Testes de Integração

- **API**: `test/integration` com `WebApplicationFactory` + banco em memória (ou `Testcontainers` com SQL Server). Validar endpoints chave (`POST /api/v1/appointments`, `GET /api/v1/dashboard/daily-metrics`).
- **Notifications**: usar WireMock para simular SendGrid/Twilio. Testes verificam reprocessamento em falha 500.
- **Auth Flow**: integração com Logto sandbox (ou mock server) para validar login redirects e refresh tokens.
- **Front-end E2E**: Playwright rodando contra ambiente de staging; cenários: agendamento completo, cancelamento, dashboard admin.

Cobertura mínima exigida: 85% domain/application, 80% componentes front críticos. Tests executados via `dotnet test` e `yarn test`. Pipeline CI impede merge sem sucesso.

## Sequenciamento de Desenvolvimento

### Ordem de Construção

1. **Infraestrutura Básica**: provisionar repositórios, configurar pipelines CI/CD, preparar ambiente Logto (cliente, redirect URIs) e secrets.
2. **Esquema de Dados & Migrations**: modelar entidades básicas (clients, barbers, services, appointments, payments, notification_jobs). Garantir seeds para ambientes dev.
3. **Autenticação & Middleware**: integrar Logto, configurar Authorization/Policies (`roles`: admin, manager, barber, client).
4. **Casos de Uso de Agenda**: implementar `CreateAppointment`, `ListAvailability`, `CancelAppointment`, `Reschedule`. Expor endpoints REST + SignalR hub.
5. **Front-end Módulo Cliente**: telas de catálogo, formulário de agendamento, histórico. Integração com API + hooks de auth.
6. **Painel Administrativo**: endpoints de gestão (barbers/services, dashboard). UI administrativa.
7. **Módulo Financeiro**: registro de pagamentos, cálculo de comissão, relatórios CSV.
8. **Notificações**: implementações SendGrid/Twilio, hosted service, templates.
9. **Observabilidade & Hardening**: métricas, logs, security headers, rate limiting.
10. **Testes E2E & Perf**: executar Playwright, testes de carga (k6) focado em agendamento/cancelamento.

### Dependências Técnicas

- Provisionamento Logto (clientId/secret) antes do desenvolvimento front-end autenticado.
- Credenciais SendGrid/Twilio definidas antes do módulo de notificações.
- Acesso a SQL Server (dev/staging/prod) com roles apropriadas.
- Pipeline de deploy (Azure DevOps ou GitHub Actions) com permissões nas subscrições cloud.

## Monitoramento e Observabilidade

- **Métricas (Prometheus)**: `http_requests_total` (labels: method, endpoint, status), `appointment_created_total`, `notifications_sent_total`, `notification_failed_total`, `signalr_connections_active`, `db_query_duration_seconds` (EF instrumentation).
- **Logs**: Serilog `Information` para operações bem-sucedidas (com correlação `TraceId`), `Warning` para violações de negócio, `Error` para exceções não tratadas. Logger configurado via `logging.md`, sem dados sensíveis.
- **Dashboards**: Grafana com panels: throughput de agendamentos, latência média API, falhas de notificações, conexões SignalR.
- **Alertas**: Application Insights alert rules para `availability < 95%`, `notification_failed_total`>5 em 10min, tempo de resposta >2s p95.
- **Tracing**: OpenTelemetry exporter -> Azure Monitor. Cada request carrega `X-Correlation-Id` propagado para SignalR e jobs.

## Considerações Técnicas

### Decisões Principais

- **Clean Architecture**: favorece testabilidade e isolamento de domínios; permite evoluir para microserviços se necessário.
- **Logto para identidade**: terceiriza gestão de usuários, reduz esforço em compliance OIDC/OpenID, facilita social login futuro.
- **EF Core + SQL Server**: combina produtividade com suporte a transações complexas; índices garantem performance para ~50 usuários.
- **SignalR**: provê atualizações quase em tempo real para dashboards e agendas sem necessidade de polling.
- **SendGrid/Twilio**: escolhidos por APIs REST simples, SDKs maduros, opção de canais múltiplos (email/WhatsApp/SMS).
- **React + Shadcn/Tailwind**: aderente à regra interna `react.md`, reduz custo de design system inicial.

Alternativas avaliadas: Next.js (descartado para manter SPA simples), IdentityServer self-hosted (mais complexo), mensageria RabbitMQ (adiado até haver volume maior; hoje fila baseada em DB suficiente).

### Riscos Conhecidos

- **Dependência de provedores externos**: falhas SendGrid/Twilio podem impactar lembretes. Mitigação: retries + fallback canal (email quando WhatsApp falhar) e dashboards de falha.
- **Precisão de disponibilidade**: conflitos concorrentes ao agendar. Mitigação: transações com `SERIALIZABLE` ou lock otimista (concurrency token) garantindo exclusão mútua por barbeiro/horário.
- **Gestão de templates**: mensagens inconsistentes se alteradas manualmente. Mitigação: armazenar templates versionados em tabela + revisão.
- **Escalabilidade futura**: crescimento além de 50 usuários requer fila dedicada. Mitigação: design modular facilita mover `notification_jobs` para RabbitMQ posteriormente.
- **Conformidade LGPD**: necessidade de consentimento auditável. Mitigação: campo `clients.consent_communication` e logs de alteração.

### Requisitos Especiais

- **Retenção de Dados**: agendamentos e pagamentos preservados 5 anos; logs Application Insights 90 dias (exportável). Implementar job anual para arquivar registros antigos em storage frio.
- **Timezone**: armazenar `DateTime` em UTC, converter na UI conforme fuso configurado da barbearia.
- **Security Hardening**: aplicar HTTP headers (`Strict-Transport-Security`, `Content-Security-Policy`), proteger SignalR com bearer tokens, limitar upload (caso futuro de anexos) a 5MB.

### Conformidade com Padrões

- **React**: apenas componentes funcionais, TypeScript `.tsx`, Tailwind + Shadcn, React Query; sem `styled-components`. Hooks nomeados com `use*` e componentes <300 linhas.
- **HTTP**: endpoints RESTful com nomes em inglês (plural), verbos POST para ações; paginação limit/offset, partial response (`fields`). Documentação OpenAPI atualizada.
- **Code Standards**: camelCase/PascalCase conforme `code-standard.md`; funções com responsabilidade única; dependências invertidas via interfaces.
- **SQL**: tabelas `snake_case` plural, PK/ FK `*_id`, `created_at/updated_at`, migrations obrigatórias.
- **Tests**: Jest + sinon, xUnit; organização em `/test/unit` e `/test/integration`; seguir AAA.
- **Logging**: Serilog (sem `Console.WriteLine`), níveis apropriados.
- **Unit of Work**: Implementação conforme guia; todos os use cases mutáveis dependem de `IUnitOfWork`.
