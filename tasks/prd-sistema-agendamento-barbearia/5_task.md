---
status: pending
parallelizable: true
blocked_by: ["2.0","3.0","4.0"]
---

<task_context>
<domain>engine/backend/admin</domain>
<type>implementation</type>
<scope>core_feature</scope>
<complexity>high</complexity>
<dependencies>database,http_server</dependencies>
<unblocks>6.0,10.0,11.0</unblocks>
</task_context>

# Tarefa 5.0: Entregar APIs administrativas e financeiras com relatórios

## Visão Geral
Construir endpoints para gestão de barbeiros, serviços, dashboard diário e controle financeiro (pagamentos, relatórios CSV) alinhados às políticas de acesso administrativo.

## Requisitos
- Implementar CRUD de barbeiros e serviços com validação de horários e especialidades.
- Disponibilizar endpoints de dashboard (`/dashboard/daily-metrics`) e agregações financeiras.
- Implementar registro de pagamentos, cálculo de comissões e exportação CSV/PDF.
- Garantir políticas de autorização corretas e testes cobrindo fluxos principais/alternativos.

## Subtarefas
- [ ] 5.1 Criar casos de uso e repositórios para gestão de barbeiros/serviços.
- [ ] 5.2 Desenvolver endpoints de dashboard e métricas financeiras.
- [ ] 5.3 Implementar exportação CSV/PDF e validações de permissão.
- [ ] 5.4 Escrever testes unitários e de integração, incluindo cenários de acesso negado.

## Detalhes de Implementação
- Ver "Funcionalidades Principais > Painel Administrativo" e "Gestão Financeira" na Tech Spec.
- Seguir convenções `rules/http.md` e `rules/tests.md`.

## Critérios de Sucesso
- Endpoints administrativos retornam dados consistentes com seeds/massa de teste.
- Exportações geradas em CSV/PDF com cabeçalhos corretos e codificação UTF-8.
- Testes garantindo cobertura de fluxos de autorização e validações de negócio.
