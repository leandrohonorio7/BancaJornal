# Exemplos de Código - Padrões de Referência

## 📋 Índice
- [Entidades de Domínio](#entidades-de-domínio)
- [Repositórios](#repositórios)
- [Serviços de Aplicação](#serviços-de-aplicação)
- [ViewModels](#viewmodels)
- [Views XAML](#views-xaml)
- [Validações](#validações)
- [Transações](#transações)

---

## Entidades de Domínio

### Entity com Encapsulamento Completo
```csharp
namespace BancaJornal.Model.Entities;

public class Produto
{
    // Propriedades com setters privados
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public decimal PrecoVenda { get; private set; }
    public int QuantidadeEstoque { get; private set; }
    public bool Ativo { get; private set; }
    
    // Construtor protegido para EF Core
    protected Produto() 
    { 
        Nome = string.Empty;
    }
    
    // Construtor público com validações
    public Produto(string nome, decimal precoVenda, int quantidadeEstoque)
    {
        ValidarNome(nome);
        ValidarPreco(precoVenda);
        ValidarQuantidade(quantidadeEstoque);
        
        Nome = nome;
        PrecoVenda = precoVenda;
        QuantidadeEstoque = quantidadeEstoque;
        Ativo = true;
    }
    
    // Métodos públicos que mantêm invariantes
    public void Atualizar(string nome, decimal precoVenda)
    {
        ValidarNome(nome);
        ValidarPreco(precoVenda);
        Nome = nome;
        PrecoVenda = precoVenda;
    }
    
    public void AdicionarEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser positiva.", nameof(quantidade));
        QuantidadeEstoque += quantidade;
    }
    
    public void RemoverEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser positiva.", nameof(quantidade));
        QuantidadeEstoque -= quantidade;
    }
    
    public void Ativar() => Ativo = true;
    public void Desativar() => Ativo = false;
    
    // Validações privadas
    private void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório.", nameof(nome));
        if (nome.Length > 200)
            throw new ArgumentException("Nome não pode exceder 200 caracteres.", nameof(nome));
    }
    
    private void ValidarPreco(decimal preco)
    {
        if (preco < 0)
            throw new ArgumentException("Preço não pode ser negativo.", nameof(preco));
    }
    
    private void ValidarQuantidade(int quantidade)
    {
        if (quantidade < 0)
            throw new ArgumentException("Quantidade não pode ser negativa.", nameof(quantidade));
    }
}
```

### Aggregate Root (Venda)
```csharp
public class Venda
{
    private readonly List<ItemVenda> _itens = new();
    
    public int Id { get; private set; }
    public DateTime DataVenda { get; private set; }
    public decimal ValorTotal { get; private set; }
    
    // Read-only collection para preservar encapsulamento
    public IReadOnlyCollection<ItemVenda> Itens => _itens.AsReadOnly();
    
    protected Venda() { }
    
    public Venda(string? observacao = null)
    {
        DataVenda = DateTime.Now;
        ValorTotal = 0;
        Observacao = observacao;
    }
    
    // Gerencia seus agregados
    public void AdicionarItem(Produto produto, int quantidade)
    {
        if (produto == null)
            throw new ArgumentNullException(nameof(produto));
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade inválida.", nameof(quantidade));
            
        var item = new ItemVenda(produto, quantidade);
        _itens.Add(item);
        RecalcularValorTotal();
    }
    
    private void RecalcularValorTotal()
    {
        ValorTotal = _itens.Sum(i => i.ValorTotal);
    }
}
```

### Value Object (ItemVenda)
```csharp
public class ItemVenda
{
    public int Id { get; private set; }
    public int ProdutoId { get; private set; }
    public string NomeProduto { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public int Quantidade { get; private set; }
    public decimal ValorTotal { get; private set; }
    
    protected ItemVenda() 
    { 
        NomeProduto = string.Empty;
    }
    
    public ItemVenda(Produto produto, int quantidade)
    {
        if (produto == null)
            throw new ArgumentNullException(nameof(produto));
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade inválida.", nameof(quantidade));
            
        ProdutoId = produto.Id;
        NomeProduto = produto.Nome;
        PrecoUnitario = produto.PrecoVenda;
        Quantidade = quantidade;
        ValorTotal = PrecoUnitario * Quantidade;
    }
}
```

---

## Repositórios

### Interface de Repositório
```csharp
namespace BancaJornal.Repository.Interfaces;

public interface IProdutoRepository
{
    // Consultas
    Task<Produto?> ObterPorIdAsync(int id);
    Task<IEnumerable<Produto>> ObterTodosAsync();
    Task<IEnumerable<Produto>> ObterAtivosAsync();
    Task<IEnumerable<Produto>> BuscarPorNomeAsync(string nome);
    
    // Comandos
    Task AdicionarAsync(Produto produto);
    Task AtualizarAsync(Produto produto);
    Task RemoverAsync(int id);
    
    // Consultas específicas do domínio
    Task<int> ContarProdutosEmEstoqueAsync();
    Task<IEnumerable<Produto>> ObterProdutosComEstoqueBaixoAsync(int quantidadeMinima = 5);
}
```

### Implementação de Repositório
```csharp
namespace BancaJornal.Repository.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly BancaJornalDbContext _context;
    
    public ProdutoRepository(BancaJornalDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<Produto?> ObterPorIdAsync(int id)
    {
        return await _context.Produtos
            .AsNoTracking() // Read-only para performance
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    
    public async Task<IEnumerable<Produto>> ObterTodosAsync()
    {
        return await _context.Produtos
            .AsNoTracking()
            .OrderBy(p => p.Nome)
            .ToListAsync();
    }
    
    public async Task AdicionarAsync(Produto produto)
    {
        if (produto == null)
            throw new ArgumentNullException(nameof(produto));
        await _context.Produtos.AddAsync(produto);
        // NÃO chamar SaveChanges aqui - deixar para UnitOfWork
    }
    
    public async Task AtualizarAsync(Produto produto)
    {
        if (produto == null)
            throw new ArgumentNullException(nameof(produto));
        _context.Produtos.Update(produto);
        await Task.CompletedTask;
    }
}
```

### Unit of Work
```csharp
namespace BancaJornal.Repository.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly BancaJornalDbContext _context;
    private IProdutoRepository? _produtoRepository;
    private IVendaRepository? _vendaRepository;
    
    public UnitOfWork(BancaJornalDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    // Lazy initialization
    public IProdutoRepository Produtos
    {
        get
        {
            _produtoRepository ??= new ProdutoRepository(_context);
            return _produtoRepository;
        }
    }
    
    public IVendaRepository Vendas
    {
        get
        {
            _vendaRepository ??= new VendaRepository(_context);
            return _vendaRepository;
        }
    }
    
    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }
    
    public async Task RollbackAsync()
    {
        await Task.Run(() =>
        {
            foreach (var entry in _context.ChangeTracker.Entries())
            {
                entry.State = EntityState.Detached;
            }
        });
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }
}
```

---

## Serviços de Aplicação

### Service com Transações
```csharp
namespace BancaJornal.Application.Services;

public class VendaService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public VendaService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }
    
    public async Task<VendaDto> CriarVendaAsync(
        List<(int ProdutoId, int Quantidade)> itens, 
        string? observacao = null)
    {
        if (itens == null || !itens.Any())
            throw new ArgumentException("Venda sem itens.", nameof(itens));
        
        try
        {
            var venda = new Venda(observacao);
            
            foreach (var (produtoId, quantidade) in itens)
            {
                var produto = await _unitOfWork.Produtos.ObterPorIdAsync(produtoId);
                if (produto == null)
                    throw new InvalidOperationException($"Produto {produtoId} não encontrado.");
                
                venda.AdicionarItem(produto, quantidade);
                produto.RemoverEstoque(quantidade);
                await _unitOfWork.Produtos.AtualizarAsync(produto);
            }
            
            await _unitOfWork.Vendas.AdicionarAsync(venda);
            await _unitOfWork.CommitAsync(); // Transação atômica
            
            return VendaDto.FromEntity(venda);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(); // Desfaz tudo em caso de erro
            throw;
        }
    }
}
```

### Service Simples (CRUD)
```csharp
public class ProdutoService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public ProdutoService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }
    
    public async Task<ProdutoDto> CriarProdutoAsync(
        string nome, 
        string descricao, 
        decimal precoVenda, 
        int quantidadeEstoque)
    {
        var produto = new Produto(nome, descricao, precoVenda, quantidadeEstoque);
        await _unitOfWork.Produtos.AdicionarAsync(produto);
        await _unitOfWork.CommitAsync();
        return ProdutoDto.FromEntity(produto);
    }
    
    public async Task<IEnumerable<ProdutoDto>> ObterTodosAsync()
    {
        var produtos = await _unitOfWork.Produtos.ObterTodosAsync();
        return produtos.Select(ProdutoDto.FromEntity);
    }
}
```

---

## ViewModels

### ViewModel com CommunityToolkit.Mvvm
```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BancaJornal.Desktop.ViewModels;

public partial class ProdutoViewModel : ObservableObject
{
    private readonly ProdutoService _produtoService;
    
    // [ObservableProperty] gera propriedade pública automaticamente
    [ObservableProperty]
    private string _nome = string.Empty;
    
    [ObservableProperty]
    private decimal _precoVenda;
    
    [ObservableProperty]
    private ObservableCollection<ProdutoDto> _produtos = new();
    
    public ProdutoViewModel(ProdutoService produtoService)
    {
        _produtoService = produtoService;
    }
    
    // [RelayCommand] gera comando automaticamente
    [RelayCommand]
    private async Task CarregarProdutos()
    {
        try
        {
            var lista = await _produtoService.ObterTodosAsync();
            Produtos.Clear();
            foreach (var produto in lista)
            {
                Produtos.Add(produto);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    [RelayCommand]
    private async Task SalvarProduto()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Nome))
            {
                MessageBox.Show("Nome é obrigatório.", "Validação", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            await _produtoService.CriarProdutoAsync(Nome, "", PrecoVenda, 0);
            MessageBox.Show("Produto salvo!", "Sucesso", 
                MessageBoxButton.OK, MessageBoxImage.Information);
            
            Nome = string.Empty;
            PrecoVenda = 0;
            await CarregarProdutos();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
```

---

## Views XAML

### DataGrid com Binding
```xml
<DataGrid ItemsSource="{Binding Produtos}" 
          SelectedItem="{Binding ProdutoSelecionado}"
          AutoGenerateColumns="False">
    <DataGrid.Columns>
        <DataGridTextColumn Header="ID" Binding="{Binding Id}" Width="50"/>
        <DataGridTextColumn Header="Nome" Binding="{Binding Nome}" Width="*"/>
        <DataGridTextColumn Header="Preço" 
                            Binding="{Binding PrecoVenda, StringFormat='R$ {0:N2}'}" 
                            Width="100"/>
        <DataGridCheckBoxColumn Header="Ativo" 
                                Binding="{Binding Ativo}" 
                                Width="60" 
                                IsReadOnly="True"/>
    </DataGrid.Columns>
</DataGrid>
```

### Formulário com Validação
```xml
<StackPanel>
    <TextBlock Text="Nome *" FontSize="12"/>
    <TextBox Text="{Binding Nome, UpdateSourceTrigger=PropertyChanged}"/>
    
    <TextBlock Text="Preço *" FontSize="12"/>
    <TextBox Text="{Binding PrecoVenda, UpdateSourceTrigger=PropertyChanged}"/>
    
    <Button Content="Salvar" Command="{Binding SalvarCommand}"/>
</StackPanel>
```

### Command com Parâmetro
```xml
<DataGrid ItemsSource="{Binding Itens}">
    <DataGrid.Columns>
        <DataGridTemplateColumn Width="60">
            <DataGridTemplateColumn.CellTemplate>
                <DataTemplate>
                    <Button Content="Remover"
                            Command="{Binding DataContext.RemoverItemCommand, 
                                     RelativeSource={RelativeSource AncestorType=DataGrid}}"
                            CommandParameter="{Binding}"/>
                </DataTemplate>
            </DataGridTemplateColumn.CellTemplate>
        </DataGridTemplateColumn>
    </DataGrid.Columns>
</DataGrid>
```

---

## Validações

### Validação no Domínio
```csharp
private void ValidarNome(string nome)
{
    if (string.IsNullOrWhiteSpace(nome))
        throw new ArgumentException("Nome é obrigatório.", nameof(nome));
    
    if (nome.Length < 3)
        throw new ArgumentException("Nome deve ter pelo menos 3 caracteres.", nameof(nome));
    
    if (nome.Length > 200)
        throw new ArgumentException("Nome não pode exceder 200 caracteres.", nameof(nome));
}
```

### Validação no ViewModel
```csharp
[RelayCommand]
private async Task Salvar()
{
    // Validação antes de chamar serviço
    if (string.IsNullOrWhiteSpace(Nome))
    {
        MessageBox.Show("Nome é obrigatório.", "Validação", 
            MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
    }
    
    if (PrecoVenda <= 0)
    {
        MessageBox.Show("Preço deve ser maior que zero.", "Validação", 
            MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
    }
    
    try
    {
        await _service.CriarAsync(Nome, PrecoVenda);
        MessageBox.Show("Sucesso!");
    }
    catch (ArgumentException ex)
    {
        // Exceção do domínio
        MessageBox.Show(ex.Message, "Validação", 
            MessageBoxButton.OK, MessageBoxImage.Warning);
    }
    catch (Exception ex)
    {
        // Outros erros
        MessageBox.Show($"Erro inesperado: {ex.Message}", "Erro", 
            MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
```

---

## Transações

### Padrão de Transação Completa
```csharp
public async Task<VendaDto> ProcessarVendaAsync(/* params */)
{
    try
    {
        // 1. Criar entidade
        var venda = new Venda();
        
        // 2. Operações no domínio
        foreach (var item in itens)
        {
            var produto = await _unitOfWork.Produtos.ObterPorIdAsync(item.ProdutoId);
            venda.AdicionarItem(produto, item.Quantidade);
            produto.RemoverEstoque(item.Quantidade);
            await _unitOfWork.Produtos.AtualizarAsync(produto);
        }
        
        // 3. Persistir
        await _unitOfWork.Vendas.AdicionarAsync(venda);
        
        // 4. Commit (tudo ou nada)
        await _unitOfWork.CommitAsync();
        
        return VendaDto.FromEntity(venda);
    }
    catch (Exception)
    {
        // 5. Rollback em caso de erro
        await _unitOfWork.RollbackAsync();
        throw;
    }
}
```

---

**Nota:** Todos os exemplos seguem os princípios SOLID e a arquitetura em camadas do projeto.
