using Microsoft.AspNetCore.Http;

namespace Services.File
{
    public interface IFilesService
    {
        Task<byte[]> GetFile(string filesName);
        Task<string> UploadImage(IFormFile file, string directory);
        Task<string> UploadVideo(IFormFile file,string directory);
        Task DeleteAsync(string path);
    }
}