# Diretrizes do Repositório

## Estrutura do Projeto e Organização dos Módulos
Agrupe projetos de tempo de execução em `src/` e projetos de teste em `tests/`. Busque uma divisão de arquitetura limpa, como `src/Barbearia.Api` (host ASP.NET Core), `src/Barbearia.Application` (manipuladores e validadores CQRS), `src/Barbearia.Domain` (entidades, objetos de valor, agregados) e `src/Barbearia.Infrastructure` (contexto EF Core, repositórios, mensagens). Dentro de cada camada, organize por capacidade de negócios: por exemplo, `Agendamento`, `Administração`, `Notificações`, `Segurança`, `Finanças`. Mantenha abstrações compartilhadas em `src/Barbearia.Shared` somente quando a reutilização for transversal. Armazene diagramas ou ADRs em `docs/` assim que disponíveis para manter `src/` focado no código.

Siga os seguitens Padrões de Codificação:

1. Utilize camelCase para a declaração de métodos, funções e variáveis, PascalCase para classes e interfaces e kebab-case para arquivos e diretórios
2. Evite abreviações, mas também não escreva nomes muito longos (com mais de 30 caracteres)
3. Declare constantes para representar magic numbers com legibilidade
4. Os métodos e funções devem executar uma ação clara e bem definida, e isso deve ser refletido no seu nome, que deve começar por um verbo, nunca um substantivo
5. Sempre que possível, evite passar mais de 3 parâmetros, dê preferência para o uso de objetos caso necessário
6. Evite efeitos colaterais, em geral um método ou função deve fazer uma mutação ou consulta, nunca permita que uma consulta tenha efeitos colaterais
7. Nunca faça o aninhamento de mais de dois if/else, sempre dê preferência por early returns
8. Nunca utilize flag params para chavear o comportamento de métodos e funções, nesses casos faça a extração para métodos e funções com comportamentos específicos
9. Evite métodos longos, com mais de 50 linhas
10. Evite classes longas, com mais de 300 linhas
11. Sempre inverta as dependências para recursos externos tanto nos use cases quanto nos interface adapters utilizando o Dependency Inversion Principle
12. Evite linhas em branco dentro de métodos e funções
13. Evite o uso de comentários sempre que possível
14. Nunca declare mais de uma variável na mesma linha
15. Declare as variáveis o mais próximo possível de onde serão utilizadas
16. Prefira composição do que herança sempre que possível
17. Todo o código deve ser escrito em inglês

## Comandos de Construção, Teste e Desenvolvimento
- `dotnet restore` — instala dependências do NuGet.
- `dotnet build` — compila todos os projetos com avisos tratados como erros na CI.
- `dotnet watch run --project src/Barbearia.Api` — recarrega a API durante o desenvolvimento local.
- `dotnet test --collect:"XPlat Code Coverage"` — executa o conjunto completo de testes e exporta os resultados da cobertura.
- `dotnet ef database update --project src/Barbearia.Infrastructure` — aplica migrações pendentes do Entity Framework à instância do SQL Server.

## Estilo de Codificação e Convenções de Nomenclatura
Use recursos do C# 12 no .NET 8 com recuo de quatro espaços e namespaces com escopo de arquivo. Prefira `PascalCase` para classes, registros e membros públicos; `camelCase` para campos locais, campos privados com sublinhado à esquerda e `UPPER_SNAKE_CASE` apenas para constantes. Mantenha os sufixos de DTOs, comandos e consultas explícitos (`CreateAppointmentCommand`). Execute `dotnet format` antes de enviar e mantenha os tipos de referência anuláveis ​​habilitados.

## Diretrizes de Teste
Adote projetos de teste baseados em xUnit chamados `<Project>.Tests`. Espelhe a nomenclatura da pasta `src/`, por exemplo, `tests/Barbearia.Application.Tests/Scheduling`. Nomeie os testes usando `MethodName_State_ExpectedBehavior`. Busque uma cobertura de linha ≥80% nas camadas de aplicação e domínio; adicione testes de integração para contratos de API usando `WebApplicationFactory`. Semeie data fixtures determinísticos e evite afetar serviços ativos — simule hubs SignalR, mensagens externas e gateways de notificação.

## Diretrizes de solicitação de confirmação e pull
Siga Commits Convencionais (`feat(agendamento): evitar slots sobrepostos`). Os commits devem permanecer pequenos, compilados e incluir migrações ou documentos atualizados quando o comportamento mudar. As solicitações pull devem descrever o contexto de negócios, fazer referência à tarefa PRD relevante, listar evidências de teste (saída `dotnet test`, capturas de tela para Swagger) e destacar os impactos da migração. Solicite pelo menos uma revisão, garanta a aprovação do CI e anexe notas de implementação quando a mudança afetar a infraestrutura ou a postura de segurança.

### 🔖 Tipos de commit

| Tipo     | Descrição                                                                 |
|----------|---------------------------------------------------------------------------|
| feat     | Nova funcionalidade                                                       |
| fix      | Correção de bug                                                           |
| docs     | Alterações na documentação                                                |
| style    | Formatação, identação, espaços, etc. (sem alteração de código funcional) |
| refactor | Refatoração de código (sem mudança de funcionalidade)                    |
| test     | Adição ou modificação de testes                                           |
| chore    | Tarefas de manutenção (build, configs, dependências, etc.)               |

## Dicas de Segurança e Configuração
Armazene segredos via `dotnet user-secrets` ou variáveis ​​de ambiente; nunca confirme strings de conexão ou chaves de API reais. Mantenha a string de conexão padrão do SQL Server em `appsettings.Development.json` usando os valores de composição locais do Docker. Valide todas as entradas no lado do servidor, habilite o registro por meio de coletores Serilog ou Application Insights e configure a autorização baseada em função antes de expor novos endpoints.