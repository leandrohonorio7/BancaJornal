namespace BancaJornal.Model.Entities;

/// <summary>
/// Entidade de domínio que representa um produto da banca de jornal.
/// Segue princípios DDD: encapsulamento, invariantes e independência de infraestrutura.
/// </summary>
public class Produto
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public string Descricao { get; private set; }
    public decimal PrecoVenda { get; private set; }
    public int QuantidadeEstoque { get; private set; }
    public string? CodigoBarras { get; private set; }
    public DateTime DataCadastro { get; private set; }
    public bool Ativo { get; private set; }

    // Construtor protegido para EF Core
    protected Produto() 
    { 
        Nome = string.Empty;
        Descricao = string.Empty;
    }

    public Produto(string nome, string descricao, decimal precoVenda, int quantidadeEstoque, string? codigoBarras = null)
    {
        ValidarNome(nome);
        ValidarPreco(precoVenda);
        ValidarQuantidade(quantidadeEstoque);

        Nome = nome;
        Descricao = descricao;
        PrecoVenda = precoVenda;
        QuantidadeEstoque = quantidadeEstoque;
        CodigoBarras = codigoBarras;
        DataCadastro = DateTime.Now;
        Ativo = true;
    }

    /// <summary>
    /// Atualiza os dados do produto. Aplica SRP - responsabilidade única de manter dados válidos.
    /// </summary>
    public void Atualizar(string nome, string descricao, decimal precoVenda, string? codigoBarras = null)
    {
        ValidarNome(nome);
        ValidarPreco(precoVenda);

        Nome = nome;
        Descricao = descricao;
        PrecoVenda = precoVenda;
        CodigoBarras = codigoBarras;
    }

    /// <summary>
    /// Adiciona quantidade ao estoque do produto.
    /// </summary>
    public void AdicionarEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(quantidade));

        QuantidadeEstoque += quantidade;
    }

    /// <summary>
    /// Remove quantidade do estoque. Permite estoque negativo para controle de produtos vendidos sem estoque.
    /// </summary>
    public void RemoverEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(quantidade));

        QuantidadeEstoque -= quantidade;
    }

    public void Ativar() => Ativo = true;
    public void Desativar() => Ativo = false;

    private void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do produto não pode ser vazio.", nameof(nome));
        
        if (nome.Length > 200)
            throw new ArgumentException("Nome do produto não pode exceder 200 caracteres.", nameof(nome));
    }

    private void ValidarPreco(decimal preco)
    {
        if (preco < 0)
            throw new ArgumentException("Preço não pode ser negativo.", nameof(preco));
    }

    private void ValidarQuantidade(int quantidade)
    {
        // Permitimos quantidade inicial negativa para casos especiais
        if (quantidade < 0)
            throw new ArgumentException("Quantidade inicial não pode ser negativa.", nameof(quantidade));
    }
}
