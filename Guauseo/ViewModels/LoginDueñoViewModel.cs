using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Guauseo.DataAccess;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Guauseo.ViewModels
{
    public partial class LoginDueñoViewModel : ObservableObject
    {

        private readonly DueñoDbContext _dbContext;
        private readonly INavigation _navigation;

        [ObservableProperty]
        private string correo;

        [ObservableProperty]
        private string contraseña;

        //[ObservableProperty]
        //private string errorMessage;

        public event Action<string> ShowAlert;

        public ICommand LoginCommand { get; set; }

        public LoginDueñoViewModel()
        {
            //_dbContext = new PruebaDbContext();
            //LoginCommand = new AsyncRelayCommand(Login);
        }

        public LoginDueñoViewModel(INavigation navigation)
        {
            _dbContext = new DueñoDbContext();
            _navigation = navigation;

            LoginCommand = new AsyncRelayCommand(Login);
            //LoginCommand = new Command(async () => await
            //    Login());
        }


        private async Task Login()
        {
            var user = await _dbContext.Dueños
                .FirstOrDefaultAsync(u => u.Correo == Correo && u.Contraseña == Contraseña);

            if (user != null)
            {
                await _navigation.PushModalAsync(new Views.InicioDueñoView(user.Codigo));

            }
            else
            {
                //ErrorMessage = "Usuario incorrecto o no existe";
                ShowAlert?.Invoke("Usuario o contraseña incorrecta");
                return;
            }
        }

    }
}
