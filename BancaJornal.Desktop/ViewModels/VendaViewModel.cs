using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BancaJornal.Application.DTOs;
using BancaJornal.Application.Services;

namespace BancaJornal.Desktop.ViewModels;

/// <summary>
/// ViewModel para registro de vendas.
/// Permite busca rápida de produtos e adição de itens à venda.
/// Aplica MVVM e SRP.
/// </summary>
public partial class VendaViewModel : ObservableObject
{
    private readonly VendaService _vendaService;
    private readonly ProdutoService _produtoService;

    [ObservableProperty]
    private ObservableCollection<ProdutoDto> _produtosDisponiveis = new();

    [ObservableProperty]
    private ObservableCollection<ItemVendaTemp> _itensVenda = new();

    [ObservableProperty]
    private string _buscaProduto = string.Empty;

    [ObservableProperty]
    private ProdutoDto? _produtoSelecionado;

    [ObservableProperty]
    private int _quantidadeItem = 1;

    [ObservableProperty]
    private decimal _valorTotal;

    [ObservableProperty]
    private string _observacao = string.Empty;

    public VendaViewModel(VendaService vendaService, ProdutoService produtoService)
    {
        _vendaService = vendaService;
        _produtoService = produtoService;
    }

    [RelayCommand]
    private async Task BuscarProdutos()
    {
        try
        {
            IEnumerable<ProdutoDto> produtos;

            if (string.IsNullOrWhiteSpace(BuscaProduto))
            {
                produtos = await _produtoService.ObterAtivosAsync();
            }
            else
            {
                produtos = await _produtoService.BuscarPorNomeAsync(BuscaProduto);
            }

            ProdutosDisponiveis.Clear();
            foreach (var produto in produtos)
            {
                ProdutosDisponiveis.Add(produto);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao buscar produtos: {ex.Message}", "Erro", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void AdicionarItem()
    {
        if (ProdutoSelecionado == null)
        {
            MessageBox.Show("Selecione um produto.", "Aviso", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (QuantidadeItem <= 0)
        {
            MessageBox.Show("Quantidade deve ser maior que zero.", "Aviso", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var itemExistente = ItensVenda.FirstOrDefault(i => i.ProdutoId == ProdutoSelecionado.Id);
        
        if (itemExistente != null)
        {
            itemExistente.Quantidade += QuantidadeItem;
            itemExistente.ValorTotal = itemExistente.Quantidade * itemExistente.PrecoUnitario;
        }
        else
        {
            var novoItem = new ItemVendaTemp
            {
                ProdutoId = ProdutoSelecionado.Id,
                NomeProduto = ProdutoSelecionado.Nome,
                PrecoUnitario = ProdutoSelecionado.PrecoVenda,
                Quantidade = QuantidadeItem,
                ValorTotal = ProdutoSelecionado.PrecoVenda * QuantidadeItem
            };

            ItensVenda.Add(novoItem);
        }

        CalcularValorTotal();
        QuantidadeItem = 1;
        ProdutoSelecionado = null;
    }

    [RelayCommand]
    private void RemoverItem(ItemVendaTemp item)
    {
        ItensVenda.Remove(item);
        CalcularValorTotal();
    }

    [RelayCommand]
    private async Task FinalizarVenda()
    {
        if (!ItensVenda.Any())
        {
            MessageBox.Show("Adicione pelo menos um item à venda.", "Aviso", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var resultado = MessageBox.Show(
            $"Finalizar venda no valor de R$ {ValorTotal:F2}?", 
            "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (resultado == MessageBoxResult.Yes)
        {
            try
            {
                var itens = ItensVenda.Select(i => (i.ProdutoId, i.Quantidade)).ToList();
                await _vendaService.CriarVendaAsync(itens, 
                    string.IsNullOrWhiteSpace(Observacao) ? null : Observacao);

                MessageBox.Show("Venda finalizada com sucesso!", "Sucesso", 
                    MessageBoxButton.OK, MessageBoxImage.Information);

                IniciarNovaVenda();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao finalizar venda: {ex.Message}", "Erro", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    [RelayCommand]
    private void IniciarNovaVenda()
    {
        ItensVenda.Clear();
        BuscaProduto = string.Empty;
        Observacao = string.Empty;
        QuantidadeItem = 1;
        ProdutoSelecionado = null;
        ValorTotal = 0;
        BuscarProdutosCommand.Execute(null);
    }

    private void CalcularValorTotal()
    {
        ValorTotal = ItensVenda.Sum(i => i.ValorTotal);
    }
}

/// <summary>
/// Classe temporária para representar itens da venda no ViewModel.
/// </summary>
public partial class ItemVendaTemp : ObservableObject
{
    [ObservableProperty]
    private int _produtoId;

    [ObservableProperty]
    private string _nomeProduto = string.Empty;

    [ObservableProperty]
    private decimal _precoUnitario;

    [ObservableProperty]
    private int _quantidade;

    [ObservableProperty]
    private decimal _valorTotal;
}
