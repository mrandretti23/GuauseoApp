using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Media;
using Guauseo.Services;

namespace Guauseo.Platforms.Android.Services
{
    public class AndroidMediaPicker : Guauseo.Services.IMediaPicker
    {
        public bool IsCameraAvailable => MediaPicker.Default.IsCaptureSupported;

        public async Task<FileResult> CapturePhotoAsync(MediaPickerOptions options = null)
        {
            return await MediaPicker.Default.CapturePhotoAsync(options);
        }
    }
}
