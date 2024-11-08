using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Guauseo.DataAccess;
using CommunityToolkit.Mvvm.Input;
using Guauseo.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace Guauseo.ViewModels
{
    public partial class CalificacionViewModel : ObservableObject
    {
        private readonly PaseoDbContext _paseodbContext;
        private readonly PaseadorDbContext _paseadorDbContext;
        private readonly MascotaDbContext _mascotaDbContext;
        private readonly CalificacionDbContext _calificacionDbContext;

        private readonly decimal _codigoPaseo;
        private readonly decimal? _codigoPaseador;

        [ObservableProperty]
        private string ubicacionNombre;

        [ObservableProperty]
        private string nombreMascota1;

        [ObservableProperty]
        private string nombreMascota2;

        [ObservableProperty]
        private string tiempoPaseo;

        [ObservableProperty]
        private string estadoPaseo;

        [ObservableProperty]
        private string valorPagar;

        [ObservableProperty]
        private string nombrePaseador;

        [ObservableProperty]
        private string selectedPatitas;

        [ObservableProperty]
        private string comentario;

        public IAsyncRelayCommand CalificarCommand { get; }
        public IAsyncRelayCommand AhoraNoCommand { get; }

        public CalificacionViewModel()
        {

        }

        public CalificacionViewModel(decimal codigoPaseo, decimal? codigoPaseador)
        {
            _paseodbContext = new PaseoDbContext();
            _paseadorDbContext = new PaseadorDbContext();
            _mascotaDbContext = new MascotaDbContext();
            _calificacionDbContext = new CalificacionDbContext();

            _codigoPaseo = codigoPaseo;
            _codigoPaseador = codigoPaseador;

            // Cargar los datos del paseo y paseador
            CargarDatosPaseoAsync(codigoPaseo, codigoPaseador);
            CalificarCommand = new AsyncRelayCommand(Calificar);
            AhoraNoCommand = new AsyncRelayCommand(AhoraNo);
        }

        private async Task CargarDatosPaseoAsync(decimal codigoPaseo, decimal? codigoPaseador)
        {
            var paseo = await _paseodbContext.Paseo.FirstOrDefaultAsync(p => p.Codigo == codigoPaseo);

            if (paseo != null)
            {
                UbicacionNombre = paseo.UbicacionNombre;
                TiempoPaseo = paseo.TiempoSeleccionado;
                EstadoPaseo = paseo.EstadoPaseo;
                ValorPagar = paseo.ValorPagar.ToString();


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

                var paseador = await _paseadorDbContext.Paseadores.FirstOrDefaultAsync(p => p.Codigo == codigoPaseador);

                if (paseador != null)
                {
                    NombrePaseador = paseador.Nombre;
                }
            }
        }


        public async Task AhoraNo()
        {
            bool confirm = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Está seguro de no dejar tu calificación?", "Sí", "No");
            if (!confirm) return;


            await App.Current.MainPage.Navigation.PopToRootAsync();

        }

        private async Task Calificar()
        {
            if (string.IsNullOrEmpty(SelectedPatitas) || string.IsNullOrEmpty(Comentario))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Debe seleccionar una calificación y agregar un comentario.", "OK");
                return;
            }
            try
            { 
                var nuevaCalificacion = new CalificacionModel
                {
                    PaseadorCodigo = _codigoPaseador,
                    PaseoCodigo = _codigoPaseo,
                    Calificacion = SelectedPatitas,
                    Comentario = Comentario,
                    Fecha = DateTime.Now,
                };

                _calificacionDbContext.Calificacion.Add(nuevaCalificacion);
                await _calificacionDbContext.SaveChangesAsync();

                await Application.Current.MainPage.DisplayAlert("Éxito", "La calificación ha sido guardada.", "OK");
                await App.Current.MainPage.Navigation.PopToRootAsync();
            }
            catch (DbUpdateException ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message;
                if (innerExceptionMessage != null)
                {
                    Console.WriteLine("Inner Exception: " + innerExceptionMessage);
                    await Application.Current.MainPage.DisplayAlert("Error", $"Error interno: {innerExceptionMessage}", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
                }
            }
        }

    }
}
