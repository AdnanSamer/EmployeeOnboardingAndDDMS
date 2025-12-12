using EmployeeOnboarding_DDMS.Aplication.Interfaces;

namespace EmployeeOnboarding_DDMS.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _basePath;

        public FileStorageService()
        {
            _basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            
            // Ensure wwwroot directory exists
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderPath)
        {
            var fullFolderPath = Path.Combine(_basePath, folderPath);
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(fullFolderPath))
            {
                Directory.CreateDirectory(fullFolderPath);
            }

            var filePath = Path.Combine(fullFolderPath, fileName);
            
            using (var fileStreamOutput = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fileStreamOutput);
            }

            // Return relative path for database storage
            return Path.Combine(folderPath, fileName).Replace("\\", "/");
        }

        public async Task<byte[]> GetFileAsync(string filePath)
        {
            var fullPath = Path.Combine(_basePath, filePath);
            
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            return await File.ReadAllBytesAsync(fullPath);
        }

        public Task<bool> DeleteFileAsync(string filePath)
        {
            var fullPath = Path.Combine(_basePath, filePath);
            
            if (!File.Exists(fullPath))
            {
                return Task.FromResult(false);
            }

            File.Delete(fullPath);
            return Task.FromResult(true);
        }

        public Task<string> GetFileUrlAsync(string filePath)
        {
            // Return relative URL path
            var urlPath = filePath.Replace("\\", "/");
            if (!urlPath.StartsWith("/"))
            {
                urlPath = "/" + urlPath;
            }
            return Task.FromResult(urlPath);
        }
    }
}
