using BancaJornal.Application.DTOs;
using BancaJornal.Model.Entities;
using BancaJornal.Repository.Interfaces;

namespace BancaJornal.Application.Services;

/// <summary>
/// Serviço de aplicação para gerenciamento de produtos.
/// Coordena operações entre camadas Model e Repository.
/// Aplica SRP - responsabilidade única de orquestrar operações de produtos.
/// Aplica DIP - depende de abstrações (interfaces), não de implementações concretas.
/// </summary>
public class ProdutoService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProdutoService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ProdutoDto?> ObterPorIdAsync(int id)
    {
        var produto = await _unitOfWork.Produtos.ObterPorIdAsync(id);
        return produto != null ? ProdutoDto.FromEntity(produto) : null;
    }

    public async Task<IEnumerable<ProdutoDto>> ObterTodosAsync()
    {
        var produtos = await _unitOfWork.Produtos.ObterTodosAsync();
        return produtos.Select(ProdutoDto.FromEntity);
    }

    public async Task<IEnumerable<ProdutoDto>> ObterAtivosAsync()
    {
        var produtos = await _unitOfWork.Produtos.ObterAtivosAsync();
        return produtos.Select(ProdutoDto.FromEntity);
    }

    public async Task<IEnumerable<ProdutoDto>> BuscarPorNomeAsync(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Enumerable.Empty<ProdutoDto>();

        var produtos = await _unitOfWork.Produtos.BuscarPorNomeAsync(nome);
        return produtos.Select(ProdutoDto.FromEntity);
    }

    public async Task<ProdutoDto> CriarProdutoAsync(string nome, string descricao, decimal precoVenda, int quantidadeEstoque, string? codigoBarras = null)
    {
        // Validação de código de barras único
        if (!string.IsNullOrWhiteSpace(codigoBarras))
        {
            var produtoExistente = await _unitOfWork.Produtos.ObterPorCodigoBarrasAsync(codigoBarras);
            if (produtoExistente != null)
                throw new InvalidOperationException("Já existe um produto com este código de barras.");
        }

        var produto = new Produto(nome, descricao, precoVenda, quantidadeEstoque, codigoBarras);
        await _unitOfWork.Produtos.AdicionarAsync(produto);
        await _unitOfWork.CommitAsync();

        return ProdutoDto.FromEntity(produto);
    }

    public async Task<ProdutoDto> AtualizarProdutoAsync(int id, string nome, string descricao, decimal precoVenda, string? codigoBarras = null)
    {
        var produto = await _unitOfWork.Produtos.ObterPorIdAsync(id);
        if (produto == null)
            throw new InvalidOperationException("Produto não encontrado.");

        // Validação de código de barras único
        if (!string.IsNullOrWhiteSpace(codigoBarras) && codigoBarras != produto.CodigoBarras)
        {
            var produtoExistente = await _unitOfWork.Produtos.ObterPorCodigoBarrasAsync(codigoBarras);
            if (produtoExistente != null && produtoExistente.Id != id)
                throw new InvalidOperationException("Já existe um produto com este código de barras.");
        }

        produto.Atualizar(nome, descricao, precoVenda, codigoBarras);
        await _unitOfWork.Produtos.AtualizarAsync(produto);
        await _unitOfWork.CommitAsync();

        return ProdutoDto.FromEntity(produto);
    }

    public async Task AdicionarEstoqueAsync(int produtoId, int quantidade)
    {
        var produto = await _unitOfWork.Produtos.ObterPorIdAsync(produtoId);
        if (produto == null)
            throw new InvalidOperationException("Produto não encontrado.");

        produto.AdicionarEstoque(quantidade);
        await _unitOfWork.Produtos.AtualizarAsync(produto);
        await _unitOfWork.CommitAsync();
    }

    public async Task AtivarProdutoAsync(int id)
    {
        var produto = await _unitOfWork.Produtos.ObterPorIdAsync(id);
        if (produto == null)
            throw new InvalidOperationException("Produto não encontrado.");

        produto.Ativar();
        await _unitOfWork.Produtos.AtualizarAsync(produto);
        await _unitOfWork.CommitAsync();
    }

    public async Task DesativarProdutoAsync(int id)
    {
        var produto = await _unitOfWork.Produtos.ObterPorIdAsync(id);
        if (produto == null)
            throw new InvalidOperationException("Produto não encontrado.");

        produto.Desativar();
        await _unitOfWork.Produtos.AtualizarAsync(produto);
        await _unitOfWork.CommitAsync();
    }

    public async Task RemoverProdutoAsync(int id)
    {
        // Validar se o produto não está em vendas antes de remover
        await _unitOfWork.Produtos.RemoverAsync(id);
        await _unitOfWork.CommitAsync();
    }
}
