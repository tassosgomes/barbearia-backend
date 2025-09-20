---
status: pending
parallelizable: true
blocked_by: ["1.0","4.0","6.0"]
---

<task_context>
<domain>engine/backend/observability</domain>
<type>configuration</type>
<scope>performance</scope>
<complexity>medium</complexity>
<dependencies>external_apis,http_server</dependencies>
<unblocks>release</unblocks>
</task_context>

# Tarefa 7.0: Configurar observabilidade, métricas e hardening backend

## Visão Geral
Aplicar logging estruturado, métricas Prometheus, tracing OpenTelemetry e configurações de segurança (rate limiting, headers) garantindo visibilidade e resiliência da API.

## Requisitos
- Configurar Serilog com sink Application Insights e correlação `TraceId`.
- Expor métricas customizadas (appointments, notifications) e HTTP padrão.
- Configurar tracing distribuído e propagação de `X-Correlation-Id` para SignalR/jobs.
- Implementar rate limiting, headers de segurança e testes de fumaça para endpoints de saúde.

## Subtarefas
- [ ] 7.1 Configurar Serilog + sinks (console, App Insights) com filtros adequados.
- [ ] 7.2 Implementar OpenTelemetry (metrics/traces) e endpoint `/metrics` protegido.
- [ ] 7.3 Aplicar rate limiting, security headers e validação de CORS seguindo spec.
- [ ] 7.4 Criar testes/gravações k6 básicas e documentação de dashboards/alertas.

## Detalhes de Implementação
- Seções "Monitoramento e Observabilidade" e "Considerações Técnicas > Requisitos Especiais" da Tech Spec.
- Seguir `rules/logging.md`.

## Critérios de Sucesso
- Métricas e traces visíveis em ambiente de teste, com dashboards configurados.
- Rate limiting e headers validados via testes automatizados.
- Documentação descrevendo procedimentos de monitoramento e alertas.
