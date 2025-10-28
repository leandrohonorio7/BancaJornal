namespace BancaJornal.Repository.Interfaces;

/// <summary>
/// Unit of Work pattern para gerenciar transações e coordenar múltiplos repositórios.
/// Garante consistência transacional entre operações em diferentes entidades.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IProdutoRepository Produtos { get; }
    IVendaRepository Vendas { get; }
    
    Task<int> CommitAsync();
    Task RollbackAsync();
}
