using Microsoft.EntityFrameworkCore;
using Guauseo.Models;
using System.ComponentModel;
using System.Drawing;
using Guauseo.ViewModels;
using Microsoft.Maui.Controls;

namespace Guauseo.Views;

public partial class MiPerfilPaseadorView : ContentPage
{
	public MiPerfilPaseadorView(decimal usuario)
	{
		InitializeComponent();
        BindingContext = new MiPerfilPaseadorViewModel(usuario, Navigation);
    }
}