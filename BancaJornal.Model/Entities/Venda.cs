namespace BancaJornal.Model.Entities;

/// <summary>
/// Entidade de domínio que representa uma venda realizada na banca.
/// Implementa padrão Aggregate Root do DDD.
/// </summary>
public class Venda
{
    private readonly List<ItemVenda> _itens = new();

    public int Id { get; private set; }
    public DateTime DataVenda { get; private set; }
    public decimal ValorTotal { get; private set; }
    public string? Observacao { get; private set; }
    
    // Navigation property (read-only para preservar encapsulamento)
    public IReadOnlyCollection<ItemVenda> Itens => _itens.AsReadOnly();

    // Construtor protegido para EF Core
    protected Venda() { }

    public Venda(string? observacao = null)
    {
        DataVenda = DateTime.Now;
        ValorTotal = 0;
        Observacao = observacao;
    }

    /// <summary>
    /// Adiciona um item à venda. Calcula automaticamente o valor total.
    /// Aplica SRP - venda gerencia seus próprios itens.
    /// </summary>
    public void AdicionarItem(Produto produto, int quantidade)
    {
        if (produto == null)
            throw new ArgumentNullException(nameof(produto));

        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(quantidade));

        var item = new ItemVenda(produto, quantidade);
        _itens.Add(item);
        
        RecalcularValorTotal();
    }

    /// <summary>
    /// Remove um item da venda e recalcula o total.
    /// </summary>
    public void RemoverItem(ItemVenda item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        _itens.Remove(item);
        RecalcularValorTotal();
    }

    public void AtualizarObservacao(string? observacao)
    {
        Observacao = observacao;
    }

    private void RecalcularValorTotal()
    {
        ValorTotal = _itens.Sum(i => i.ValorTotal);
    }

    /// <summary>
    /// Valida se a venda pode ser finalizada.
    /// </summary>
    public bool PodeSerFinalizada()
    {
        return _itens.Any() && ValorTotal > 0;
    }
}
