using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BancaJornal.Desktop.ViewModels;

/// <summary>
/// ViewModel principal da aplicação.
/// Gerencia navegação entre telas e estado global.
/// Aplica MVVM - separação entre lógica e apresentação.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableObject? _currentViewModel;

    [ObservableProperty]
    private string _titulo = "Banca de Jornal - Sistema de Gerenciamento";

    private readonly DashboardViewModel _dashboardViewModel;
    private readonly ProdutoViewModel _produtoViewModel;
    private readonly VendaViewModel _vendaViewModel;

    public MainViewModel(
        DashboardViewModel dashboardViewModel,
        ProdutoViewModel produtoViewModel,
        VendaViewModel vendaViewModel)
    {
        _dashboardViewModel = dashboardViewModel;
        _produtoViewModel = produtoViewModel;
        _vendaViewModel = vendaViewModel;

        // Iniciar com dashboard
        CurrentViewModel = _dashboardViewModel;
    }

    [RelayCommand]
    private void NavegarParaDashboard()
    {
        CurrentViewModel = _dashboardViewModel;
        _dashboardViewModel.CarregarDadosCommand.Execute(null);
    }

    [RelayCommand]
    private void NavegarParaProdutos()
    {
        CurrentViewModel = _produtoViewModel;
        _produtoViewModel.CarregarProdutosCommand.Execute(null);
    }

    [RelayCommand]
    private void NavegarParaVendas()
    {
        CurrentViewModel = _vendaViewModel;
        _vendaViewModel.IniciarNovaVendaCommand.Execute(null);
    }
}
