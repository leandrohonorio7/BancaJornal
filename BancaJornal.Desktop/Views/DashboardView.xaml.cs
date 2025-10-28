using System.Windows.Controls;

namespace BancaJornal.Desktop.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
    }

    private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        // Carregar dados automaticamente ao exibir a view
        if (DataContext is ViewModels.DashboardViewModel vm)
        {
            vm.CarregarDadosCommand.Execute(null);
        }
    }
}
