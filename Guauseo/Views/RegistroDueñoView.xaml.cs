namespace Guauseo.Views;
using Guauseo.ViewModels;

public partial class RegistroDueñoView : ContentPage
{
    
	public RegistroDueñoView()
	{
		InitializeComponent();
		BindingContext = new RegistroDueñoViewModel();
        var viewModel = new RegistroDueñoViewModel();
        viewModel.ShowAlert += async (message) => await DisplayAlert("Mensaje", message, "OK");
        BindingContext = viewModel;
    }
}