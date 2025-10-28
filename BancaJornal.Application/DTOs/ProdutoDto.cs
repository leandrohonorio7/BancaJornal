using BancaJornal.Model.Entities;

namespace BancaJornal.Application.DTOs;

/// <summary>
/// DTO para transferência de dados de Produto entre camadas.
/// Evita expor entidades de domínio diretamente na camada de apresentação.
/// </summary>
public class ProdutoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal PrecoVenda { get; set; }
    public int QuantidadeEstoque { get; set; }
    public string? CodigoBarras { get; set; }
    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; }

    public static ProdutoDto FromEntity(Produto produto)
    {
        return new ProdutoDto
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            PrecoVenda = produto.PrecoVenda,
            QuantidadeEstoque = produto.QuantidadeEstoque,
            CodigoBarras = produto.CodigoBarras,
            DataCadastro = produto.DataCadastro,
            Ativo = produto.Ativo
        };
    }
}
