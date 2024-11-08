using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using Guauseo.Services;

namespace Guauseo.Platforms.Android.Services
{
    public class AndroidFilePicker : Guauseo.Services.IFilePicker
    {
        public async Task<FileResult> PickAsync(PickOptions options = null)
        {
            return await FilePicker.Default.PickAsync(options);
        }
    }
}
