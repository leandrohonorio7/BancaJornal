using System.Windows.Controls;

namespace BancaJornal.Desktop.Views;

public partial class ProdutoView : UserControl
{
    public ProdutoView()
    {
        InitializeComponent();
    }

    private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is ViewModels.ProdutoViewModel vm)
        {
            vm.CarregarProdutosCommand.Execute(null);
        }
    }
}
