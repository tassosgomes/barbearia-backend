---
status: pending
parallelizable: true
blocked_by: ["1.0"]
---

<task_context>
<domain>engine/backend/core</domain>
<type>implementation</type>
<scope>core_feature</scope>
<complexity>high</complexity>
<dependencies>database</dependencies>
<unblocks>4.0,5.0,6.0</unblocks>
</task_context>

# Tarefa 2.0: Modelar domínio e preparar migrações do banco de dados

## Visão Geral
Definir entidades e value objects do domínio (clientes, barbeiros, serviços, agendamentos, pagamentos) e implementar o contexto EF Core com migrations alinhadas às convenções SQL estabelecidas.

## Requisitos
- Modelar agregados com regras de negócio descritas no PRD.
- Mapear entidades no DbContext com configurações Fluent API alinhadas ao padrão snake_case.
- Criar migrations iniciais e seeds para ambientes de desenvolvimento.
- Implementar testes de unidade para garantir integridade das regras de domínio.

## Subtarefas
- [ ] 2.1 Mapear entidades e value objects conforme "Design de Implementação > Modelos de Dados".
- [ ] 2.2 Configurar DbContext, mappings Fluent API e índices obrigatórios.
- [ ] 2.3 Gerar migrations base e script de seed para ambientes de dev/test.
- [ ] 2.4 Criar testes unitários de domínio (xUnit) cobrindo validações de agendamento e cancelamento.

## Detalhes de Implementação
- Referenciar "Modelos de Dados" e "SQL Server Schema" da Tech Spec.
- Ajustar nomenclaturas conforme `rules/sql.md`.

## Critérios de Sucesso
- Migrations aplicam sem erros em banco SQL Server local.
- Testes de domínio passam com cobertura mínima de 85% para regras principais.
- Documentação das entidades e seeds disponível no repositório.
