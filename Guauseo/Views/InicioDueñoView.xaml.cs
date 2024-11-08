using Microsoft.EntityFrameworkCore;
using Guauseo.Models;
using System.ComponentModel;
using System.Drawing;
using Guauseo.ViewModels;
using Microsoft.Maui.Controls;

namespace Guauseo.Views;



public partial class InicioDueñoView : ContentPage
{
	private readonly decimal _usuario;
	public InicioDueñoView(decimal user)
	{
		InitializeComponent();
		_usuario = user;
	}

    private async void MisMascotasButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new Views.MisMascotasView(_usuario));
    }

    private async void MiPerfilButton_Clicked(object sender, EventArgs e)
    {
        
        await Navigation.PushModalAsync(new Views.MiPerfilDueñoView(_usuario));
    }

    private async void SolicitarPaseoButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new Views.SolicitarPaseoView(_usuario));
    }

}