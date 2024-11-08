using Guauseo.ViewModels;

namespace Guauseo.Views;

public partial class CalificacionView : ContentPage
{
	public CalificacionView(decimal codigoPaseo, decimal? codigoPaseador)
	{
		InitializeComponent();
        BindingContext = new CalificacionViewModel(codigoPaseo, codigoPaseador);
    }
}