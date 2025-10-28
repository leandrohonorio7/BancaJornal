using BancaJornal.Model.Entities;

namespace BancaJornal.Repository.Interfaces;

/// <summary>
/// Contrato para repositório de Produto.
/// Aplica ISP (Interface Segregation Principle) - interface específica para Produto.
/// Aplica DIP (Dependency Inversion Principle) - dependência de abstração, não de implementação.
/// </summary>
public interface IProdutoRepository
{
    Task<Produto?> ObterPorIdAsync(int id);
    Task<IEnumerable<Produto>> ObterTodosAsync();
    Task<IEnumerable<Produto>> ObterAtivosAsync();
    Task<IEnumerable<Produto>> BuscarPorNomeAsync(string nome);
    Task<Produto?> ObterPorCodigoBarrasAsync(string codigoBarras);
    Task<IEnumerable<Produto>> ObterProdutosComEstoqueBaixoAsync(int quantidadeMinima = 5);
    Task AdicionarAsync(Produto produto);
    Task AtualizarAsync(Produto produto);
    Task RemoverAsync(int id);
    Task<int> ContarProdutosEmEstoqueAsync();
}
