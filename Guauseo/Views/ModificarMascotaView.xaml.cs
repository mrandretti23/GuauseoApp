using Guauseo.ViewModels;

namespace Guauseo.Views;

public partial class ModificarMascotaView : ContentPage
{
    public ModificarMascotaView(decimal due�oCodigo, decimal mascotaCodigo)
    {
        InitializeComponent();
        BindingContext = new ModificarMascotaViewModel(due�oCodigo, mascotaCodigo, Navigation);
        var viewModel = new ModificarMascotaViewModel(due�oCodigo, mascotaCodigo, Navigation);
        viewModel.ShowAlert += async (message) => await DisplayAlert("Mensaje", message, "OK");
        BindingContext = viewModel;
    }
}