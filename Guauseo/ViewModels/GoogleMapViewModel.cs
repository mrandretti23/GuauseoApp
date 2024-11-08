using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Guauseo.ViewModels
{
    public class GoogleMapViewModel : ObservableObject
    {
        private Location _selectedLocation;
        private string _address;
        private readonly string googleMapsApiKey = "AIzaSyCU1--GmBvQANF5WSO1H-XDu5yJ1Ka2kfI";

        private Microsoft.Maui.Controls.Maps.Map _map;

        public Microsoft.Maui.Controls.Maps.Map Map
        {
            get => _map;
            set
            {
                _map = value;
                OnPropertyChanged();
            }
        }
        public INavigation Navigation { get; set; }

        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged();
            }
        }

        public Action<Location, string> LocationSelected { get; set; }
        public ICommand SearchCommand { get; }
        public ICommand ConfirmLocationCommand { get; }

        public GoogleMapViewModel()
        {
            SearchCommand = new Command(async () => await OnSearchClicked());
            ConfirmLocationCommand = new Command(async () => await OnConfirmLocationClicked());

            InitializeMapAsync();
        }

        public async Task InitializeMapAsync()
        {
            var location = await GetCurrentLocation();
            if (location != null)
            {
                var mapSpan = MapSpan.FromCenterAndRadius(location, Distance.FromMiles(1));
                Map.MoveToRegion(mapSpan);
            }
        }

        private async Task<Location> GetCurrentLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    return new Location(location.Latitude, location.Longitude);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo dirección: {ex.Message}");
            }

            return null;
        }

        public void OnMapClicked(Location location)
        {
            if (location != null)
            {
                _selectedLocation = location;
                Map.Pins.Clear();
                Map.Pins.Add(new Pin
                {
                    Label = "Selecciona tu ubicación",
                    Location = _selectedLocation
                });
            }
        }

        private async Task OnSearchClicked()
        {
            var address = Address;
            if (!string.IsNullOrWhiteSpace(address))
            {
                var locations = await Geocoding.GetLocationsAsync(address);
                var location = locations?.FirstOrDefault();
                if (location != null)
                {
                    var mapSpan = MapSpan.FromCenterAndRadius(new Location(location.Latitude, location.Longitude), Distance.FromMiles(1));
                    Map.MoveToRegion(mapSpan);

                    Map.Pins.Clear();
                    Map.Pins.Add(new Pin
                    {
                        Label = address,
                        Location = new Location(location.Latitude, location.Longitude)
                    });
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("No encontrado", "Ubicación no encontrada", "OK");
                }
            }
        }

        private async Task OnConfirmLocationClicked()
        {
            if (_selectedLocation != null)
            {
                var placemarks = await Geocoding.GetPlacemarksAsync(_selectedLocation);
                var placemark = placemarks?.FirstOrDefault();
                var address = placemark != null ? $"{placemark.Thoroughfare}, {placemark.Locality}" : "Ubicación seleccionada";

                LocationSelected?.Invoke(_selectedLocation, address);
                await Navigation.PopModalAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Escoge una dirección en el mapa.", "OK");
            }
        }

    }

}