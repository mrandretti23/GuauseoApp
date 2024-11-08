using Microsoft.EntityFrameworkCore;
using Guauseo.Models;
using System.ComponentModel;
using System.Drawing;
using Guauseo.ViewModels;
using Microsoft.Maui.Controls;


namespace Guauseo.Views;

public partial class MiPerfilDueñoView : ContentPage
{
    //private readonly int _usuario;
    public MiPerfilDueñoView(decimal usuario)
	{
		InitializeComponent();
		BindingContext = new MiPerfilDueñoViewModel(usuario, Navigation);
		//_usuario = usuario;
        
    }
}