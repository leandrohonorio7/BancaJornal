using BancaJornal.Model.Entities;

namespace BancaJornal.Application.DTOs;

/// <summary>
/// DTO para transferência de dados de Venda.
/// </summary>
public class VendaDto
{
    public int Id { get; set; }
    public DateTime DataVenda { get; set; }
    public decimal ValorTotal { get; set; }
    public string? Observacao { get; set; }
    public List<ItemVendaDto> Itens { get; set; } = new();

    public static VendaDto FromEntity(Venda venda)
    {
        return new VendaDto
        {
            Id = venda.Id,
            DataVenda = venda.DataVenda,
            ValorTotal = venda.ValorTotal,
            Observacao = venda.Observacao,
            Itens = venda.Itens.Select(ItemVendaDto.FromEntity).ToList()
        };
    }
}

public class ItemVendaDto
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public string NomeProduto { get; set; } = string.Empty;
    public decimal PrecoUnitario { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorTotal { get; set; }

    public static ItemVendaDto FromEntity(ItemVenda item)
    {
        return new ItemVendaDto
        {
            Id = item.Id,
            ProdutoId = item.ProdutoId,
            NomeProduto = item.NomeProduto,
            PrecoUnitario = item.PrecoUnitario,
            Quantidade = item.Quantidade,
            ValorTotal = item.ValorTotal
        };
    }
}
