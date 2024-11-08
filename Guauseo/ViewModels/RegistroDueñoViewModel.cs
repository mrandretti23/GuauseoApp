using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Guauseo.DataAccess;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using System.Windows.Input;
using Guauseo.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;




namespace Guauseo.ViewModels
{
    public partial class RegistroDueñoViewModel : ObservableObject
    {

        private readonly DueñoDbContext _dbContext;

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
        private DateTime fecha;
      
        [ObservableProperty]
        private string estado;

        [ObservableProperty]
        private string errorMessage;

        public event Action<string> ShowAlert;

        public ICommand RegistroCommand { get; set; }

        public RegistroDueñoViewModel()
        {
            _dbContext = new DueñoDbContext();
            RegistroCommand = new AsyncRelayCommand(Registro);
        }

        private async Task Registro()
        {
            ErrorMessage = Validacion();

            if (ErrorMessage != null)
            {
                ShowAlert?.Invoke(ErrorMessage);
                return;
            }

            try
            {
                var usuariocreado = await _dbContext.Dueños.FirstOrDefaultAsync(u => u.Correo == Correo);
                if (usuariocreado == null)
                {
                    
                    var dueño = new DueñoModel
                    {
                    Correo = Correo,
                    Contraseña = Contraseña,
                    Nombre = Nombre,
                    Cedula = Cedula,
                    Direccion = Direccion,
                    Telefono = Telefono,
                    Fecha = DateTime.Now,
                    Estado = "OK",
                    };

                    _dbContext.Dueños.Add(dueño);
                    await _dbContext.SaveChangesAsync();

                    Correo = Contraseña = Nombre = Cedula = Direccion = Telefono = string.Empty;

                    ShowAlert?.Invoke("Usuario creado con exito");

                    await App.Current.MainPage.Navigation.PopModalAsync();
            
                }
            }
            catch (Exception)
            {
                ShowAlert?.Invoke("Ya existe un usuario con este correo electrónico");
                return;
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
