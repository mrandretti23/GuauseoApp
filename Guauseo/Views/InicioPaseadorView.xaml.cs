using Guauseo.ViewModels;

namespace Guauseo.Views;

public partial class InicioPaseadorView : ContentPage
{
    private readonly decimal _usuario;
    public InicioPaseadorView(decimal paseadorCodigo)
	{
		InitializeComponent();
        _usuario = paseadorCodigo;
        BindingContext = new InicioPaseadorViewModel(_usuario);

    }

    private async void OfrecerPaseoButton_Clicked(object sender, EventArgs e)
    {
        var viewModel = (InicioPaseadorViewModel)BindingContext;
        await viewModel.OfrecerPaseoCommand.ExecuteAsync(null);
    }

    private async void MiPerfilButton_Clicked(object sender, EventArgs e)
    {

        await Navigation.PushModalAsync(new Views.MiPerfilPaseadorView(_usuario));
    }
}