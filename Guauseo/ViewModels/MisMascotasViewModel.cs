using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;
using Guauseo.Models;
using Guauseo.DataAccess;
using System.Collections.ObjectModel;
using Guauseo.Views;

namespace Guauseo.ViewModels
{
    public partial class MisMascotasViewModel:ObservableObject
    {
        private readonly MascotaDbContext _dbContext;
        private readonly INavigation _navigation;

        [ObservableProperty]
        private decimal dueñoCodigo;

        [ObservableProperty]
        private decimal mascotaCodigo;

        [ObservableProperty]
        private ObservableCollection<MascotaModel> mascotas;

        public ICommand AddMascotaCommand { get; }
        public ICommand UpdateMascotaCommand { get; }

        public ICommand DeleteMascotaCommand { get; }

        public MisMascotasViewModel()
        {

        }
        public MisMascotasViewModel(decimal dueñoCodigo, INavigation navigation)
        {
            _dbContext = new MascotaDbContext();
            _navigation = navigation;
            DueñoCodigo = dueñoCodigo;
            Mascotas = new ObservableCollection<MascotaModel>();
            AddMascotaCommand = new AsyncRelayCommand(AddMascota);
            UpdateMascotaCommand = new AsyncRelayCommand<decimal>(UpdateMascota);
            DeleteMascotaCommand = new AsyncRelayCommand<decimal>(DeleteMascota);
            LoadMascotas();
        }

       

        private async void LoadMascotas()
        {
            var mascotasList = await _dbContext.Mascotas
                .Where(m => m.DueñoCodigo == DueñoCodigo)
                .ToListAsync();

            Mascotas.Clear();
            foreach (var mascota in mascotasList)
            {
                Mascotas.Add(mascota);
                //MascotaCodigo = mascota.Codigo;
            }
        }

        private async Task AddMascota()
        {
            //await _navigation.PushAsync(new RegistroMascotaView(DueñoCodigo));
            await _navigation.PushModalAsync(new Views.RegistroMascotaView(DueñoCodigo));

        }

        private async Task UpdateMascota(decimal mascotaCodigo)
        {
            //await _navigation.PushAsync(new RegistroMascotaView(DueñoCodigo));
            await _navigation.PushModalAsync(new Views.ModificarMascotaView(DueñoCodigo, mascotaCodigo ));

        }

        private async Task DeleteMascota(decimal mascotaCodigo)
        {
            bool confirm = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Está seguro de que desea eliminar a su mascota?", "Sí", "No");
            if (!confirm) return;

            try
            {
                 var mascota = await _dbContext.Mascotas.FirstOrDefaultAsync(m => m.Codigo == mascotaCodigo);
                if (mascota != null)
                {
                _dbContext.Mascotas.Remove(mascota);
                await _dbContext.SaveChangesAsync();
                Mascotas.Remove(mascota);

                await App.Current.MainPage.DisplayAlert("Exito", "Cuenta eliminada con éxito", "OK");

                }
            }
            catch (Exception ex)
            {

                await App.Current.MainPage.DisplayAlert("Error", "Error al eliminar la mascota", "OK");
            }
}
    }
}
