namespace BancaJornal.Application.DTOs;

/// <summary>
/// DTO para dados do dashboard.
/// </summary>
public class DashboardDto
{
    public int ProdutosEmEstoque { get; set; }
    public int ProdutosComEstoqueBaixo { get; set; }
    public int VendasMesCorrente { get; set; }
    public decimal TotalVendasMesCorrente { get; set; }
    public List<ProdutoVendidoDto> ProdutosMaisVendidos { get; set; } = new();
}

public class ProdutoVendidoDto
{
    public string NomeProduto { get; set; } = string.Empty;
    public int QuantidadeVendida { get; set; }
    public decimal ValorTotal { get; set; }
}
