using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using System.Windows.Input;
using Guauseo.DataAccess;


namespace Guauseo.ViewModels
{
    public partial class LoginPaseadorViewModel:ObservableObject
    {

        private readonly PaseadorDbContext _dbContext;
        private readonly INavigation _navigation;

        [ObservableProperty]
        private string correo;

        [ObservableProperty]
        private string contraseña;

        public event Action<string> ShowAlert;

        public ICommand LoginCommand { get; set; }

        public LoginPaseadorViewModel()
        {
           
        }

        public LoginPaseadorViewModel(INavigation navigation)
        {
            _dbContext = new PaseadorDbContext();
            _navigation = navigation;

            LoginCommand = new AsyncRelayCommand(Login);
            
        }


        private async Task Login()
        {
            var paseador = await _dbContext.Paseadores
                .FirstOrDefaultAsync(u => u.Correo == Correo && u.Contraseña == Contraseña);

            if (paseador != null)
            {
                await _navigation.PushModalAsync(new Views.InicioPaseadorView(paseador.Codigo));

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
