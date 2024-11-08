using Guauseo.ViewModels;
using Guauseo.Models;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Controls.Maps;
using System.Collections.ObjectModel;


namespace Guauseo.Views;

public partial class VisualizarEstadoPaseoView : ContentPage
{
    private Polyline _rutaPolyline;

    public VisualizarEstadoPaseoView(decimal codigoPaseo)
    {
        InitializeComponent();
        var viewModel = new VisualizarEstadoPaseoViewModel(codigoPaseo);
        BindingContext = viewModel;

        viewModel.SetMap(map);

        // Inicializar el mapa y configurar la ruta
        map.MapType = MapType.Street;
        _rutaPolyline = new Polyline
        {
            StrokeColor = Colors.Blue,
            StrokeWidth = 5
        };

        map.MapElements.Add(_rutaPolyline);

        // Suscribirse a los cambios en la ruta desde el ViewModel
        if (viewModel != null)
        {
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        // Inicializar la ruta en caso de que ya haya datos al inicio
        ActualizarRuta(viewModel.RouteCoordinates);
        ActualizarPines(viewModel.MapPins);
    }

    private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {

        // Verificación de nulos
        if (sender == null || e == null)
            return;

        if (BindingContext is VisualizarEstadoPaseoViewModel viewModel)
        {
            if (e.PropertyName == nameof(VisualizarEstadoPaseoViewModel.RouteCoordinates))
            {
                ActualizarRuta(viewModel.RouteCoordinates);
            }

            if (e.PropertyName == nameof(VisualizarEstadoPaseoViewModel.MapPins))
            {
                ActualizarPines(viewModel.MapPins);
            }
        }
    }

    private void ActualizarRuta(ObservableCollection<Location> coordinates)
    {
        if (coordinates == null || coordinates.Count == 0)
            return;

        _rutaPolyline.Geopath.Clear();

        foreach (var coord in coordinates)
        {
            _rutaPolyline.Geopath.Add(coord);
        }

        // Ajustar la vista del mapa para que muestre la ruta completa
        var initialLocation = coordinates.First();
        map.MoveToRegion(MapSpan.FromCenterAndRadius(initialLocation, Distance.FromKilometers(1)));
    }

    private void ActualizarPines(ObservableCollection<Pin> pines)
    {
        if (pines == null || pines.Count == 0)
            return;

        map.Pins.Clear(); // Limpia todos los pines actuales

        foreach (var pin in pines)
        {
            map.Pins.Add(pin); // Agrega los nuevos pines al mapa
        }

        // Ajustar la vista del mapa para centrarse en los pines
        if (pines.Count > 0)
        {
            var centerLocation = pines[0].Location;
            map.MoveToRegion(MapSpan.FromCenterAndRadius(centerLocation, Distance.FromKilometers(1))); // Ajusta el zoom y la ubicación
        }
    }
}