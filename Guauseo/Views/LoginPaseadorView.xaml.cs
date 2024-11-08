using Microsoft.EntityFrameworkCore;
using Guauseo.Models;
using System.ComponentModel;
using System.Drawing;
using Guauseo.ViewModels;
using Microsoft.Maui.Controls;

namespace Guauseo.Views;

public partial class LoginPaseadorView : ContentPage
{
	public LoginPaseadorView()
	{
		InitializeComponent();

        BindingContext = new LoginPaseadorViewModel(Navigation);
        var viewModel = new LoginPaseadorViewModel(Navigation);
        viewModel.ShowAlert += async (message) => await DisplayAlert("Error", message, "OK");
        BindingContext = viewModel;
    }

    private void btn_registrate_Clicked(object sender, EventArgs e)
    {
        Navigation.PushModalAsync(new Views.RegistroPaseadorView());
        

    }
}