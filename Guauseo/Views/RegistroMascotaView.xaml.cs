using Guauseo.ViewModels;
using Guauseo.Services;


namespace Guauseo.Views;


public partial class RegistroMascotaView : ContentPage
{
	public RegistroMascotaView(decimal dueñoCodigo)
	{
		InitializeComponent();
        BindingContext = new RegistroMascotaViewModel(dueñoCodigo, Navigation);
        var viewModel = new RegistroMascotaViewModel(dueñoCodigo, Navigation);
        viewModel.ShowAlert += async (message) => await DisplayAlert("Mensaje", message, "OK");
        BindingContext = viewModel;
    }
}