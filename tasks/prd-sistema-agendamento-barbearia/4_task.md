---
status: pending
parallelizable: true
blocked_by: ["2.0","3.0"]
---

<task_context>
<domain>engine/backend/scheduling</domain>
<type>implementation</type>
<scope>core_feature</scope>
<complexity>high</complexity>
<dependencies>database,http_server,temporal</dependencies>
<unblocks>5.0,6.0,7.0,9.0,12.0</unblocks>
</task_context>

# Tarefa 4.0: Implementar módulo de agendamentos e hub SignalR

## Visão Geral
Desenvolver use cases, repositórios, endpoints REST e hub SignalR responsáveis pelo ciclo completo de agendamento (listar disponibilidade, criar, reagendar, cancelar) garantindo regras de negócio e notificações em tempo real.

## Requisitos
- Implementar casos de uso `CreateAppointment`, `ListAvailability`, `CancelAppointment`, `RescheduleAppointment`.
- Criar controllers REST, validações FluentValidation e responses alinhados à Tech Spec.
- Expor hub SignalR `/hubs/scheduling` com eventos para atualizações de agenda.
- Cobrir com testes unitários e integração (use cases + controllers) e assegurar transações via `IUnitOfWork`.

## Subtarefas
- [ ] 4.1 Implementar casos de uso e handlers com regras de disponibilidade/antecedência.
- [ ] 4.2 Criar repositórios EF Core, mappers e consultas otimizadas (índices).
- [ ] 4.3 Desenvolver endpoints REST e hub SignalR, incluindo documentação OpenAPI.
- [ ] 4.4 Escrever testes unitários (use cases) e integração (endpoints) incluindo cenários de concorrência.

## Detalhes de Implementação
- Referir "Design de Implementação > Endpoints de API" e "Sequenciamento > Casos de Uso de Agenda".
- Aplicar recomendações `rules/http.md`, `rules/tests.md`, `rules/unit-of -work.md`.

## Critérios de Sucesso
- Endpoints de agendamento respondem em <2s com regras respeitadas.
- SignalR notifica atualizações de slots em ambiente local com múltiplos clientes.
- Suite de testes cobre fluxos principais e alternativos de agendamento.
