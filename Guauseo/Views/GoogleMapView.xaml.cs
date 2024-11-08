using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Controls;
using Guauseo.ViewModels;


namespace Guauseo.Views;

public partial class GoogleMapView : ContentPage
{
    public GoogleMapView()
    {
        InitializeComponent();
        var viewModel = new GoogleMapViewModel();
        viewModel.Map = mape;
        viewModel.Navigation = Navigation;
        BindingContext = viewModel;

        InitializeMap(viewModel);
    }

    private async void InitializeMap(GoogleMapViewModel viewModel)
    {
        try
        {
            viewModel.InitializeMapAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing map: {ex.Message}");
            await DisplayAlert("Error", "Ocurrió un error al obtener la ubicación actual.", "OK");
        }
    }

    private void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        var viewModel = (GoogleMapViewModel)BindingContext;
        viewModel.OnMapClicked(e.Location);
    }


}