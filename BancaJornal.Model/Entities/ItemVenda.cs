namespace BancaJornal.Model.Entities;

/// <summary>
/// Value Object que representa um item dentro de uma venda.
/// Implementa conceitos de DDD - imutabilidade e ausência de identidade própria.
/// </summary>
public class ItemVenda
{
    public int Id { get; private set; }
    public int VendaId { get; private set; }
    public int ProdutoId { get; private set; }
    public string NomeProduto { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public int Quantidade { get; private set; }
    public decimal ValorTotal { get; private set; }

    // Navigation properties
    public Venda? Venda { get; private set; }
    public Produto? Produto { get; private set; }

    // Construtor protegido para EF Core
    protected ItemVenda() 
    { 
        NomeProduto = string.Empty;
    }

    public ItemVenda(Produto produto, int quantidade)
    {
        if (produto == null)
            throw new ArgumentNullException(nameof(produto));

        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(quantidade));

        ProdutoId = produto.Id;
        NomeProduto = produto.Nome;
        PrecoUnitario = produto.PrecoVenda;
        Quantidade = quantidade;
        ValorTotal = PrecoUnitario * Quantidade;
    }

    /// <summary>
    /// Atualiza a quantidade do item e recalcula o valor total.
    /// </summary>
    public void AtualizarQuantidade(int novaQuantidade)
    {
        if (novaQuantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(novaQuantidade));

        Quantidade = novaQuantidade;
        ValorTotal = PrecoUnitario * Quantidade;
    }
}
