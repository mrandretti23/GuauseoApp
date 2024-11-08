using Microsoft.Maui.Controls;
using Guauseo.ViewModels;

namespace Guauseo.Views;

public partial class MisMascotasView : ContentPage
{
	public MisMascotasView(decimal dueñoCodigo)
	{
		InitializeComponent();
        BindingContext = new MisMascotasViewModel(dueñoCodigo, Navigation);
    }
}