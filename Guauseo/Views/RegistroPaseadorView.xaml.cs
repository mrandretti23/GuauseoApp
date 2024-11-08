namespace Guauseo.Views;
using Guauseo.ViewModels;

public partial class RegistroPaseadorView : ContentPage
{
	public RegistroPaseadorView()
	{
		InitializeComponent();
        BindingContext = new RegistroPaseadorViewModel();
        var viewModel = new RegistroPaseadorViewModel();
        viewModel.ShowAlert += async (message) => await DisplayAlert("Mensaje", message, "OK");
        BindingContext = viewModel;
    }
}