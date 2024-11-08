using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Guauseo.Models;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Guauseo.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Timers;
using Timer = System.Timers.Timer;
using System.Reflection;

namespace Guauseo.ViewModels
{
    public partial class VisualizarEstadoPaseoViewModel : ObservableObject
    {
        private readonly PaseoDbContext _paseodbContext;
        private readonly PaseoDbContext _paseodbContext2;
        private readonly DueñoDbContext _dueñoDbContext;
        private readonly MascotaDbContext _mascotaDbContext;
        private readonly PaseadorDbContext _paseadorDbContext;
        private readonly CalificacionDbContext _calificacionDbContext;

        private readonly decimal _codigoPaseo;


        [ObservableProperty]
        private string _calificacionPromedio;

        [ObservableProperty]
        private string estadoPaseo;

        [ObservableProperty]
        private string valorPagar;

        [ObservableProperty]
        private string ubicacionNombre;

        [ObservableProperty]
        private string nombreDueno;

        [ObservableProperty]
        private string numeroTelefonoDueno;

        [ObservableProperty]
        private string nombreMascota1;

        [ObservableProperty]
        private string nombreMascota2;

        [ObservableProperty]
        private string tiempoSeleccionado;

        [ObservableProperty]
        private string nombrePaseador;

        [ObservableProperty]
        private string telefonoPaseador;

        [ObservableProperty]
        private string cedulaPaseador;

        [ObservableProperty]
        private string calificacionPaseador;

        [ObservableProperty]
        private bool _isCancelarPaseoVisible = true;

        [ObservableProperty]
        private bool isMapaVisible;

        [ObservableProperty]
        private ObservableCollection<Pin> mapPins;

        [ObservableProperty]
        private ObservableCollection<Location> routeCoordinates;

        [ObservableProperty]
        private MapSpan mapSpan;

        private Timer _updateLocationTimer;
        private Microsoft.Maui.Controls.Maps.Map _map;

        //public IAsyncRelayCommand CargarDatosCommand { get; }
        //public IAsyncRelayCommand ActualizarEstadoPaseoAsyncCommand { get; }
        public IRelayCommand CancelarPaseoCommand { get; }

        public VisualizarEstadoPaseoViewModel()
        {

        }
        
        public VisualizarEstadoPaseoViewModel(decimal codigoPaseo)
        {
            _paseodbContext = new PaseoDbContext();
            _paseodbContext2 = new PaseoDbContext();
            _dueñoDbContext = new DueñoDbContext();
            _mascotaDbContext = new MascotaDbContext();
            _paseadorDbContext = new PaseadorDbContext();
            _codigoPaseo = codigoPaseo;
            _calificacionDbContext = new CalificacionDbContext();

            CancelarPaseoCommand = new AsyncRelayCommand(CancelarPaseo);
            CargarDatosPaseo(codigoPaseo);
            ActualizarEstadoPaseoAsync();
            

        }

        public async Task CargarDatosPaseo(decimal codigoPaseo)
        {
            
                var paseo = await _paseodbContext.Paseo.FirstOrDefaultAsync(p => p.Codigo == codigoPaseo);
            if (paseo != null)
            {
                EstadoPaseo = paseo.EstadoPaseo;
                ValorPagar = paseo.ValorPagar;
                TiempoSeleccionado = paseo.TiempoSeleccionado;
                UbicacionNombre = paseo.UbicacionNombre;

                var dueño = await _dueñoDbContext.Dueños.FirstOrDefaultAsync(d => d.Codigo == paseo.DueñoCodigo);
                if (dueño != null)
                {
                    NombreDueno = dueño.Nombre;
                }

                var mascota1 = await _mascotaDbContext.Mascotas.FirstOrDefaultAsync(m => m.Codigo == paseo.CodigoMascota1);
                if (mascota1 != null)
                {
                    NombreMascota1 = mascota1.Nombre;
                }
                else
                {
                    NombreMascota1 = "Ninguno";
                }

                var mascota2 = await _mascotaDbContext.Mascotas.FirstOrDefaultAsync(m => m.Codigo == paseo.CodigoMascota2);
                if (mascota2 != null)
                {
                    NombreMascota2 = mascota2.Nombre;
                }
                else
                {
                    NombreMascota2 = "";
                }
                
            }
        }

        public async Task ActualizarEstadoPaseoAsync()
        {
            bool espera = false;
            while (!espera)
            {
                try
                {

                    using (var paseoDbContext = new PaseoDbContext())
                    //using (var paseadorDbContext = new PaseadorDbContext())
                    {
                        var paseo = await paseoDbContext.Paseo.FirstOrDefaultAsync(p => p.Codigo == _codigoPaseo);

                        if (paseo != null)
                        {
                            EstadoPaseo = paseo.EstadoPaseo;
                            //await Application.Current.MainPage.DisplayAlert("Exito", "Estado actualizado", "OK");
                            if (EstadoPaseo == "Aceptado")
                            {
                                espera = true;

                                await Application.Current.MainPage.DisplayAlert("Exito", "Se encontro un paseador.", "OK");
                                
                                var paseador = await _paseadorDbContext.Paseadores.FirstOrDefaultAsync(p => p.Codigo == paseo.PaseadorCodigo);
                                if (paseador != null)
                                {
                                    IsCancelarPaseoVisible = false;
                                    NombrePaseador = paseador.Nombre;
                                    TelefonoPaseador = paseador.Telefono;
                                    CedulaPaseador = paseador.Cedula;
                                    //CalificacionPaseador = paseador.Calificacion;
                                    CargarCalificacionesAsync();


                                }
                            }
                            
                        }

                            await Task.Delay(10000); // Espera 10 segundos antes de la próxima verificación
                    }
                }
                catch (Exception ex) 
                {
                    await Application.Current.MainPage.DisplayAlert("error", $"Error en ActualizarEstadoPaseoAsync: {ex.Message}", "OK");
                }

            }

            // Iniciar la tarea de actualización de estado en camino
            ActualizarEstadoEnCamino();
        }


        private async Task  ActualizarEstadoEnCamino()
        {
            //await Application.Current.MainPage.DisplayAlert("Exito", "Iniciando ActualizarEstadoEnCamino.", "OK");

            Timer estadoTimer = new Timer(10000); // Verificar cada 10 segundos
            estadoTimer.Elapsed += async (sender, e) =>
            {
                Application.Current.Dispatcher.Dispatch(async () =>
                {
                    try
                    {
                        using (var paseoDbContext = new PaseoDbContext())
                        {
                            //await Application.Current.MainPage.DisplayAlert("Exito", "Verificando estado del paseo.", "OK");

                            var paseo = await paseoDbContext.Paseo.FirstOrDefaultAsync(p => p.Codigo == _codigoPaseo);

                            if (paseo != null)
                            {
                                //await Application.Current.MainPage.DisplayAlert("Estado del paseo", paseo.EstadoPaseo, "OK");

                                if (paseo.EstadoPaseo == "En camino" && paseo.CoordenadasPaseador != null)
                                {
                                    estadoTimer.Stop(); // Detener el temporizador cuando el estado es "En camino"
                                    IsMapaVisible = true;
                                    EstadoPaseo = paseo.EstadoPaseo;

                                    var paseador = await _paseadorDbContext.Paseadores.FirstOrDefaultAsync(p => p.Codigo == paseo.PaseadorCodigo);
                                    if (paseador != null)
                                    {
                                        
                                        NombrePaseador = paseador.Nombre;
                                        TelefonoPaseador = paseador.Telefono;
                                        CedulaPaseador = paseador.Cedula;
                                        //CalificacionPaseador = paseador.Calificacion;
                                        CalificacionPaseador = "No cuenta con calificación todavia";

                                    }

                                    // Configurar el mapa y mostrar la ruta inicial
                                    await ConfigurarMapaYActualizarRuta(paseo.CoordenadasDueño, paseo.CoordenadasPaseador);

                                    // Iniciar el temporizador para actualizar la ubicación del paseador
                                    _updateLocationTimer = new Timer(20000);
                                    _updateLocationTimer.Elapsed += async (sender, e) =>
                                    {
                                        try
                                        {
                                            
                                            Application.Current.Dispatcher.Dispatch(async () =>
                                            {
                                                await ActualizarUbicacionPaseador();
                                            });
                                        }
                                        catch (Exception ex)
                                        {
                                            // Captura cualquier excepción que ocurra dentro del temporizador
                                            await Application.Current.MainPage.DisplayAlert("Error", $"Error en el temporizador: {ex.Message}\nStackTrace: {ex.StackTrace}", "OK");
                                        }
                                    };

                                        _updateLocationTimer.AutoReset = true;
                                        _updateLocationTimer.Start();
                                }
                            }
                        }
                    }
                    catch (TargetInvocationException ex)
                    {
                        // Capturar excepción interna y registrar información detallada
                        string errorMessage = $"Error en ActualizarEstadoEnCamino: {ex.Message}";
                        if (ex.InnerException != null)
                        {
                            errorMessage += $"\nExcepción interna: {ex.InnerException.Message}\nStackTrace: {ex.InnerException.StackTrace}";
                        }
                        await Application.Current.MainPage.DisplayAlert("Error", errorMessage, "OK");
                    }
                    catch (Exception ex)
                    {
                        string errorMessage = $"Error en ActualizarEstadoEnCamino: {ex.Message}\nStackTrace: {ex.StackTrace}";
                        await Application.Current.MainPage.DisplayAlert("Error", errorMessage, "OK");
                    }
                });
            };
            estadoTimer.Start();

        }

        public void SetMap(Microsoft.Maui.Controls.Maps.Map map)
        {
            _map = map;
        }

        private async Task ConfigurarMapaYActualizarRuta(string coordenadasDueño, string coordenadasPaseador)
        {
            // Dividir las coordenadas del dueño
            var dueñoCoords = coordenadasDueño.Split(',');
            var dueñoLatitude = double.Parse(dueñoCoords[0]);
            var dueñoLongitude = double.Parse(dueñoCoords[1]);

            // Dividir las coordenadas del paseador
            var paseadorCoords = coordenadasPaseador.Split(',');
            var paseadorLatitude = double.Parse(paseadorCoords[0]);
            var paseadorLongitude = double.Parse(paseadorCoords[1]);

            var dueñoLocation = new Location(dueñoLatitude, dueñoLongitude);
            var paseadorLocation = new Location(paseadorLatitude, paseadorLongitude);

            // Configurar los pines y la ruta en el mapa
            MapSpan = MapSpan.FromCenterAndRadius(dueñoLocation, Distance.FromMiles(1));
            if (MapPins == null)
            {
                MapPins = new ObservableCollection<Pin>()
                {
                new Pin { Label = "Mi ubicacion", Type = PinType.Place, Location = dueñoLocation },
                new Pin { Label = "Paseador", Type = PinType.Place, Location = paseadorLocation }
                };
        }


            RouteCoordinates = new ObservableCollection<Location>
            {
                dueñoLocation,
                paseadorLocation
            };

            if (_map != null)
            {
                await Application.Current.MainPage.Dispatcher.DispatchAsync(() =>
                {
                    _map.MoveToRegion(MapSpan);
                });
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "El mapa no está inicializado.", "OK");
            }

            // Configurar la vista inicial del mapa
            OnRouteUpdated?.Invoke(RouteCoordinates);
        }

        public Action<ObservableCollection<Location>> OnRouteUpdated;

        private async Task ActualizarUbicacionPaseador()
        {
            try
            { 
                //await Application.Current.MainPage.DisplayAlert("Exito", "Verificando nueva ubicacion, si actualiza.", "OK");
                using (var paseoDbContext = new PaseoDbContext())
                {
                    var paseo = await paseoDbContext.Paseo.FirstOrDefaultAsync(p => p.Codigo == _codigoPaseo);
            
                    if (paseo != null && (paseo.EstadoPaseo == "En camino" || paseo.EstadoPaseo == "Estoy Afuera" || paseo.EstadoPaseo == "Paseando" || paseo.EstadoPaseo == "Estoy de regreso"))
                    {
                        var paseadorCoords = paseo.CoordenadasPaseador.Split(',');
                        var paseadorLatitude = double.Parse(paseadorCoords[0]);
                        var paseadorLongitude = double.Parse(paseadorCoords[1]);

                        var paseadorLocation = new Location(paseadorLatitude, paseadorLongitude);

                        var dispatcher = Application.Current.MainPage.Dispatcher;

                        // Ejecutar el código en el hilo de la interfaz de usuario
                        await dispatcher.DispatchAsync(async () =>
                        {
                            try
                            {
                                if (paseo.EstadoPaseo == "Estoy Afuera")
                                {
                                    EstadoPaseo = paseo.EstadoPaseo;
                                    await Application.Current.MainPage.DisplayAlert("Saca a tu mascota", "El paseador se encuentra en el lugar de recogida.", "OK");
                                    
                                }


                                if (paseo.EstadoPaseo == "Estoy de regreso")
                                {
                                    EstadoPaseo = paseo.EstadoPaseo;
                                    await Application.Current.MainPage.DisplayAlert("Finalizo el paseo", "El paseador se encuentra afuera sal a recibirlo.", "OK");
                                    
                                }


                                // Verificar si MapPins no es nulo
                                if (_map != null)
                                {
                                    // Remover el pin anterior con el label "Paseador"
                                    var previousPin = MapPins.FirstOrDefault(p => p.Label == "Paseador");
                                    if (previousPin != null)
                                    {
                                        MapPins.Remove(previousPin);
                                        _map.Pins.Remove(previousPin);
                                    }

                                    var newPin = new Pin
                                    {
                                        Label = "Paseador",
                                        Type = PinType.Place,
                                        Location = paseadorLocation
                                    };
                                    MapPins.Add(newPin);
                                    _map.Pins.Add(newPin);


                                }

                                // Obtener la ruta actualizada desde la ubicación actual del paseador hasta el dueño
                                var miCoords = _map.Pins.FirstOrDefault(p => p.Label == "Mi ubicacion")?.Location;

                                if (miCoords != null)
                                {
                                    
                                    RouteCoordinates = new ObservableCollection<Location>
                                    {
                                        miCoords,
                                        paseadorLocation
                                    };

                                    RouteCoordinates = new ObservableCollection<Location>(RouteCoordinates);

                                }

                                if (_map != null)
                                {
                                    _map.MoveToRegion(MapSpan.FromCenterAndRadius(paseadorLocation, Distance.FromMiles(1)));
                                }

                                EstadoPaseo = paseo.EstadoPaseo;
                            }
                            catch (Exception ex)
                            {
                                // Capturar y mostrar cualquier error que ocurra en el hilo de la UI
                                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                            }
                        });
                    }

                    if (paseo.EstadoPaseo == "Finalizado")
                    {
                        _updateLocationTimer.Stop();

                        await Application.Current.MainPage.DisplayAlert("Exito", "Paseo finalizado.", "OK");

                        // Navegar a la vista de calificación si el estado del paseo es "Finalizado"
                        if (Application.Current.MainPage != null)
                        {
                            await Application.Current.MainPage.Navigation.PushModalAsync(new Views.CalificacionView(_codigoPaseo, paseo.PaseadorCodigo));
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Error", "No se pudo navegar a la página de calificación. MainPage es nulo.", "OK");
                        }
                    }
                }
            }catch(Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error en ActualizarUbicacionPaseador: {ex.Message}", "OK");
            }
        }

        private async Task CancelarPaseo()
        {


            bool answer = await Application.Current.MainPage.DisplayAlert("Cancelar Paseo", "Deseas cancelar el paseo ?", "Sí", "No");
            if (!answer)
            {
                return;
            }

            var paseo = await _paseodbContext.Paseo
                .Where(p => p.Codigo == _codigoPaseo)
                .OrderByDescending(p => p.Codigo)
                .FirstOrDefaultAsync();

            if (paseo != null)
            {
                _paseodbContext.Paseo.Remove(paseo);
                await _paseodbContext.SaveChangesAsync();
                await Application.Current.MainPage.Navigation.PopModalAsync();


                // Navegar de regreso a la página anterior

            }   
        }


        public async Task CargarCalificacionesAsync()
        {
            try
            {

                using (var paseoDbContext = new PaseoDbContext())
                //using (var paseadorDbContext = new PaseadorDbContext())
                {
                    // Obtén las calificaciones del paseador
                    var paseo = await paseoDbContext.Paseo.FirstOrDefaultAsync(p => p.Codigo == _codigoPaseo);
                    var calificaciones = await _calificacionDbContext.Calificacion
                    .Where(c => c.PaseadorCodigo == paseo.PaseadorCodigo)
                    .ToListAsync();

                if (calificaciones != null && calificaciones.Count > 0)
                {
                    // Convertir las calificaciones a valores numéricos
                    var valoresCalificacion = calificaciones.Select(c => ConvertirCalificacion(c.Calificacion)).ToList();

                    // Calcular la media
                    var promedio = valoresCalificacion.Average();

                    // Mostrar el resultado en el Label
                    CalificacionPromedio = $"{promedio:N2}";
                }
                else
                {
                    CalificacionPromedio = "No hay calificaciones aún.";
                }
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

