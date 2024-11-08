using Guauseo.ViewModels;

namespace Guauseo.Views;

public partial class ModificarMascotaView : ContentPage
{
    public ModificarMascotaView(decimal dueñoCodigo, decimal mascotaCodigo)
    {
        InitializeComponent();
        BindingContext = new ModificarMascotaViewModel(dueñoCodigo, mascotaCodigo, Navigation);
        var viewModel = new ModificarMascotaViewModel(dueñoCodigo, mascotaCodigo, Navigation);
        viewModel.ShowAlert += async (message) => await DisplayAlert("Mensaje", message, "OK");
        BindingContext = viewModel;
    }
}