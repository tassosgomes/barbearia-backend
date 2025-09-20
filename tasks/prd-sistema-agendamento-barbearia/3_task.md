---
status: pending
parallelizable: true
blocked_by: ["1.0"]
---

<task_context>
<domain>engine/backend/security</domain>
<type>implementation</type>
<scope>configuration</scope>
<complexity>medium</complexity>
<dependencies>external_apis,http_server</dependencies>
<unblocks>4.0,5.0,8.0</unblocks>
</task_context>

# Tarefa 3.0: Integrar autenticação e autorização com Logto

## Visão Geral
Configurar o provedor Logto como identidade OIDC para a API, habilitando validação de tokens JWT, políticas de roles e middlewares de autorização reutilizáveis pelo front-end.

## Requisitos
- Registrar aplicação Logto e parametrizar clientId, secret e scopes.
- Implementar `AddAuthentication().AddJwtBearer()` com validação via JWKS Logto.
- Criar políticas por role (admin, manager, barber, client) e attribute helpers.
- Garantir tratamento de falhas (401/403) e logs conforme `rules/http.md` e `logging.md`.

## Subtarefas
- [ ] 3.1 Configurar opções Logto no appsettings e registrar em DI.
- [ ] 3.2 Implementar middleware de autenticação e policies de autorização.
- [ ] 3.3 Criar testes de integração garantindo fluxo de login e proteção de endpoints.
- [ ] 3.4 Documentar no README instruções para obter tokens em ambientes locais.

## Detalhes de Implementação
- Ver "Pontos de Integração > Logto (OIDC)" e "Sequenciamento > Autenticação & Middleware".

## Critérios de Sucesso
- Endpoints protegidos retornam 401/403 corretamente quando não autenticados.
- Testes de integração validam obtenção de claims e roles esperadas.
- Time consegue configurar Logto localmente seguindo a documentação fornecida.
