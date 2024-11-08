using Guauseo.ViewModels;
using Guauseo.Services;


namespace Guauseo.Views;


public partial class RegistroMascotaView : ContentPage
{
	public RegistroMascotaView(decimal due�oCodigo)
	{
		InitializeComponent();
        BindingContext = new RegistroMascotaViewModel(due�oCodigo, Navigation);
        var viewModel = new RegistroMascotaViewModel(due�oCodigo, Navigation);
        viewModel.ShowAlert += async (message) => await DisplayAlert("Mensaje", message, "OK");
        BindingContext = viewModel;
    }
}