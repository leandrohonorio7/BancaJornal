using Microsoft.EntityFrameworkCore;
using BancaJornal.Model.Entities;
using BancaJornal.Repository.Data;
using BancaJornal.Repository.Interfaces;

namespace BancaJornal.Repository.Repositories;

/// <summary>
/// Implementação concreta do repositório de Produto usando Entity Framework Core.
/// Aplica DIP - implementa interface definida no domínio.
/// Aplica SRP - responsabilidade única de persistência de Produto.
/// </summary>
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
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Produto>> ObterTodosAsync()
    {
        return await _context.Produtos
            .AsNoTracking()
            .OrderBy(p => p.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Produto>> ObterAtivosAsync()
    {
        return await _context.Produtos
            .AsNoTracking()
            .Where(p => p.Ativo)
            .OrderBy(p => p.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Produto>> BuscarPorNomeAsync(string nome)
    {
        return await _context.Produtos
            .AsNoTracking()
            .Where(p => p.Nome.Contains(nome) || p.Descricao.Contains(nome))
            .OrderBy(p => p.Nome)
            .ToListAsync();
    }

    public async Task<Produto?> ObterPorCodigoBarrasAsync(string codigoBarras)
    {
        return await _context.Produtos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.CodigoBarras == codigoBarras);
    }

    public async Task<IEnumerable<Produto>> ObterProdutosComEstoqueBaixoAsync(int quantidadeMinima = 5)
    {
        return await _context.Produtos
            .AsNoTracking()
            .Where(p => p.Ativo && p.QuantidadeEstoque <= quantidadeMinima)
            .OrderBy(p => p.QuantidadeEstoque)
            .ToListAsync();
    }

    public async Task AdicionarAsync(Produto produto)
    {
        if (produto == null)
            throw new ArgumentNullException(nameof(produto));

        await _context.Produtos.AddAsync(produto);
    }

    public async Task AtualizarAsync(Produto produto)
    {
        var tracked = await _context.Produtos.FindAsync(produto.Id);
    if (tracked != null)
    {
        // Atualize apenas as propriedades necessárias
        _context.Entry(tracked).CurrentValues.SetValues(produto);
    }
    else
    {
        // Se não estiver sendo rastreado, anexe e marque como modificado
        _context.Produtos.Attach(produto);
        _context.Entry(produto).State = EntityState.Modified;
    }
    }

    public async Task RemoverAsync(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto != null)
        {
            _context.Produtos.Remove(produto);
        }
    }

    public async Task<int> ContarProdutosEmEstoqueAsync()
    {
        return await _context.Produtos
            .Where(p => p.Ativo && p.QuantidadeEstoque > 0)
            .CountAsync();
    }
}
