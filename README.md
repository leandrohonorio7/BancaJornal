# Sistema de Gerenciamento de Banca de Jornal

Sistema desktop completo para gerenciamento de banca de jornal, desenvolvido em C# com WPF, seguindo arquitetura em camadas e princípios SOLID.

## 🏗️ Arquitetura

O sistema foi desenvolvido seguindo uma arquitetura em camadas bem definida:

### Camadas

```
BancaJornal/
├── BancaJornal.Model/          # Camada de Domínio
├── BancaJornal.Repository/     # Camada de Persistência
├── BancaJornal.Application/    # Camada de Aplicação
└── BancaJornal.Desktop/        # Camada de Apresentação
```

#### 1. **MODEL** (Domínio)
- **Responsabilidade**: Entidades de domínio e regras de negócio puras
- **Padrões**: DDD (Domain-Driven Design), Value Objects, Entities
- **Princípios SOLID**: SRP, OCP
- **Entidades principais**:
  - `Produto`: Gerencia dados e regras de produtos
  - `Venda`: Aggregate Root para vendas
  - `ItemVenda`: Value Object de itens vendidos

#### 2. **REPOSITORY** (Persistência)
- **Responsabilidade**: Acesso e persistência de dados
- **Padrões**: Repository Pattern, Unit of Work
- **Princípios SOLID**: DIP, LSP, ISP
- **Tecnologia**: Entity Framework Core + SQLite
- **Componentes**:
  - Interfaces de repositórios (`IProdutoRepository`, `IVendaRepository`)
  - Implementações concretas usando EF Core
  - `IUnitOfWork` para gerenciamento transacional

#### 3. **APPLICATION** (Casos de Uso)
- **Responsabilidade**: Orquestração de casos de uso e coordenação entre camadas
- **Padrões**: Service Layer, DTOs
- **Princípios SOLID**: DIP, ISP, SRP
- **Serviços**:
  - `ProdutoService`: Gerenciamento de produtos
  - `VendaService`: Processamento de vendas
  - `DashboardService`: Agregação de dados para dashboard

#### 4. **DESKTOP** (Apresentação)
- **Responsabilidade**: Interface com usuário
- **Padrões**: MVVM (Model-View-ViewModel)
- **Tecnologia**: WPF (Windows Presentation Foundation)
- **Componentes**:
  - Views (XAML): Interface visual
  - ViewModels: Lógica de apresentação
  - Injeção de Dependências via Microsoft.Extensions.DependencyInjection

## ✨ Funcionalidades

### 1. Dashboard
- Métricas em tempo real:
  - Produtos em estoque
  - Produtos com estoque baixo
  - Total de vendas do mês
  - Valor total de vendas
- Top 10 produtos mais vendidos no mês

### 2. Gerenciamento de Produtos (CRUD)
- Cadastro de produtos com validações
- Edição de dados
- Controle de estoque
- Ativação/desativação de produtos
- Busca rápida por nome ou descrição
- Suporte a código de barras único

### 3. Registro de Vendas
- Busca rápida de produtos (mesmo sem estoque)
- Adição de múltiplos itens
- Cálculo automático de valores
- Atualização automática de estoque
- Campo de observações
- Transações atômicas (rollback em caso de erro)

## 🛠️ Tecnologias Utilizadas

- **.NET 8.0**: Framework principal
- **WPF**: Interface desktop
- **Entity Framework Core 8.0**: ORM
- **SQLite**: Banco de dados
- **CommunityToolkit.Mvvm**: Implementação MVVM
- **Microsoft.Extensions.DependencyInjection**: Injeção de dependências

## 🚀 Como Executar

### Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 ou superior (recomendado)
- Windows 10 ou superior

### Passos

1. **Clone ou navegue até o repositório**
   ```bash
   cd c:\Users\leandro.honorio.lima\source\repo-ia
   ```

2. **Restaurar dependências**
   ```bash
   dotnet restore BancaJornal.sln
   ```

3. **Compilar o projeto**
   ```bash
   dotnet build BancaJornal.sln
   ```

4. **Executar a aplicação**
   ```bash
   dotnet run --project BancaJornal.Desktop
   ```

   Ou abra `BancaJornal.sln` no Visual Studio e pressione F5.

### Primeira Execução

Na primeira execução, o banco de dados SQLite (`bancajornal.db`) será criado automaticamente no diretório de execução.

## 📋 Princípios SOLID Aplicados

### Single Responsibility Principle (SRP)
- Cada classe tem uma responsabilidade única e bem definida
- Serviços separados para cada domínio (Produto, Venda, Dashboard)
- ViewModels específicos para cada tela

### Open/Closed Principle (OCP)
- Entidades de domínio abertas para extensão via métodos
- Comportamentos podem ser estendidos sem modificar código existente

### Liskov Substitution Principle (LSP)
- Implementações de repositórios podem ser substituídas sem quebrar o código
- Interfaces bem definidas garantem contratos consistentes

### Interface Segregation Principle (ISP)
- Interfaces específicas para cada repositório
- Clientes não dependem de métodos que não utilizam

### Dependency Inversion Principle (DIP)
- Todas as dependências são baseadas em abstrações (interfaces)
- Injeção de dependências configurada centralmente
- Camadas superiores não conhecem implementações concretas

## 🔄 Fluxo de Execução

```
1. DESKTOP (View) → Usuário interage
2. DESKTOP (ViewModel) → Valida entrada e chama serviço
3. APPLICATION (Service) → Coordena operação
4. MODEL (Entity) → Aplica regras de negócio
5. REPOSITORY → Persiste dados
6. Unit of Work → Confirma transação
```

## 📁 Estrutura de Diretórios

```
BancaJornal/
├── BancaJornal.Model/
│   └── Entities/
│       ├── Produto.cs
│       ├── Venda.cs
│       └── ItemVenda.cs
│
├── BancaJornal.Repository/
│   ├── Data/
│   │   └── BancaJornalDbContext.cs
│   ├── Interfaces/
│   │   ├── IProdutoRepository.cs
│   │   ├── IVendaRepository.cs
│   │   └── IUnitOfWork.cs
│   └── Repositories/
│       ├── ProdutoRepository.cs
│       ├── VendaRepository.cs
│       └── UnitOfWork.cs
│
├── BancaJornal.Application/
│   ├── DTOs/
│   │   ├── ProdutoDto.cs
│   │   ├── VendaDto.cs
│   │   └── DashboardDto.cs
│   └── Services/
│       ├── ProdutoService.cs
│       ├── VendaService.cs
│       └── DashboardService.cs
│
└── BancaJornal.Desktop/
    ├── ViewModels/
    │   ├── MainViewModel.cs
    │   ├── DashboardViewModel.cs
    │   ├── ProdutoViewModel.cs
    │   └── VendaViewModel.cs
    ├── Views/
    │   ├── MainWindow.xaml
    │   ├── DashboardView.xaml
    │   ├── ProdutoView.xaml
    │   └── VendaView.xaml
    └── App.xaml (Configuração DI)
```

## 🎯 Boas Práticas Implementadas

- ✅ Separação clara de responsabilidades entre camadas
- ✅ Encapsulamento de entidades de domínio
- ✅ Validações no domínio (invariantes)
- ✅ DTOs para transferência de dados entre camadas
- ✅ Injeção de dependências para desacoplamento
- ✅ Tratamento centralizado de transações (Unit of Work)
- ✅ Padrão Repository para abstração de persistência
- ✅ MVVM para separação de lógica e apresentação
- ✅ Async/await para operações assíncronas
- ✅ Comentários XML para documentação de código

## 🐛 Tratamento de Erros

- Validações no domínio com exceções descritivas
- Try/catch em ViewModels com feedback ao usuário
- Rollback automático de transações em caso de erro
- MessageBox para comunicação de erros ao usuário

## 📝 Notas Importantes

- O sistema permite vendas de produtos sem estoque (estoque negativo) para controle posterior
- Produtos desativados não aparecem na busca de vendas
- Código de barras é opcional mas deve ser único quando informado
- Todas as operações de banco de dados são transacionais

## 🔮 Possíveis Melhorias Futuras

- Sistema de login e autenticação
- Relatórios em PDF
- Backup automático do banco de dados
- Impressão de cupom de venda
- Controle de caixa
- Gestão de fornecedores
- Histórico de alterações
- Logs estruturados (Serilog)
- Testes unitários e de integração

## 📄 Licença

Este projeto foi desenvolvido para fins educacionais e de demonstração.

---

**Desenvolvido seguindo os princípios de Clean Architecture, DDD e SOLID**
