using System.Windows.Controls;

namespace BancaJornal.Desktop.Views;

public partial class VendaView : UserControl
{
    public VendaView()
    {
        InitializeComponent();
    }

    private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is ViewModels.VendaViewModel vm)
        {
            vm.IniciarNovaVendaCommand.Execute(null);
        }
    }
}
