using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Guauseo.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Guauseo.ViewModels
{
    public partial class InicioPaseadorViewModel : ObservableObject
    {
        private readonly PaseoDbContext _paseodbContext;
        private readonly decimal _paseadorCodigo;
        private readonly CalificacionDbContext _calificacionDbContext;

        [ObservableProperty]
        private string _calificacionPromedio;

        public IAsyncRelayCommand OfrecerPaseoCommand { get; }

        public InicioPaseadorViewModel()
        {

        }

        public InicioPaseadorViewModel(decimal paseadorCodigo)
        {
            _paseodbContext = new PaseoDbContext();
            _paseadorCodigo = paseadorCodigo;
            _calificacionDbContext = new CalificacionDbContext();

            OfrecerPaseoCommand = new AsyncRelayCommand(OfrecerPaseoAsync);

            CargarCalificacionesAsync();
        }

        private async Task OfrecerPaseoAsync()
        {
            var (latitude, longitude) = await GetCurrentLocationAsync();

            if (latitude != 0 && longitude != 0)
            {
                await Application.Current.MainPage.DisplayAlert("Buscando", "Se está buscando solicitud de paseo...", "OK");
                await BuscarDueñoCercano(latitude, longitude);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo obtener la ubicación actual.", "OK");
            }
        }

        public async Task<(double Latitude, double Longitude)> GetCurrentLocationAsync()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    return (location.Latitude, location.Longitude);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo ubicación: {ex.Message}");
            }

            return (0, 0); // O manejar el error adecuadamente
        }

        public async Task BuscarDueñoCercano(double paseadorLatitude, double paseadorLongitude)
        {
            while (true)
            {
                var paseos = await _paseodbContext.Paseo
                    .Where(p => p.EstadoPaseo == "Buscando")
                    .ToListAsync();

                foreach (var paseo in paseos)
                {
                    var dueñoCoords = paseo.CoordenadasDueño.Split(',');
                    var dueñoLatitude = double.Parse(dueñoCoords[0]);
                    var dueñoLongitude = double.Parse(dueñoCoords[1]);

                    var distance = CalculateHaversineDistance(paseadorLatitude, paseadorLongitude, dueñoLatitude, dueñoLongitude);

                    if (distance <= 2)
                    {
                        
                        decimal CodigoPaseo = paseo.Codigo;

                        await Application.Current.MainPage.DisplayAlert("Encontrado", "Se encontró una solicitud de paseo cercana.", "OK");

                        paseo.EstadoPaseo = "Espera";
                        await _paseodbContext.SaveChangesAsync();

                        await Application.Current.MainPage.Navigation.PushModalAsync(new Views.VisualizarResultadoBusquedaPaseadorView(_paseadorCodigo, CodigoPaseo));

                        return;
                    }
                }

                await Task.Delay(20000); // Espera 20 segundos antes de la próxima verificación
            }
        }

        private double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371; // Radio de la Tierra en km
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);
            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double angle)
        {
            return angle * (Math.PI / 180);
        }


        public async Task CargarCalificacionesAsync()
        {
            try
            {
                // Obtener las calificaciones del paseador
                var calificaciones = await _calificacionDbContext.Calificacion
                    .Where(c => c.PaseadorCodigo == _paseadorCodigo)
                    .ToListAsync();

                if (calificaciones != null && calificaciones.Count > 0)
                {
                    // Convertir las calificaciones a valores numéricos
                    var valoresCalificacion = calificaciones.Select(c => ConvertirCalificacion(c.Calificacion)).ToList();

                    // Calcular la media
                    var promedio = valoresCalificacion.Average();

                    // Mostrar el resultado en el Label
                    CalificacionPromedio = $"CALIFICACIÓN: {promedio:N2}";
                }
                else
                {
                    CalificacionPromedio = "CALIFICACIÓN: No hay calificaciones aún.";
                }
            }
            catch (Exception ex)
            {
                // Manejar errores
                CalificacionPromedio = $"Error al obtener las calificaciones: {ex.Message}";
            }
        }

        private decimal ConvertirCalificacion(string calificacion)
        {
            return calificacion switch
            {
                "1 patita" => 1,
                "2 patitas" => 2,
                "3 patitas" => 3,
                "4 patitas" => 4,
                "5 patitas" => 5,
                _ => 0 // Por si acaso hay algún valor inesperado
            };
        }

    }
}

