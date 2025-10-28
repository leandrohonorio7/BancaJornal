using System.Windows;
using BancaJornal.Desktop.ViewModels;

namespace BancaJornal.Desktop.Views;

/// <summary>
/// Janela principal da aplicação.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
