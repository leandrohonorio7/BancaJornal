using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BancaJornal.Application.DTOs;
using BancaJornal.Application.Services;

namespace BancaJornal.Desktop.ViewModels;

/// <summary>
/// ViewModel para tela de dashboard.
/// Exibe métricas e informações resumidas do sistema.
/// Aplica MVVM e SRP.
/// </summary>
public partial class DashboardViewModel : ObservableObject
{
    private readonly DashboardService _dashboardService;

    [ObservableProperty]
    private int _produtosEmEstoque;

    [ObservableProperty]
    private int _produtosComEstoqueBaixo;

    [ObservableProperty]
    private int _vendasMesCorrente;

    [ObservableProperty]
    private decimal _totalVendasMesCorrente;

    [ObservableProperty]
    private ObservableCollection<ProdutoVendidoDto> _produtosMaisVendidos = new();

    [ObservableProperty]
    private bool _carregando;

    public DashboardViewModel(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [RelayCommand]
    private async Task CarregarDados()
    {
        try
        {
            Carregando = true;
            var dados = await _dashboardService.ObterDadosDashboardAsync();

            ProdutosEmEstoque = dados.ProdutosEmEstoque;
            ProdutosComEstoqueBaixo = dados.ProdutosComEstoqueBaixo;
            VendasMesCorrente = dados.VendasMesCorrente;
            TotalVendasMesCorrente = dados.TotalVendasMesCorrente;
            
            ProdutosMaisVendidos.Clear();
            foreach (var produto in dados.ProdutosMaisVendidos)
            {
                ProdutosMaisVendidos.Add(produto);
            }
        }
        catch (Exception ex)
        {
            // Em produção, usar sistema de logging adequado
            System.Windows.MessageBox.Show($"Erro ao carregar dashboard: {ex.Message}", "Erro", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
        finally
        {
            Carregando = false;
        }
    }
}
