# Documento de Requisitos de Produto (PRD) — Sistema de Agendamento de Barbearia

## Visão Geral

O Sistema de Agendamento de Barbearia é uma plataforma web responsiva que centraliza o ciclo completo de atendimento em barbearias de médio porte. O produto atende clientes finais, barbeiros e gestores, substituindo processos manuais de agenda por uma solução digital integrada que reduz conflitos de horário, melhora a comunicação com o cliente e oferece visibilidade operacional e financeira em tempo real. A iniciativa busca eliminar falhas de agendamento, diminuir cancelamentos de última hora e oferecer dados confiáveis para tomada de decisão diária, mantendo uma experiência mobile-first para clientes e um painel administrativo unificado para a equipe interna.

## Objetivos

- Reduzir o tempo médio de agendamento para menos de 2 minutos em fluxos mobile e desktop.
- Diminuir a taxa de cancelamentos de última hora em pelo menos 50% em três meses por meio de lembretes proativos.
- Alcançar taxa de ocupação média acima de 80% ao fornecer visibilidade de horários disponíveis e reagendamentos rápidos.
- Garantir disponibilidade do sistema em 95%+ durante o horário comercial da barbearia.
- Manter tempo de resposta das principais APIs abaixo de 2 segundos para consultas e criação de agendamentos.

## Histórias de Usuário

- Como **visitante**, quero visualizar serviços e horários disponíveis sem login para decidir rapidamente se a barbearia atende minhas necessidades.
- Como **cliente novo**, quero criar um agendamento informando meus dados básicos para receber confirmação imediata do meu horário.
- Como **cliente recorrente**, quero acessar meu histórico de agendamentos para repetir serviços e reagendar com poucos cliques.
- Como **cliente**, quero cancelar ou reagendar um horário até 2 horas antes para evitar cobranças e liberar o slot para outra pessoa.
- Como **cliente**, quero receber lembretes automáticos via email ou WhatsApp para recordar meu compromisso e reduzir a chance de esquecer.
- Como **barbeiro**, quero visualizar minha agenda diária consolidada para planejar meu tempo e confirmar presença dos clientes.
- Como **barbeiro**, quero bloquear horários específicos (férias, pausas) para evitar que clientes agendem nesses períodos.
- Como **gerente**, quero cadastrar e atualizar dados de serviços, barbeiros e horários de atendimento para manter a operação alinhada com a oferta real.
- Como **administrador**, quero acompanhar métricas diárias de agendamento, cancelamentos e receita gerada para tomar decisões operacionais e financeiras.
- Como **administrador**, quero registrar pagamentos recebidos no caixa e calcular automaticamente comissões de barbeiros para fechar o dia com precisão.

## Funcionalidades Principais

### Agendamento Online

Plataforma voltada ao cliente que permite navegação pelos serviços, escolha de profissional, seleção de data e horário com disponibilidade em tempo real e confirmação via canais digitais.

Requisitos funcionais:
1. O sistema deve exibir catálogo de serviços ativos com preço, duração e descrição resumida.
2. O cliente deve conseguir filtrar horários por barbeiro preferido e tipo de serviço.
3. O sistema deve impedir agendamentos em horários não disponíveis ou fora do expediente configurado.
4. Após confirmação, o cliente deve receber notificação automática via email e/ou WhatsApp com detalhes do agendamento.
5. O cliente deve conseguir reagendar ou cancelar agendamentos confirmados com antecedência mínima de 2 horas.
6. O sistema deve suportar agendamentos para múltiplos serviços no mesmo horário, respeitando a duração total.

### Painel Administrativo e Operacional

Interface para gestores, administradores e barbeiros controlarem agenda consolidada, cadastros e configurações gerais.

Requisitos funcionais:
7. Administradores devem cadastrar, editar, ativar ou desativar barbeiros, definindo horários de trabalho e especialidades.
8. Administradores devem gerenciar catálogo de serviços (criar, editar, definir duração, preço e status ativo/inativo).
9. O painel deve apresentar dashboard diário com métricas de agendamentos, cancelamentos, ocupação e próximos atendimentos.
10. Barbeiros devem ter visão individual da própria agenda com possibilidade de confirmar atendimentos ou indicar no-show.
11. Gestores devem poder bloquear horários (manutenção, eventos internos) e esses bloqueios devem refletir imediatamente na vitrine de agendamentos.
12. O sistema deve registrar histórico de alterações em agendamentos para auditoria (quem criou, alterou, cancelou e quando).

### Gestão Financeira e Relatórios

Módulo administrativo dedicado a registrar pagamentos recebidos no ponto de venda, consolidar receitas e calcular comissões de barbeiros.

Requisitos funcionais:
13. Administradores devem registrar pagamentos associados a agendamentos concluídos, com forma de pagamento (dinheiro, cartão, Pix, outros).
14. O sistema deve calcular comissões de barbeiros com base em percentuais configurados por serviço ou profissional.
15. Relatórios diários devem apresentar resumo de receitas, comissões, número de serviços prestados e distribuição por forma de pagamento.
16. O sistema deve exportar relatórios financeiros em formato CSV ou PDF para integração manual com contabilidade.
17. Gestores devem visualizar histórico financeiro por período (diário, semanal, mensal) com filtros por barbeiro e serviço.

## Experiência do Usuário

- **Personas e necessidades**: visitantes necessitam de uma jornada rápida para descoberta de serviços; clientes valorizam conveniência, confirmação imediata e lembretes; barbeiros precisam de visão clara de agenda e ferramentas de bloqueio; gestores buscam controle completo e dados confiáveis.
- **Fluxos principais**: jornada cliente (home → serviços → barbeiro → data/horário → dados pessoais → resumo → confirmação → lembretes); jornada barbeiro (login → agenda diária → confirmações/reagendamentos → bloqueios); jornada gestor (login → dashboard → cadastros → relatórios).
- **UI/UX**: priorização mobile-first com componentes responsivos em React; uso de CTA principal “Agendar Agora”; destaque de horários disponíveis e estados (confirmado, pendente, cancelado); mensagens de erro claras com orientação para correção; suporte a tema acessível com contraste adequado.
- **Acessibilidade**: cumprimento de diretrizes WCAG 2.1 nível AA, incluindo navegação por teclado, etiquetas para leitores de tela, validação assistiva em formulários e conteúdo textual alternativo para imagens dos barbeiros.

## Restrições Técnicas de Alto Nível

- **Stack**: Frontend em React 18+, Material UI/Chakra, React Query e React Hook Form; backend em .NET 8/ASP.NET Core com Entity Framework Core; banco de dados SQL Server.
- **Integrações**: serviços de envio de email e WhatsApp para notificações; SignalR para atualização em tempo real da agenda; autenticação via JWT com refresh tokens.
- **Performance**: tempo de resposta máximo de 2 segundos nas APIs de agendamento; suporte mínimo a 100 usuários concorrentes sem degradação perceptível.
- **Confiabilidade**: disponibilidade de 95%+ em horário comercial; logs centralizados de atividades críticas; auditoria de alterações em agendamentos e pagamentos.
- **Segurança e privacidade**: trânsito protegido via HTTPS; sanitização de dados de entrada; conformidade com LGPD para tratamento de dados pessoais (consentimento para comunicações, direito de exclusão); rate limiting em endpoints sensíveis.
- **Infraestrutura**: deploy em containers Docker com pipeline CI/CD (Azure/AWS) já disponível; monitoramento contínuo via Application Insights ou equivalente.

## Não-Objetivos (Fora de Escopo)

- Processamento de pagamentos on-line integrado a gateways externos; registro financeiro é manual após recebimento presencial.
- Desenvolvimento de aplicativos mobile nativos; a entrega atual limita-se a PWA responsiva.
- Programa de fidelidade, pontuação ou benefícios de clientes recorrentes.
- Integração com redes sociais ou chatbots externos para captura de agendamentos.
- Recursos de inteligência artificial para recomendação de horários ou previsão de demanda.
- Relatórios analíticos avançados ou dashboards de BI além dos relatórios operacionais previstos.

## Questões em Aberto

- Confirmar volume médio de agendamentos diários e pico de uso para dimensionamento inicial e testes de carga.
- Definir política formal de cancelamento (cobrança de multa, exceções) e se haverá comunicação específica para no-show.
- Selecionar provedores oficiais de envio de WhatsApp e email (custo, limites, SLA) e formalizar integrações necessárias.
- Especificar padrões visuais da marca (paleta, tipografia, tom de voz) e guidelines a seguir na construção da interface.
- Determinar se haverá suporte multiunidade no futuro próximo, impactando modelo de dados e gestão de agendas.
- Validar necessidade de multilíngue ou localização específica (ex.: português/inglês) para clientes estrangeiros.
