using System.IO;
using System.Threading.Tasks;

namespace Guauseo.Services
{
    public interface IMediaPicker
    {
        bool IsCameraAvailable { get;}
        Task<FileResult> CapturePhotoAsync(MediaPickerOptions options = null);
    }
}
