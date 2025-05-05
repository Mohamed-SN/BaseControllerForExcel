using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFromExcelSheet.BLL.Interface
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(byte[] fileBytes, string extension, string folderName);

        void DeleteFile(string folderName, string imageName);


    }
    public class FileService(IWebHostEnvironment environment) : IFileService
    {



        public async Task<string> SaveFileAsync(byte[] fileBytes, string extension, string folderName)
        {
            if (fileBytes == null || fileBytes.Length == 0)
                return null;

            string uploadsFolder = Path.Combine("wwwroot", folderName);
            string uniqueFileName = Guid.NewGuid() + extension;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            Directory.CreateDirectory(uploadsFolder);

            await File.WriteAllBytesAsync(filePath, fileBytes);

            string serverBaseUrl = "https://localhost:7075/";
            string relativePath = Path.Combine(folderName, uniqueFileName);

            return $"{serverBaseUrl}{relativePath.Replace("\\", "/")}";
        }



        public void DeleteFile(string folderName, string imageName)
        {
            if (string.IsNullOrEmpty(folderName) || string.IsNullOrEmpty(imageName))
            {
                throw new ArgumentException("Folder name and image name cannot be null or empty.");
            }

            var webRootPath = Environment.CurrentDirectory;
            var folderPath = Path.Combine(webRootPath, folderName);
            var fullPath = Path.Combine(folderPath, imageName);

            // Remove the wwwroot prefix from the full path
            string relativePath = fullPath.Replace(Path.GetFullPath(webRootPath), "").TrimStart('\\');

            if (!File.Exists(relativePath))
            {
                throw new FileNotFoundException($"File not found: {relativePath}");
            }

            try
            {
                File.Delete(relativePath);
                Console.WriteLine($"File deleted successfully: {relativePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file: {ex.Message}");
                throw;
            }
        }
    }
}
