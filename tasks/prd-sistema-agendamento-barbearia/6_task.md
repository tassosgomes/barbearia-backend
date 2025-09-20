---
status: pending
parallelizable: true
blocked_by: ["3.0","4.0"]
---

<task_context>
<domain>engine/backend/notifications</domain>
<type>implementation</type>
<scope>core_feature</scope>
<complexity>high</complexity>
<dependencies>external_apis,database,temporal</dependencies>
<unblocks>7.0,12.0</unblocks>
</task_context>

# Tarefa 6.0: Construir infraestrutura de notificações e jobs em background

## Visão Geral
Implementar fila persistida de notificações, serviço de envio assíncrono e integrações com SendGrid (email) e Twilio (WhatsApp/SMS) para lembretes e confirmações.

## Requisitos
- Criar tabela `notification_jobs` com estados (pending, sent, failed) e retenção configurável.
- Implementar `INotificationDispatcher` e hosted service para processamento em lote com políticas de retry.
- Integrar com APIs SendGrid e Twilio, incluindo fallback e tratamento de erros.
- Garantir observabilidade básica (métricas de fila) e testes cobrindo fluxos feliz e falhas.

## Subtarefas
- [ ] 6.1 Modelar job de notificação e migrations correspondentes.
- [ ] 6.2 Implementar dispatcher/enqueuer nos casos de uso de agendamento/cancelamento.
- [ ] 6.3 Desenvolver hosted service para envio com retries e fallback canal.
- [ ] 6.4 Criar testes unitários (dispatcher) e integração com mocks de SendGrid/Twilio.

## Detalhes de Implementação
- Ver "Pontos de Integração > SendGrid/Twilio" e "Arquitetura > Background Services".
- Aplicar guidelines `rules/logging.md` para registro de falhas.

## Critérios de Sucesso
- Jobs enfileirados são processados e marcados como `sent` com métricas registradas.
- Falhas externas resultam em retry e marcação `failed` após limite, com logs nível ERROR.
- Testes simulam respostas 2xx/4xx e garantem comportamento idempotente.
