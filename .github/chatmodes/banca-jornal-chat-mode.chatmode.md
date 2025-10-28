---
description: 'Description of the custom chat mode.'
tools: ['runCommands', 'runTasks', 'edit', 'runNotebooks', 'search', 'new', 'ms-mssql.mssql/mssql_show_schema', 'ms-mssql.mssql/mssql_connect', 'ms-mssql.mssql/mssql_disconnect', 'ms-mssql.mssql/mssql_list_servers', 'ms-mssql.mssql/mssql_list_databases', 'ms-mssql.mssql/mssql_get_connection_details', 'ms-mssql.mssql/mssql_change_database', 'ms-mssql.mssql/mssql_list_tables', 'ms-mssql.mssql/mssql_list_schemas', 'ms-mssql.mssql/mssql_list_views', 'ms-mssql.mssql/mssql_list_functions', 'ms-mssql.mssql/mssql_run_query', 'extensions', 'todos', 'usages', 'vscodeAPI', 'problems', 'changes', 'testFailure', 'openSimpleBrowser', 'fetch', 'githubRepo', 'ms-python.python/getPythonEnvironmentInfo', 'ms-python.python/getPythonExecutableCommand', 'ms-python.python/installPythonPackage', 'ms-python.python/configurePythonEnvironment']
---
Define the purpose of this chat mode and how AI should behave: response style, available tools, focus areas, and any mode-specific instructions or constraints.

---

## Critérios para Desenvolvimento em Camadas

Ao gerar código ou sugerir arquitetura, siga estas diretrizes para sistemas divididos em camadas: FRONT/DESKTOP, MODEL, APPLICATION e REPOSITORY.

### 1. FRONT/DESKTOP
- Responsável pela interface com o usuário e apresentação dos dados.
- Separe lógica de apresentação da lógica de negócio.
- Utilize padrões como MVVM ou MVC.
- Comunique-se com a camada Application via serviços ou APIs.
- Realize validações básicas antes do envio de dados.

### 2. MODEL
- Representa entidades de domínio e regras de negócio puras.
- Mantenha as classes independentes de frameworks e infraestrutura.
- Utilize Value Objects e Entities conforme DDD.
- Aplique princípios SOLID, especialmente SRP e OCP.

### 3. APPLICATION
- Orquestra casos de uso e coordena Model e Repository.
- Não inclua lógica de apresentação ou persistência.
- Utilize injeção de dependências para acoplar Model e Repository.
- Aplique DIP e ISP.

### 4. REPOSITORY
- Responsável pelo acesso e persistência de dados.
- Defina interfaces para repositórios no domínio.
- Implemente padrões Repository e Unit of Work.
- Aplique DIP e LSP.

### Boas Práticas Gerais
- Aplique todos os princípios SOLID.
- Garanta testabilidade isolada de cada camada.
- Separe responsabilidades claramente.
- Utilize injeção de dependências.
- Documente código e decisões arquiteturais.
- Centralize tratamento de erros e logs.

### Fluxo de Execução
1. FRONT/DESKTOP solicita ação do usuário.
2. APPLICATION recebe, valida e coordena o fluxo.
3. MODEL representa e valida entidades.
4. REPOSITORY persiste ou recupera dados.