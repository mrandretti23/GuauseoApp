using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
using Guauseo.Models;
using Guauseo.DataAccess;
using Guauseo.Views;
using Microsoft.Maui.Controls;
using Microsoft.EntityFrameworkCore;

namespace Guauseo.ViewModels
{
    public partial class SolicitarPaseoViewModel : ObservableObject
    {
        private readonly INavigation _navigation;
        private readonly MascotaDbContext _dbContext;
        private readonly PaseoDbContext _paseoDbContext;

        [ObservableProperty]
        private decimal dueñoCodigo;

        [ObservableProperty]
        private ObservableCollection<MascotaModel> mascotas;

        [ObservableProperty]
        private ObservableCollection<MascotaModel> mascotasSeleccionables;

        [ObservableProperty]
        private ObservableCollection<MascotaModel> mascotasSeleccionadas;

        [ObservableProperty]
        private ObservableCollection<string> tiemposDePaseo;

        [ObservableProperty]
        private string tiempoSeleccionado;

        [ObservableProperty]
        private string ubicacionCoordenadas;

        [ObservableProperty]
        private string ubicacionNombre;

        [ObservableProperty]
        private string valorAPagar;

        [ObservableProperty]
        private DateTime fecha;


        public ICommand AbrirMapaCommand { get; }
        public ICommand ConfirmarPaseoCommand { get; }

        public SolicitarPaseoViewModel()
        {

        }

        public SolicitarPaseoViewModel(decimal dueñoCodigo, INavigation navigation)
        {
            _dbContext = new MascotaDbContext();
            _paseoDbContext = new PaseoDbContext();
            _navigation = navigation;
            DueñoCodigo = dueñoCodigo;
            LoadMascotas();
            TiemposDePaseo = new ObservableCollection<string> { "30 minutos", "1 hora", "2 horas" };
            MascotasSeleccionadas = new ObservableCollection<MascotaModel>();

            AbrirMapaCommand = new AsyncRelayCommand(AbrirMapa);
            ConfirmarPaseoCommand = new AsyncRelayCommand(ConfirmarPaseo);
        }

        private void LoadMascotas()
        {
            // Cargar las mascotas del dueño desde la base de datos
            Mascotas = new ObservableCollection<MascotaModel>(_dbContext.Mascotas.Where(m => m.DueñoCodigo == DueñoCodigo).ToList());
            MascotasSeleccionables = new ObservableCollection<MascotaModel>(Mascotas);
        }

        private async Task AbrirMapa()
        {
            try
            {
                var googleMapView = new GoogleMapView();
                var googleMapViewModel = (GoogleMapViewModel)googleMapView.BindingContext;
                googleMapViewModel.LocationSelected = (location, address) =>
                {
                    UbicacionNombre = address;
                    UbicacionCoordenadas = $"{location.Latitude}, {location.Longitude}";
                };

                await _navigation.PushModalAsync(googleMapView);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AbrirMapa: {ex}");
                await Application.Current.MainPage.DisplayAlert("Error", "Ocurrió un error al abrir el mapa.", "OK");
            }
        }

        private void CalcularValorAPagar()
        {
            int numeroMascotas = MascotasSeleccionadas.Count;
            double valor = 0;

            if (numeroMascotas == 1)
            {
                if (TiempoSeleccionado == "30 minutos") valor = 5;
                else if (TiempoSeleccionado == "1 hora") valor = 10;
                else if (TiempoSeleccionado == "2 horas") valor = 20;
            }
            else if (numeroMascotas == 2)
            {
                if (TiempoSeleccionado == "30 minutos") valor = 7.5;
                else if (TiempoSeleccionado == "1 hora") valor = 15;
                else if (TiempoSeleccionado == "2 horas") valor = 30;
            }

            ValorAPagar = $"Valor a pagar (efectivo): ${valor}";
        }

        private async Task ConfirmarPaseo()
        {

            if (MascotasSeleccionadas.Count == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se han seleccionado mascotas.", "OK");
                return;
            }

            if (MascotasSeleccionadas.Count > 2)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Solo puedes seleccionar hasta dos mascotas.", "OK");
                return;
            }

            CalcularValorAPagar();

            bool answer = await Application.Current.MainPage.DisplayAlert("Confirmar Paseo", ValorAPagar, "Sí", "No");
            if (!answer)
            {
                return;
            }

            // Crear un nuevo registro de paseo en la base de datos
            var paseo = new PaseoModel
            {
                DueñoCodigo = DueñoCodigo,
                UbicacionNombre = UbicacionNombre,
                CoordenadasDueño = UbicacionCoordenadas,
                CodigoMascota1 = MascotasSeleccionadas[0].Codigo,
                CodigoMascota2 = (MascotasSeleccionadas.Count > 1 ? MascotasSeleccionadas[1].Codigo : null),
                ValorPagar = ValorAPagar,
                EstadoPaseo = "Buscando",
                TiempoSeleccionado = TiempoSeleccionado,
                Fecha = DateTime.Now,

            };

            try
            { 
            _paseoDbContext.Paseo.Add(paseo);
            await _paseoDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar paseo: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Ocurrió un error al guardar el paseo.", "OK");
                return;
            }

            // Obtener el último registro de paseo creado por el dueño actual
            var ultimoPaseo = await _paseoDbContext.Paseo
                .Where(p => p.DueñoCodigo == DueñoCodigo)
                .OrderByDescending(p => p.Codigo)
                .FirstOrDefaultAsync();

            if (ultimoPaseo == null)
            {
                // Manejar el caso donde no se encontró ningún paseo para el dueño actual
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo encontrar el paseo recién creado.", "OK");
                return;
            }

            // Obtener el ID del nuevo registro de paseo creado
            decimal nuevoPaseoId = ultimoPaseo.Codigo;

            //var nombresMascotas = MascotasSeleccionadas.Select(m => m.Nombre).ToList();

            // Navegar a la vista de estado de paseo pasando el ID del nuevo registro
            await _navigation.PushModalAsync(new VisualizarEstadoPaseoView(nuevoPaseoId));
        }

        public void UpdateSelectedMascotas(List<MascotaModel> selectedMascotas)
        {
            MascotasSeleccionadas.Clear();
            foreach (var mascota in selectedMascotas)
            {
                MascotasSeleccionadas.Add(mascota);
            }
        }
    }
}
