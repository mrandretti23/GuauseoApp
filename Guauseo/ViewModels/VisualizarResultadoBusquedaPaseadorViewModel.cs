using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Guauseo.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Guauseo.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Guauseo.ViewModels
{
    public partial class VisualizarResultadoBusquedaPaseadorViewModel : ObservableObject
    {
        [ObservableProperty]
        private string direccion;

        [ObservableProperty]
        private string nombreDueño;

        [ObservableProperty]
        private string telefonoDueño;

        [ObservableProperty]
        private ObservableCollection<string> nombresMascotas;

        [ObservableProperty]
        private string tiempoPaseo;

        [ObservableProperty]
        private string valorAPagar;

        [ObservableProperty]
        private string estadoPaseo;

        [ObservableProperty]
        private MapSpan mapSpan;

        [ObservableProperty]
        private ObservableCollection<Pin> mapPins;

        [ObservableProperty]
        private ObservableCollection<Location> routeCoordinates;

        [ObservableProperty]
        private bool isComenzarPaseoVisible;

        [ObservableProperty]
        private bool isAceptarPaseoVisible;

        [ObservableProperty]
        private bool isCancelarPaseoVisible;

        [ObservableProperty]
        private bool isEstoyAfuera;

        [ObservableProperty]
        private bool isDarInicioPaseo;

        [ObservableProperty]
        private bool isEstoyDeRegreso;

        [ObservableProperty]
        private bool isFinalizarPaseo;

        [ObservableProperty]
        private bool isMapaVisible;

        private Microsoft.Maui.Controls.Maps.Map _map;


        public IRelayCommand AceptarPaseoCommand { get; }
        public IRelayCommand CancelarPaseoCommand { get; }

        public IRelayCommand ComenzarPaseoCommand { get; }

        public IRelayCommand EstoyAfueraCommand { get; }

        public IRelayCommand DarInicioPaseoCommand { get; }

        public IRelayCommand EstoyDeRegresoCommand { get; }

        public IRelayCommand FinalizarPaseoCommand { get; }

        private Timer _updateLocationTimer;


        private readonly decimal _paseadorCodigo;
        private readonly decimal _codigoPaseo;
        private readonly PaseoDbContext _paseodbContext;
        private readonly MascotaDbContext _mascotaDbContext;
        private readonly DueñoDbContext _dueñoDbContext;


        public VisualizarResultadoBusquedaPaseadorViewModel()
        {

        }
        
        public VisualizarResultadoBusquedaPaseadorViewModel(decimal paseadorCodigo, decimal codigoPaseo)
        {
            _paseadorCodigo = paseadorCodigo;
            _codigoPaseo = codigoPaseo;
            _paseodbContext = new PaseoDbContext();
            _mascotaDbContext = new MascotaDbContext();
            _dueñoDbContext = new DueñoDbContext();
            NombresMascotas = new ObservableCollection<string>();

            AceptarPaseoCommand = new RelayCommand(async () => await AceptarPaseo());
            CancelarPaseoCommand = new RelayCommand(async () => await CancelarPaseo());
            ComenzarPaseoCommand = new RelayCommand(async () => await ComenzarPaseo());
            EstoyAfueraCommand = new RelayCommand(async () => await EstoyAfuera());
            DarInicioPaseoCommand = new RelayCommand(async () => await DarInicioPaseo());
            EstoyDeRegresoCommand = new RelayCommand(async () => await EstoyDeRegreso());
            FinalizarPaseoCommand = new RelayCommand(async () => await FinalizarPaseo());

            IsAceptarPaseoVisible = true;
            IsCancelarPaseoVisible = true;
            IsMapaVisible = false;
            IsEstoyAfuera = false;
            IsComenzarPaseoVisible = false;
            IsDarInicioPaseo = false;
            IsEstoyDeRegreso = false;
            IsFinalizarPaseo = false;
        }

        public async void LoadClienteCercano()
        {
            var paseo = await _paseodbContext.Paseo
                .FirstOrDefaultAsync(p => p.Codigo == _codigoPaseo);

            if (paseo != null)
            {
                Direccion = paseo.UbicacionNombre;
                TiempoPaseo = paseo.TiempoSeleccionado;
                ValorAPagar = paseo.ValorPagar;

                var dueño = await _dueñoDbContext.Dueños
                    .FirstOrDefaultAsync(d => d.Codigo == paseo.DueñoCodigo);

                if (dueño != null)
                {
                    NombreDueño = dueño.Nombre;
                    TelefonoDueño = dueño.Telefono;
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se encontraron detalles del dueño.", "OK");
                    return;
                }

                var codigosMascotas = new List<decimal>();
                if (paseo.CodigoMascota1 != null)
                {
                    codigosMascotas.Add(paseo.CodigoMascota1);
                }

                if (paseo.CodigoMascota2 != null)
                {
                    codigosMascotas.Add(paseo.CodigoMascota2.Value);
                }

                var mascotas = await _mascotaDbContext.Mascotas
                    .Where(m => codigosMascotas.Contains(m.Codigo) && m.DueñoCodigo == paseo.DueñoCodigo)
                    .ToListAsync();

                foreach (var mascota in mascotas)
                {
                    NombresMascotas.Add(mascota.Nombre);
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se encontraron detalles del paseo.", "OK");
            }
        }

        public void SetMap(Microsoft.Maui.Controls.Maps.Map map)
        {
            _map = map;
        }

        private async Task AceptarPaseo()
        {
            var paseo = await _paseodbContext.Paseo
                .FirstOrDefaultAsync(p => p.Codigo == _codigoPaseo);

            if (paseo != null) {

                paseo.PaseadorCodigo = _paseadorCodigo;
                paseo.EstadoPaseo = "Aceptado";
                await _paseodbContext.SaveChangesAsync();
                
                await Application.Current.MainPage.DisplayAlert("Aceptado", "Has aceptado el paseo.", "OK");

                EstadoPaseo = paseo.EstadoPaseo;

                IsComenzarPaseoVisible = true;
                IsAceptarPaseoVisible = false;
                IsCancelarPaseoVisible = false;
                IsMapaVisible = true;


                // Obtener coordenadas del dueño y del paseador

                var dueñoCoords = paseo.CoordenadasDueño.Split(',');
                var dueñoLatitude = double.Parse(dueñoCoords[0]);
                var dueñoLongitude = double.Parse(dueñoCoords[1]);

                var coordenadasDueño = new Location(dueñoLatitude, dueñoLongitude);

                var ubicacion = await Geolocation.GetLastKnownLocationAsync();
                if (ubicacion == null)
                {
                    ubicacion = await Geolocation.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Medium,
                        Timeout = TimeSpan.FromSeconds(30)
                    });
                }

                if (ubicacion == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo obtener la ubicación actual.", "OK");
                    return;
                }

                var coordenadasPaseador = new Location(ubicacion.Latitude, ubicacion.Longitude);

                if (coordenadasDueño == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Coordenadas del dueño no son válidas.", "OK");
                    return;
                }

                // Actualizar el mapa
                MapSpan = MapSpan.FromCenterAndRadius(coordenadasDueño, Distance.FromMiles(1));
                MapPins = new ObservableCollection<Pin>
                {
                    new Pin { Label = "Dueño", Address = Direccion, Type = PinType.Place, Location = coordenadasDueño },
                    new Pin { Label = "Mi Ubicación", Type = PinType.Place, Location = coordenadasPaseador }
                };

                // Obtener ruta desde Google Maps Directions API
                var route = await GetRoute(coordenadasPaseador, coordenadasDueño);
                RouteCoordinates = new ObservableCollection<Location>(route);

                // Mover el mapa a la región deseada
                if (_map != null)
                {
                    _map.MoveToRegion(MapSpan);
                }

            }
            else 
            {
                await Application.Current.MainPage.DisplayAlert("Error", "no se puedo aceptar el paseo.", "OK");
            }

        }

        private async Task ComenzarPaseo()
        {
            bool confirm = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Está seguro de iniciar el paseo ?", "Sí", "No");
            if (!confirm) return;

            await Application.Current.MainPage.DisplayAlert("Exito", "Dirigete al punto de recogida marcada en el mapa.", "OK");

            var paseo = await _paseodbContext.Paseo
                .FirstOrDefaultAsync(p => p.Codigo == _codigoPaseo);

            if (paseo != null)
            {
                paseo.EstadoPaseo = "En camino";
                await _paseodbContext.SaveChangesAsync();

                EstadoPaseo = paseo.EstadoPaseo;

                IsComenzarPaseoVisible = false;
                IsEstoyAfuera = true;

                // Iniciar el temporizador para actualizar la ubicación
                _updateLocationTimer = new Timer(20000); // 20 segundos
                _updateLocationTimer.Elapsed += async (sender, e) => await UpdateLocation();
                _updateLocationTimer.AutoReset = true;
                _updateLocationTimer.Start();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo comenzar el paseo.", "OK");
            }
        }

        private async Task UpdateLocation()
        {

            // Solicita una nueva lectura de la ubicación con un tiempo de espera y precisión deseados
            var ubicacion = await Geolocation.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.High,  // Alta precisión para obtener la ubicación actualizada
                Timeout = TimeSpan.FromSeconds(10)  // Tiempo de espera de 10 segundos
            });

            // Si no se puede obtener la ubicación actual, intenta obtener la última ubicación conocida
            if (ubicacion == null)
            {
                ubicacion = await Geolocation.GetLastKnownLocationAsync();
            }

            if (ubicacion == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo obtener la ubicación actual.", "OK");
                return;
            }

            var coordenadasPaseador = new Location(ubicacion.Latitude, ubicacion.Longitude);

            // Obtener el dispatcher del main page
            var dispatcher = Application.Current.MainPage.Dispatcher;

            // Ejecutar el código en el hilo de la interfaz de usuario
            await dispatcher.DispatchAsync(async () =>
            {
                if (_map != null)
                {
                    // Remover el pin anterior con el label "Mi Ubicación"
                    var previousPin = MapPins.FirstOrDefault(p => p.Label == "Mi Ubicación");
                    if (previousPin != null)
                    {
                        MapPins.Remove(previousPin);
                        _map.Pins.Remove(previousPin);
                    }

                    // Crear y agregar un nuevo pin con el label "Mi Ubicación Actual"
                    var newPin = new Pin
                    {
                        Label = "Mi Ubicación",
                        Type = PinType.Place,
                        Location = coordenadasPaseador
                    };
                    MapPins.Add(newPin);
                    _map.Pins.Add(newPin);

                    // Obtener la ruta actualizada desde la ubicación actual del paseador hasta el dueño
                    var dueñoCoords = _map.Pins.FirstOrDefault(p => p.Label == "Dueño")?.Location;

                    if (dueñoCoords != null)
                    {
                        var route = await GetRoute(coordenadasPaseador, dueñoCoords);
                        RouteCoordinates = new ObservableCollection<Location>(route);

                    }

                    // Ajusta el rango del mapa
                    _map.MoveToRegion(MapSpan.FromCenterAndRadius(coordenadasPaseador, Distance.FromMiles(1)));
                }
            });

            // Actualizar la base de datos
            var paseo = await _paseodbContext.Paseo.FirstOrDefaultAsync(p => p.Codigo == _codigoPaseo);
            if (paseo != null)
            {
                paseo.CoordenadasPaseador = $"{ubicacion.Latitude},{ubicacion.Longitude}";
                await _paseodbContext.SaveChangesAsync();
            }
        }


        private async Task EstoyAfuera()
        {
            bool confirm = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Está seguro de avisar al dueño que llegaste al lugar de recogida ?", "Sí", "No");
            if (!confirm) return;

            await Application.Current.MainPage.DisplayAlert("Exito", "Espera en el punto hasta que el dueño salga con su mascota.", "OK");

            var paseo = await _paseodbContext.Paseo
                .FirstOrDefaultAsync(p => p.Codigo == _codigoPaseo);

            if (paseo != null)
            {
                paseo.EstadoPaseo = "Estoy Afuera";
                await _paseodbContext.SaveChangesAsync();

                EstadoPaseo = paseo.EstadoPaseo;

                IsEstoyAfuera = false;
                IsDarInicioPaseo = true;

            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se puede avisar que estoy afuera.", "OK");
            }
        }


        private async Task DarInicioPaseo()
        {
            bool confirm = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Está seguro de comenzar el paseo ?", "Sí", "No");
            if (!confirm) return;

            await Application.Current.MainPage.DisplayAlert("Paseo Iniciado", "Lleva a pasear a la mascota por la zona o siguiendo las instrucciones del dueño.", "OK");

            using (var paseoDbContext = new PaseoDbContext())
            {    
                var paseo = await paseoDbContext.Paseo
                .FirstOrDefaultAsync(p => p.Codigo == _codigoPaseo);

                if (paseo != null)
                {
                    paseo.EstadoPaseo = "Paseando";
                    await paseoDbContext.SaveChangesAsync();

                    EstadoPaseo = paseo.EstadoPaseo;

                    IsDarInicioPaseo = false;
                    IsEstoyDeRegreso = true;

                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se puede dar inicio al paseo.", "OK");
                }
            }
        }

        private async Task EstoyDeRegreso()
        {
            bool confirm = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Está seguro de avisar al dueño que te encuentras de regreso ?", "Sí", "No");
            if (!confirm) return;

            using (var paseoDbContext = new PaseoDbContext())
            {
                var paseo = await paseoDbContext.Paseo
                .FirstOrDefaultAsync(p => p.Codigo == _codigoPaseo);

                if (paseo != null)
                {
                    paseo.EstadoPaseo = "Estoy de regreso";
                    await paseoDbContext.SaveChangesAsync();

                    EstadoPaseo = paseo.EstadoPaseo;

                    IsEstoyDeRegreso = false;
                    IsFinalizarPaseo = true;

                    await Application.Current.MainPage.DisplayAlert("Avisando al dueño", "Espera en el punto hasta que recojan a la mascota.", "OK");

                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se puede avisar que estas de regreso.", "OK");
                }
            }
        }


        private async Task FinalizarPaseo()
        {
            bool confirm = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Está seguro de finalizar con exito el paseo ?", "Sí", "No");
            if (!confirm) return;

            using (var paseoDbContext = new PaseoDbContext())
            {
                var paseo = await paseoDbContext.Paseo
                .FirstOrDefaultAsync(p => p.Codigo == _codigoPaseo);

                if (paseo != null)
                {
                    paseo.EstadoPaseo = "Finalizado";
                    await paseoDbContext.SaveChangesAsync();

                    EstadoPaseo = paseo.EstadoPaseo;


                    await Application.Current.MainPage.DisplayAlert("Finalizado", "Paseo finalizado con exito.", "OK");
                    await App.Current.MainPage.Navigation.PopModalAsync();

                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se puede finalizar el paseo.", "OK");
                }
            }
        }


        private async Task CancelarPaseo()
        {
            {
                // Lógica para cancelar el paseo
                // Notifica al dueño que el paseo ha sido cancelado
                await Application.Current.MainPage.DisplayAlert("Cancelado", "Has cancelado el paseo.", "OK");


                var paseo = await _paseodbContext.Paseo
                .FirstOrDefaultAsync(p => p.Codigo == _codigoPaseo && p.EstadoPaseo == "Espera");

                if (paseo != null)
                {
                    paseo.EstadoPaseo = "Buscando";
                    await _paseodbContext.SaveChangesAsync();

                    // Navegar de regreso a la página anterior
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Error al cancelar paseo.", "OK");
                    return;
                }
            }
        }

        private async Task<IEnumerable<Location>> GetRoute(Location origin, Location destination)
        {
            var apiKey = "AIzaSyCU1--GmBvQANF5WSO1H-XDu5yJ1Ka2kfI";
            var url = $"https://maps.googleapis.com/maps/api/directions/json?origin={origin.Latitude},{origin.Longitude}&destination={destination.Latitude},{destination.Longitude}&key={apiKey}";

            using var client = new HttpClient();
            var response = await client.GetStringAsync(url);
            Console.WriteLine(response); // Imprime la respuesta para depuración
            var json = JObject.Parse(response);

            var status = json["status"]?.ToString();
            if (status != "OK")
            {
                var errorMessage = json["error_message"]?.ToString();
                throw new Exception($"Google Maps API error: {status} - {errorMessage}");
            }

            var route = new List<Location>();

            var steps = json["routes"]?.FirstOrDefault()?["legs"]?.FirstOrDefault()?["steps"];
            if (steps == null || !steps.Any())
            {
                throw new Exception("No steps found in the response.");
            }

            foreach (var step in steps)
            {
                var startLocation = step["start_location"];
                var endLocation = step["end_location"];

                if (startLocation != null && endLocation != null)
                {
                    route.Add(new Location((double)startLocation["lat"], (double)startLocation["lng"]));
                    route.Add(new Location((double)endLocation["lat"], (double)endLocation["lng"]));
                }
            }


            return route;
        }
    }

}
