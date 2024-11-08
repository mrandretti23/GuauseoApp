using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Guauseo.DataAccess;
using Guauseo.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Guauseo.ViewModels
{
    public partial class MiPerfilDueñoViewModel:ObservableObject
    {
        private readonly DueñoDbContext _dbContext;
        //private readonly DueñoModel _usuario;
        private readonly INavigation _navigation;

        [ObservableProperty]
        private decimal codigo;

        [ObservableProperty]
        private string correo;

        [ObservableProperty]
        private string contraseña;

        [ObservableProperty]
        private string nombre;

        [ObservableProperty]
        private string cedula;

        [ObservableProperty]
        private string direccion;

        [ObservableProperty]
        private string telefono;

        [ObservableProperty]
        private string errorMessage;

        public event Action<string> ShowAlert;

        public ICommand UpdateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public MiPerfilDueñoViewModel()
        {

        }
        public MiPerfilDueñoViewModel(decimal usuario, INavigation navigation)
        {
            _dbContext = new DueñoDbContext();
            Codigo = usuario;
            _navigation = navigation;

            UpdateCommand = new AsyncRelayCommand(UpdateProfile);
            DeleteCommand = new AsyncRelayCommand(DeleteProfile);
            LoadUserData();
        }

        private async void LoadUserData()
        {
            var usuario = await _dbContext.Dueños.FirstOrDefaultAsync(u => u.Codigo == Codigo);
            if (usuario != null)
            {
                
                Correo = usuario.Correo;
                Contraseña = usuario.Contraseña;
                Nombre = usuario.Nombre;
                Cedula = usuario.Cedula;
                Direccion = usuario.Direccion;
                Telefono = usuario.Telefono;

            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Usuario no encontrado.", "OK");
            }
        }

        
        private async Task UpdateProfile()
        {
            var u = await _dbContext.Dueños.FirstOrDefaultAsync(u => u.Codigo == Codigo);
            string contraver = u.Contraseña;

            ErrorMessage = Validacion();

            if (ErrorMessage != null)
            {
                await App.Current.MainPage.DisplayAlert("Error", errorMessage, "OK");
                return;
            }

            try
            {
                var usuario = await _dbContext.Dueños.FirstOrDefaultAsync(u => u.Codigo == Codigo);
               
                if (usuario != null)
                {
                    usuario.Correo = Correo;
                    usuario.Contraseña = Contraseña;
                    usuario.Nombre = Nombre;
                    usuario.Cedula = Cedula;
                    usuario.Direccion = Direccion;
                    usuario.Telefono = Telefono;

                    //_dbContext.Dueños.Update(usuario);
                    await _dbContext.SaveChangesAsync();
                }
                if (contraver != Contraseña) 
                    {
                    await App.Current.MainPage.DisplayAlert("Exito", "Perfil actualizado con éxito", "OK");

                    await App.Current.MainPage.Navigation.PopModalAsync();
                    await App.Current.MainPage.Navigation.PopModalAsync();
                }
                else { 
                await App.Current.MainPage.DisplayAlert("Exito", "Perfil actualizado con éxito", "OK");

                await App.Current.MainPage.Navigation.PopModalAsync();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                ShowAlert?.Invoke(ErrorMessage);
            }
        }

        private async Task DeleteProfile()
        {
            bool confirm = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Está seguro de que desea eliminar su cuenta?", "Sí", "No");
            if (!confirm) return;

            try
            {
                var usuario = await _dbContext.Dueños.FirstOrDefaultAsync(u => u.Codigo == Codigo);

                if (usuario != null)
                {
                    _dbContext.Dueños.Remove(usuario);
                    await _dbContext.SaveChangesAsync();

                    await App.Current.MainPage.DisplayAlert("Exito", "Cuenta eliminada con éxito", "OK");
                    

                    // Navegar de regreso a la página de login
                    await App.Current.MainPage.Navigation.PopToRootAsync();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                await App.Current.MainPage.DisplayAlert("Error", ErrorMessage, "OK");
            }
        }

        private string Validacion()
        {
            // Valida todos las entradas no esten vacias
            if (string.IsNullOrEmpty(Correo) ||
                string.IsNullOrEmpty(Contraseña) ||
                string.IsNullOrEmpty(Nombre) ||
                string.IsNullOrEmpty(Cedula) ||
                string.IsNullOrEmpty(Direccion) ||
                string.IsNullOrEmpty(Telefono))
            {
                return "Todos los campos deben estar llenos";
            }

            // Validar email
            if (!new EmailAddressAttribute().IsValid(Correo))
                return "Formato de email no valido";

            // Validar Password
            if (Contraseña.Length < 5 || !Regex.IsMatch(Contraseña, @"[A-Z]"))
                return "La contraseña debe tener al menos 5 caracteres y contener al menos una letra mayúscula";

            // Validar Cedula
            if (Cedula.Length < 10)
                return "La cédula debe tener al menos 10 caracteres.";

            // Validar Telefono1
            if (Telefono.Length != 10)
                return "El número de teléfono debe tener exactamente 10 numeros";

            return null;
        }
    }
}
