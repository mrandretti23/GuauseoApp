using Android.App;
using Android.Content.PM;
using Android.OS;
using Android;
using Android.Runtime;
using Microsoft.Maui;

namespace Guauseo
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        const int RequestCameraId = 0;
        const int RequestStorageId = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            if ((int)Build.VERSION.SdkInt >= 23)
            {
                RequestPermissions(new string[] { Manifest.Permission.Camera }, RequestCameraId);
                RequestPermissions(new string[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage }, RequestStorageId);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (grantResults.Length > 0)
            {
                if (requestCode == RequestCameraId)
                {
                    if (grantResults[0] != Permission.Granted)
                    {
                        // La cámara no está disponible.
                        ShowPermissionDeniedAlert("Camera");
                    }
                }
                else if (requestCode == RequestStorageId)
                {
                    if (grantResults[0] != Permission.Granted)
                    {
                        // El almacenamiento no está disponible.
                        ShowPermissionDeniedAlert("Storage");
                    }
                }
            }
        }

        private void ShowPermissionDeniedAlert(string permissionType)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle("Permission Denied");
            alert.SetMessage($"{permissionType} permission is denied. Please allow the permission from settings.");
            alert.SetPositiveButton("OK", (sender, args) => { });
            alert.Create().Show();
        }
    }
}
