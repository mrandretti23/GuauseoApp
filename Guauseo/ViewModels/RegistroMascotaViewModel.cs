using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Azure.Storage.Blobs;
using System.IO;
using Guauseo.Models;
using Guauseo.DataAccess;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


namespace Guauseo.ViewModels
{
    public partial class RegistroMascotaViewModel:ObservableObject
    {
        private readonly MascotaDbContext _dbContext;
        private readonly INavigation _navigation;

        [ObservableProperty]
        private decimal dueñoCodigo;

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
        private string estado;

        [ObservableProperty]
        private Stream fotoStream;

        [ObservableProperty]
        private string errorMessage;

        public event Action<string> ShowAlert;

        public ICommand SaveCommand { get; }
        public ICommand PickPhotoCommand { get; }
        public ICommand TakePhotoCommand { get; }

        public RegistroMascotaViewModel()
        {

        }

        public RegistroMascotaViewModel(decimal dueñoCodigo, INavigation navigation)
        {
            _dbContext = new MascotaDbContext();
            _navigation = navigation;
            DueñoCodigo = dueñoCodigo;
            SaveCommand = new AsyncRelayCommand(Save);
            PickPhotoCommand = new AsyncRelayCommand(PickPhoto);
            TakePhotoCommand = new AsyncRelayCommand(TakePhoto);
        }

        private async Task Save()
        {
            ErrorMessage = Validacion();

            if (ErrorMessage != null)
            {
                ShowAlert?.Invoke(ErrorMessage);
                return;
            }

            try
            {
                var mascotacreada = await _dbContext.Mascotas.FirstOrDefaultAsync(m => m.Nombre == Nombre);
                if (mascotacreada == null)
                {
                    //if (FotoStream != null)
                    //{
                    //    Foto = await UploadFileToAzureAsync(FotoStream, $"{Nombre} - {DueñoCodigo}.jpg");
                    //}

                    var mascota = new MascotaModel
                    {
                    DueñoCodigo = DueñoCodigo,
                    Nombre = Nombre,
                    Raza = Raza,
                    Edad = Edad,
                    Sexo = Sexo,
                    Tamaño = Tamaño,
                    Agresividad = Agresividad,
                    Necesidades = Necesidades,
                    Foto = await UploadFileToAzureAsync(FotoStream, $"{Nombre} - {DueñoCodigo}.jpg"),
                    Estado = "OK",
                    };

                    _dbContext.Mascotas.Add(mascota);
                    await _dbContext.SaveChangesAsync();
                    //await _navigation.PopAsync();

                    ShowAlert?.Invoke("Macota creada con exito");

                    await App.Current.MainPage.Navigation.PopModalAsync();
                    await App.Current.MainPage.Navigation.PopModalAsync();
                }
                else
                {
                    ShowAlert?.Invoke("Ya existe una mascota con ese nombre");
                    return;
                }
            }catch (Exception) 
            {
                ShowAlert?.Invoke("Error al guardar");
                return;
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
            // Configura tu BlobStorageConnectionString y BlobContainerName
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
                    if (FotoStream == null)
                        return "Agrega una foto de tu mascota";
                    return null;
        }
    }
}
