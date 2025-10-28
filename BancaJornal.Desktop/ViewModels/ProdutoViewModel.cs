using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BancaJornal.Application.DTOs;
using BancaJornal.Application.Services;

namespace BancaJornal.Desktop.ViewModels;

/// <summary>
/// ViewModel para CRUD de produtos.
/// Gerencia operações de criação, leitura, atualização e exclusão.
/// Aplica MVVM e SRP.
/// </summary>
public partial class ProdutoViewModel : ObservableObject
{
    private readonly ProdutoService _produtoService;

    [ObservableProperty]
    private ObservableCollection<ProdutoDto> _produtos = new();

    [ObservableProperty]
    private ProdutoDto? _produtoSelecionado;

    [ObservableProperty]
    private string _nome = string.Empty;

    [ObservableProperty]
    private string _descricao = string.Empty;

    [ObservableProperty]
    private decimal _precoVenda;

    [ObservableProperty]
    private int _quantidadeEstoque;

    [ObservableProperty]
    private string _codigoBarras = string.Empty;

    [ObservableProperty]
    private string _filtroNome = string.Empty;

    [ObservableProperty]
    private bool _modoEdicao;

    [ObservableProperty]
    private int _produtoIdEdicao;

    public ProdutoViewModel(ProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    [RelayCommand]
    private async Task CarregarProdutos()
    {
        try
        {
            var produtos = await _produtoService.ObterTodosAsync();
            Produtos.Clear();
            foreach (var produto in produtos)
            {
                Produtos.Add(produto);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao carregar produtos: {ex.Message}", "Erro", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task Buscar()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(FiltroNome))
            {
                await CarregarProdutos();
                return;
            }

            var produtos = await _produtoService.BuscarPorNomeAsync(FiltroNome);
            Produtos.Clear();
            foreach (var produto in produtos)
            {
                Produtos.Add(produto);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao buscar produtos: {ex.Message}", "Erro", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void NovoProduto()
    {
        LimparFormulario();
        ModoEdicao = false;
    }

    [RelayCommand]
    private void EditarProduto()
    {
        if (ProdutoSelecionado == null)
        {
            MessageBox.Show("Selecione um produto para editar.", "Aviso", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        ProdutoIdEdicao = ProdutoSelecionado.Id;
        Nome = ProdutoSelecionado.Nome;
        Descricao = ProdutoSelecionado.Descricao;
        PrecoVenda = ProdutoSelecionado.PrecoVenda;
        QuantidadeEstoque = ProdutoSelecionado.QuantidadeEstoque;
        CodigoBarras = ProdutoSelecionado.CodigoBarras ?? string.Empty;
        ModoEdicao = true;
    }

    [RelayCommand]
    private async Task SalvarProduto()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Nome))
            {
                MessageBox.Show("Nome do produto é obrigatório.", "Validação", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (PrecoVenda <= 0)
            {
                MessageBox.Show("Preço deve ser maior que zero.", "Validação", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ModoEdicao)
            {
                await _produtoService.AtualizarProdutoAsync(
                    ProdutoIdEdicao, Nome, Descricao, PrecoVenda, 
                    string.IsNullOrWhiteSpace(CodigoBarras) ? null : CodigoBarras);
                
                MessageBox.Show("Produto atualizado com sucesso!", "Sucesso", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                await _produtoService.CriarProdutoAsync(
                    Nome, Descricao, PrecoVenda, QuantidadeEstoque, 
                    string.IsNullOrWhiteSpace(CodigoBarras) ? null : CodigoBarras);
                
                MessageBox.Show("Produto criado com sucesso!", "Sucesso", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }

            LimparFormulario();
            await CarregarProdutos();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao salvar produto: {ex.Message}", "Erro", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task DesativarProduto()
    {
        if (ProdutoSelecionado == null)
        {
            MessageBox.Show("Selecione um produto para desativar.", "Aviso", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var resultado = MessageBox.Show(
            $"Deseja desativar o produto '{ProdutoSelecionado.Nome}'?", 
            "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (resultado == MessageBoxResult.Yes)
        {
            try
            {
                await _produtoService.DesativarProdutoAsync(ProdutoSelecionado.Id);
                MessageBox.Show("Produto desativado com sucesso!", "Sucesso", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                await CarregarProdutos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao desativar produto: {ex.Message}", "Erro", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void LimparFormulario()
    {
        Nome = string.Empty;
        Descricao = string.Empty;
        PrecoVenda = 0;
        QuantidadeEstoque = 0;
        CodigoBarras = string.Empty;
        ModoEdicao = false;
        ProdutoIdEdicao = 0;
    }
}
