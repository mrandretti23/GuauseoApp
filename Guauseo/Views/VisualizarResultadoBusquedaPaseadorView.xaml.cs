using Guauseo.ViewModels;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace Guauseo.Views;

public partial class VisualizarResultadoBusquedaPaseadorView : ContentPage
{
    private readonly VisualizarResultadoBusquedaPaseadorViewModel _viewModel;

    public VisualizarResultadoBusquedaPaseadorView(decimal paseadorCodigo, decimal codigoPaseo)
	{
        InitializeComponent();
        _viewModel = new VisualizarResultadoBusquedaPaseadorViewModel(paseadorCodigo, codigoPaseo);
        BindingContext = _viewModel;

        // Iniciar la carga de datos del cliente cercano
        _viewModel.LoadClienteCercano();

        // Subscribe to changes in RouteCoordinates
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;

        // Establecer el mapa en el ViewModel
        _viewModel.SetMap(map);
    }

    private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(VisualizarResultadoBusquedaPaseadorViewModel.RouteCoordinates))
        {
            UpdatePolyline();
        }

        if (e.PropertyName == nameof(VisualizarResultadoBusquedaPaseadorViewModel.MapPins))
        {
            UpdateMapPins();
            MoveMapToPinsRegion();
        }
    }


    private void UpdatePolyline()
    {
        var routeCoordinates = _viewModel.RouteCoordinates;

        if (routeCoordinates != null)
        {
            var polyline = new Polyline
            {
                StrokeColor = Colors.Blue,
                StrokeWidth = 3
            };

            foreach (var location in routeCoordinates)
            {
                polyline.Geopath.Add(location);
            }

            map.MapElements.Clear();
            map.MapElements.Add(polyline);
        }

        
    }

    private void UpdateMapPins()
    {
        if (_viewModel.MapPins != null)
        {
            map.Pins.Clear();

            foreach (var pin in _viewModel.MapPins)
            {
                map.Pins.Add(pin);
            }
        }
    }

    private void MoveMapToPinsRegion()
    {
        var dueñoPin = _viewModel.MapPins.FirstOrDefault(p => p.Label == "Dueño");
        var paseadorPin = _viewModel.MapPins.FirstOrDefault(p => p.Label == "Mi Ubicación");

        if (dueñoPin != null && paseadorPin != null)
        {
            var centerLatitude = (dueñoPin.Location.Latitude + paseadorPin.Location.Latitude) / 2;
            var centerLongitude = (dueñoPin.Location.Longitude + paseadorPin.Location.Longitude) / 2;
            var centerLocation = new Location(centerLatitude, centerLongitude);

            var distance = Location.CalculateDistance(dueñoPin.Location, paseadorPin.Location, DistanceUnits.Kilometers);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(centerLocation, Distance.FromKilometers(distance / 2 + 1)));
        }
    }
}
