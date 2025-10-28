using BancaJornal.Model.Entities;

namespace BancaJornal.Repository.Interfaces;

/// <summary>
/// Contrato para repositório de Venda.
/// Aplica ISP (Interface Segregation Principle) - interface específica para Venda.
/// </summary>
public interface IVendaRepository
{
    Task<Venda?> ObterPorIdAsync(int id);
    Task<IEnumerable<Venda>> ObterTodosAsync();
    Task<IEnumerable<Venda>> ObterVendasPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    Task<IEnumerable<Venda>> ObterVendasDoMesAsync(int mes, int ano);
    Task AdicionarAsync(Venda venda);
    Task AtualizarAsync(Venda venda);
    Task RemoverAsync(int id);
    Task<decimal> ObterTotalVendasMesAsync(int mes, int ano);
    Task<int> ContarVendasMesAsync(int mes, int ano);
}
