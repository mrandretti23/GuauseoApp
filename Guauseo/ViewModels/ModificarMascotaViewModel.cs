using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Guauseo.DataAccess;
using Azure.Storage.Blobs;


namespace Guauseo.ViewModels
{
    public partial class ModificarMascotaViewModel:ObservableObject
    {
        private readonly MascotaDbContext _dbContext;
        private readonly INavigation _navigation;

        [ObservableProperty]
        private decimal dueñoCodigo;

        [ObservableProperty]
        private decimal mascotaCodigo;

        [ObservableProperty]
        private string nombre;

        [ObservableProperty]
        private string raza;

        [ObservableProperty]
        private int edad;

        [ObservableProperty]
        private string sexo;

        [ObservableProperty]
        private string tamaño;

        [ObservableProperty]
        private string agresividad;

        [ObservableProperty]
        private string necesidades;

        [ObservableProperty]
        private string foto;

        [ObservableProperty]
        private Stream fotoStream;

        [ObservableProperty]
        private string errorMessage;

        public event Action<string> ShowAlert;

        public ICommand UpdateCommand { get; set; }
        public ICommand PickPhotoCommand { get; }
        public ICommand TakePhotoCommand { get; }
        public ModificarMascotaViewModel()
        {

        }

        public ModificarMascotaViewModel(decimal dueñoCodigo, decimal codigoMascota, INavigation navigation)
        {
            _dbContext = new MascotaDbContext();
            DueñoCodigo = dueñoCodigo;
            MascotaCodigo = codigoMascota;
            _navigation = navigation;
            PickPhotoCommand = new AsyncRelayCommand(PickPhoto);
            TakePhotoCommand = new AsyncRelayCommand(TakePhoto);

            UpdateCommand = new AsyncRelayCommand(UpdateProfile);
            LoadUserData();
        }

        private async void LoadUserData()
        {
            var mascota = await _dbContext.Mascotas.FirstOrDefaultAsync(u => u.Codigo == MascotaCodigo);
            if (mascota != null)
            {

                Nombre = mascota.Nombre;
                Raza = mascota.Raza;
                Edad = mascota.Edad;
                Sexo = mascota.Sexo;
                Tamaño = mascota.Tamaño;
                Agresividad = mascota.Agresividad;
                Necesidades = mascota.Necesidades;
                Foto = mascota.Foto;

            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Mascota no encontrada.", "OK");
            }
        }

        private async Task UpdateProfile()
        {
            var u = await _dbContext.Mascotas.FirstOrDefaultAsync(u => u.Codigo == MascotaCodigo);

            ErrorMessage = Validacion();

            if (ErrorMessage != null)
            {
                await App.Current.MainPage.DisplayAlert("Error", errorMessage, "OK");
                return;
            }

            try
            {
                var mascota = await _dbContext.Mascotas.FirstOrDefaultAsync(u => u.Codigo == MascotaCodigo);

                if (mascota != null)
                {
                    mascota.Nombre = Nombre;
                    mascota.Raza = Raza;
                    mascota.Edad = Edad;
                    mascota.Sexo = Sexo;
                    mascota.Tamaño = Tamaño;
                    mascota.Agresividad = Agresividad;
                    mascota.Necesidades = Necesidades;
                    if (FotoStream != null) { 
                    mascota.Foto = await UploadFileToAzureAsync(FotoStream, $"{Nombre} - {DueñoCodigo}.jpg");
                    }
                    //_dbContext.Dueños.Update(usuario);
                    await _dbContext.SaveChangesAsync();

                    await App.Current.MainPage.DisplayAlert("Exito", "Perfil actualizado con éxito", "OK");
                    await App.Current.MainPage.Navigation.PopModalAsync();
                    await App.Current.MainPage.Navigation.PopModalAsync();
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", "error al actualizar", "OK");

                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                ShowAlert?.Invoke(ErrorMessage);
            }
        }

        private async Task PickPhoto()
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Seleccione una foto"
            });

            if (result != null)
            {
                //var stream = await result.OpenReadAsync();
                //Foto = await UploadFileToAzureAsync(stream, result.FileName);

                FotoStream = await result.OpenReadAsync();
                Foto = result.FullPath;
            }
        }

        private async Task TakePhoto()
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                var result = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Tome una foto"
                });

                if (result != null)
                {
                    //var stream = await result.OpenReadAsync();
                    //Foto = await UploadFileToAzureAsync(stream, result.FileName);

                    FotoStream = await result.OpenReadAsync();
                    Foto = result.FullPath;
                }
            }
        }
        private async Task<string> UploadFileToAzureAsync(Stream fileStream, string fileName)
        {
            // Configura BlobStorageConnectionString y BlobContainerName
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=guauseo;AccountKey=KHo2XassrZBhRtwepegyYDJGmQI3iFDxAQSOlilu5LN665dKmx7tHBAzibkt6vxFwYPPncKSxhqd+ASt5XQtLQ==;EndpointSuffix=core.windows.net";
            string containerName = "mascotas";

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(fileStream, true);

            return blobClient.Uri.ToString();
        }

        private string Validacion()
        {
            // Valida todos las entradas no esten vacias
            if (string.IsNullOrEmpty(Nombre) ||
                string.IsNullOrEmpty(Raza) ||
                string.IsNullOrEmpty(Edad.ToString()) ||
                string.IsNullOrEmpty(Sexo) ||
                string.IsNullOrEmpty(Tamaño) ||
                string.IsNullOrEmpty(Agresividad) ||
                string.IsNullOrEmpty(Necesidades))
            {
                return "Todos los campos deben estar llenos";
            }

            // Validar email
            //if (FotoStream == null)
            //    return "Agrega una foto de tu mascota";
            return null;
        }
    }
}
