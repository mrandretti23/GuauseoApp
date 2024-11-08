using System.IO;
using System.Threading.Tasks;

namespace Guauseo.Services
{
    public interface IFilePicker
    {
        Task<FileResult> PickAsync(PickOptions options = null);
    }
}
