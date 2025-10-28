using BancaJornal.Application.DTOs;
using BancaJornal.Repository.Interfaces;

namespace BancaJornal.Application.Services;

/// <summary>
/// Serviço de aplicação para geração de dados de dashboard.
/// Agrega informações de múltiplos repositórios.
/// Aplica SRP - responsabilidade única de fornecer dados para dashboard.
/// </summary>
public class DashboardService
{
    private readonly IUnitOfWork _unitOfWork;

    public DashboardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<DashboardDto> ObterDadosDashboardAsync()
    {
        var mesAtual = DateTime.Now.Month;
        var anoAtual = DateTime.Now.Year;

        var produtosEmEstoque = await _unitOfWork.Produtos.ContarProdutosEmEstoqueAsync();
        var produtosEstoqueBaixo = await _unitOfWork.Produtos.ObterProdutosComEstoqueBaixoAsync();
        
        // FIX: Trazer dados para memória antes de agregar (SQLite não suporta Sum/Average em decimal)
        var vendasMes = await _unitOfWork.Vendas.ObterVendasDoMesAsync(mesAtual, anoAtual);
        var vendasMesList = vendasMes.ToList(); // Materializar em memória

        // Calcular agregações no cliente (LINQ to Objects ao invés de LINQ to SQL)
        var totalVendasMes = vendasMesList.Sum(v => v.ValorTotal);
        var quantidadeVendasMes = vendasMesList.Count;

        // Calcular produtos mais vendidos no mês (já em memória, sem problemas)
        var produtosMaisVendidos = vendasMesList
            .SelectMany(v => v.Itens)
            .GroupBy(i => i.NomeProduto)
            .Select(g => new ProdutoVendidoDto
            {
                NomeProduto = g.Key,
                QuantidadeVendida = g.Sum(i => i.Quantidade),
                ValorTotal = g.Sum(i => i.ValorTotal)
            })
            .OrderByDescending(p => p.QuantidadeVendida)
            .Take(10)
            .ToList();

        return new DashboardDto
        {
            ProdutosEmEstoque = produtosEmEstoque,
            ProdutosComEstoqueBaixo = produtosEstoqueBaixo.Count(),
            VendasMesCorrente = quantidadeVendasMes,
            TotalVendasMesCorrente = totalVendasMes,
            ProdutosMaisVendidos = produtosMaisVendidos
        };
    }
}
