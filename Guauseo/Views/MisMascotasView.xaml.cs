using Microsoft.Maui.Controls;
using Guauseo.ViewModels;

namespace Guauseo.Views;

public partial class MisMascotasView : ContentPage
{
	public MisMascotasView(decimal due�oCodigo)
	{
		InitializeComponent();
        BindingContext = new MisMascotasViewModel(due�oCodigo, Navigation);
    }
}