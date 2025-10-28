using BancaJornal.Repository.Data;
using BancaJornal.Repository.Interfaces;

namespace BancaJornal.Repository.Repositories;

/// <summary>
/// Implementação do padrão Unit of Work para coordenar transações entre repositórios.
/// Garante atomicidade e consistência das operações no banco de dados.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly BancaJornalDbContext _context;
    private IProdutoRepository? _produtoRepository;
    private IVendaRepository? _vendaRepository;
    private bool _disposed;

    public UnitOfWork(BancaJornalDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

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
                entry.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }
        });
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
