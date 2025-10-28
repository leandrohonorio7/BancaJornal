# AI Coding Agent Instructions - Banca de Jornal

## Project Overview

Sistema desktop de gerenciamento de banca de jornal desenvolvido em **C# com WPF**, seguindo arquitetura em camadas (MODEL, REPOSITORY, APPLICATION, DESKTOP) e princípios **SOLID**.

**Stack tecnológico:**
- .NET 8.0 / C# 12
- WPF (Windows Presentation Foundation)
- Entity Framework Core 8.0 + SQLite
- CommunityToolkit.Mvvm (MVVM)
- Microsoft.Extensions.DependencyInjection

## Arquitetura em Camadas

Este projeto segue rigorosamente uma arquitetura em 4 camadas. **SEMPRE respeite as responsabilidades de cada camada:**

### 1. MODEL (BancaJornal.Model)
- **Responsabilidade única:** Entidades de domínio e regras de negócio puras
- **NÃO deve:** Referenciar outras camadas, frameworks de persistência ou UI
- **DEVE:** Aplicar DDD (Value Objects, Entities, Aggregates), encapsular estado, validar invariantes
- **Exemplos:** `Produto`, `Venda`, `ItemVenda`
- **Princípios SOLID:** SRP, OCP
- **Padrão de validação:** Validações no construtor e métodos públicos usando `ArgumentException`

### 2. REPOSITORY (BancaJornal.Repository)
- **Responsabilidade única:** Acesso e persistência de dados
- **DEVE:** Definir interfaces (`IProdutoRepository`, `IVendaRepository`), implementar com EF Core, usar Unit of Work
- **NÃO deve:** Conter lógica de negócio ou UI
- **Princípios SOLID:** DIP (dependências de abstrações), LSP, ISP
- **DbContext:** `BancaJornalDbContext` com configurações Fluent API
- **Padrão:** Repository + Unit of Work para transações

### 3. APPLICATION (BancaJornal.Application)
- **Responsabilidade única:** Orquestração de casos de uso
- **DEVE:** Coordenar MODEL e REPOSITORY, usar DTOs para transferência, aplicar injeção de dependências
- **NÃO deve:** Conter lógica de apresentação ou acesso direto a dados
- **Serviços:** `ProdutoService`, `VendaService`, `DashboardService`
- **Princípios SOLID:** DIP, ISP, SRP
- **Padrão de erro:** Try/catch em operações críticas com rollback via UnitOfWork

### 4. DESKTOP (BancaJornal.Desktop)
- **Responsabilidade única:** Interface com usuário
- **DEVE:** Usar padrão MVVM, separar Views (XAML) de ViewModels (lógica), comunicar-se APENAS com APPLICATION
- **NÃO deve:** Acessar repositórios diretamente ou conter regras de negócio
- **ViewModels:** `DashboardViewModel`, `ProdutoViewModel`, `VendaViewModel`
- **Padrão:** MVVM com CommunityToolkit.Mvvm (`ObservableObject`, `RelayCommand`)
- **DI:** Configurado em `App.xaml.cs` usando `ServiceCollection`

## Fluxo de Dados (CRITICAL)

```
User Input → View (XAML) → ViewModel → Service (APPLICATION) → Entity (MODEL) → Repository → Database
```

**NUNCA pule camadas.** ViewModel NUNCA acessa Repository diretamente.

## Development Workflow

### Build & Run
```powershell
# Restaurar dependências
dotnet restore BancaJornal.sln

# Compilar
dotnet build BancaJornal.sln

# Executar
dotnet run --project BancaJornal.Desktop
```

### Banco de Dados
- SQLite criado automaticamente na primeira execução (`bancajornal.db`)
- Migrations: EF Core (executar `Add-Migration` e `Update-Database` se necessário)
- Seed data: Implementar em `OnModelCreating` se necessário

## Coding Conventions

### Naming
- **Entities:** `PascalCase` (ex: `Produto`, `Venda`)
- **Interfaces:** `IPascalCase` (ex: `IProdutoRepository`)
- **DTOs:** `PascalCaseDto` (ex: `ProdutoDto`)
- **Services:** `PascalCaseService` (ex: `ProdutoService`)
- **ViewModels:** `PascalCaseViewModel` (ex: `DashboardViewModel`)
- **Private fields:** `_camelCase` (ex: `_unitOfWork`)

### File Organization
- Um arquivo por classe
- Namespaces devem corresponder à estrutura de pastas
- Colocar interfaces em pasta `Interfaces/`
- Agrupar DTOs em pasta `DTOs/`

### Error Handling
- **MODEL:** Exceções descritivas (`ArgumentException`, `InvalidOperationException`)
- **APPLICATION:** Try/catch com rollback via `IUnitOfWork.RollbackAsync()`
- **DESKTOP:** Try/catch em Commands com `MessageBox.Show()` para feedback

### Dependency Injection
- Registrar serviços em `App.xaml.cs` → `ConfigureServices()`
- Usar `AddScoped` para repositórios e serviços
- Usar `AddTransient` para ViewModels e Views
- Injetar via construtor (constructor injection)

## SOLID Principles (MANDATORY)

Ao adicionar ou modificar código:
1. **SRP:** Cada classe tem UMA responsabilidade
2. **OCP:** Estender via herança/interfaces, não modificar código existente
3. **LSP:** Implementações devem ser substituíveis
4. **ISP:** Interfaces específicas, não genéricas
5. **DIP:** Sempre depender de abstrações (interfaces), não de implementações

## Key Files & Entry Points

- **Startup:** `BancaJornal.Desktop/App.xaml.cs` (DI setup + banco)
- **Main Window:** `BancaJornal.Desktop/Views/MainWindow.xaml`
- **DbContext:** `BancaJornal.Repository/Data/BancaJornalDbContext.cs`
- **UnitOfWork:** `BancaJornal.Repository/Repositories/UnitOfWork.cs`

## Important Patterns

### Entity Validation (MODEL)
```csharp
public void Atualizar(string nome, decimal preco)
{
    ValidarNome(nome);
    ValidarPreco(preco);
    Nome = nome;
    PrecoVenda = preco;
}
```

### Service Transaction (APPLICATION)
```csharp
try {
    // Operações
    await _unitOfWork.CommitAsync();
} catch {
    await _unitOfWork.RollbackAsync();
    throw;
}
```

### ViewModel Command (DESKTOP)
```csharp
[RelayCommand]
private async Task SalvarProduto()
{
    try {
        await _produtoService.CriarProdutoAsync(...);
        MessageBox.Show("Sucesso!");
    } catch (Exception ex) {
        MessageBox.Show($"Erro: {ex.Message}");
    }
}
```

## Common Tasks

### Adding New Feature
1. Create entity in MODEL with validations
2. Create repository interface in REPOSITORY/Interfaces
3. Implement repository in REPOSITORY/Repositories
4. Add to IUnitOfWork and UnitOfWork
5. Create DTO in APPLICATION/DTOs
6. Create service in APPLICATION/Services
7. Create ViewModel in DESKTOP/ViewModels
8. Create View in DESKTOP/Views
9. Register in DI container (App.xaml.cs)

### Running Migrations
```powershell
cd BancaJornal.Repository
dotnet ef migrations add MigrationName --startup-project ../BancaJornal.Desktop
dotnet ef database update --startup-project ../BancaJornal.Desktop
```

## Project Files Reference

### Critical Files
- `App.xaml.cs`: DI container setup, application startup
- `BancaJornalDbContext.cs`: EF Core configuration
- `UnitOfWork.cs`: Transaction management
- `MainWindow.xaml`: Application shell and navigation

### Documentation
- `README.md`: Complete project documentation
- `ARCHITECTURE.md`: Architecture diagrams and patterns
- `QUICK_START.md`: Development quick start guide
- `CODE_EXAMPLES.md`: Code pattern examples
- `PROJECT_SUMMARY.md`: Project status and setup

## Anti-Patterns to Avoid

❌ **DO NOT:**
- Access Repository directly from ViewModel
- Put business logic in ViewModel or View
- Create entities with `new` in Services (use factory methods)
- Skip Unit of Work for transactions
- Use magic strings (use constants or enums)
- Expose setters in entities without validation
- Put UI code in MODEL or REPOSITORY
- Create circular dependencies between layers

✅ **DO:**
- Always use interfaces for dependencies
- Validate in entity constructors and methods
- Use async/await for data operations
- Wrap multi-operation transactions in try/catch with rollback
- Use DTOs to transfer data between layers
- Follow naming conventions strictly
- Document complex business rules
- Use [ObservableProperty] and [RelayCommand] attributes

## References

- Clean Architecture (Robert C. Martin)
- Domain-Driven Design (Eric Evans)
- SOLID Principles
- MVVM Pattern
- Repository + Unit of Work Pattern

---

**CRITICAL:** Sempre consulte `.github/chatmodes/banca-jornal-chat-mode.chatmode.md` para critérios específicos de desenvolvimento em camadas.

**Quick Reference:** See `QUICK_START.md` for step-by-step guides on adding features.