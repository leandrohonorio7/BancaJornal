using Microsoft.EntityFrameworkCore;
using BancaJornal.Model.Entities;
using BancaJornal.Repository.Data;
using BancaJornal.Repository.Interfaces;

namespace BancaJornal.Repository.Repositories;

/// <summary>
/// Implementação concreta do repositório de Venda usando Entity Framework Core.
/// Aplica DIP - implementa interface definida no domínio.
/// Aplica SRP - responsabilidade única de persistência de Venda.
/// </summary>
public class VendaRepository : IVendaRepository
{
    private readonly BancaJornalDbContext _context;

    public VendaRepository(BancaJornalDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Venda?> ObterPorIdAsync(int id)
    {
        return await _context.Vendas
            .Include(v => v.Itens)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<IEnumerable<Venda>> ObterTodosAsync()
    {
        return await _context.Vendas
            .Include(v => v.Itens)
            .AsNoTracking()
            .OrderByDescending(v => v.DataVenda)
            .ToListAsync();
    }

    public async Task<IEnumerable<Venda>> ObterVendasPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        return await _context.Vendas
            .Include(v => v.Itens)
            .AsNoTracking()
            .Where(v => v.DataVenda >= dataInicio && v.DataVenda <= dataFim)
            .OrderByDescending(v => v.DataVenda)
            .ToListAsync();
    }

    public async Task<IEnumerable<Venda>> ObterVendasDoMesAsync(int mes, int ano)
    {
        return await _context.Vendas
            .Include(v => v.Itens)
            .AsNoTracking()
            .Where(v => v.DataVenda.Month == mes && v.DataVenda.Year == ano)
            .OrderByDescending(v => v.DataVenda)
            .ToListAsync();
    }

    public async Task AdicionarAsync(Venda venda)
    {
        if (venda == null)
            throw new ArgumentNullException(nameof(venda));

        await _context.Vendas.AddAsync(venda);
    }

    public async Task AtualizarAsync(Venda venda)
    {
        if (venda == null)
            throw new ArgumentNullException(nameof(venda));

        _context.Vendas.Update(venda);
        await Task.CompletedTask;
    }

    public async Task RemoverAsync(int id)
    {
        var venda = await _context.Vendas.FindAsync(id);
        if (venda != null)
        {
            _context.Vendas.Remove(venda);
        }
    }

    public async Task<decimal> ObterTotalVendasMesAsync(int mes, int ano)
    {
        return await _context.Vendas
            .Where(v => v.DataVenda.Month == mes && v.DataVenda.Year == ano)
            .SumAsync(v => v.ValorTotal);
    }

    public async Task<int> ContarVendasMesAsync(int mes, int ano)
    {
        return await _context.Vendas
            .Where(v => v.DataVenda.Month == mes && v.DataVenda.Year == ano)
            .CountAsync();
    }
}
