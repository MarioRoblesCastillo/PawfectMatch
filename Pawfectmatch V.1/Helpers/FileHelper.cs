using System.Drawing;
using System.Drawing.Imaging;

namespace Pawfectmatch_V._1.Helpers
{
    public static class FileHelper
    {
        public static async Task<string> SaveImageAsync(IFormFile file, string uploadPath, string fileName = "")
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            // Create directory if it doesn't exist
            Directory.CreateDirectory(uploadPath);

            // Generate unique filename if not provided
            if (string.IsNullOrEmpty(fileName))
            {
                var extension = Path.GetExtension(file.FileName);
                fileName = $"{Guid.NewGuid()}{extension}";
            }

            var filePath = Path.Combine(uploadPath, fileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }

        public static bool IsValidImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            // Check file extension
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(extension))
                return false;

            // Check file size (max 5MB)
            if (file.Length > 5 * 1024 * 1024)
                return false;

            return true;
        }

        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch
                {
                    // Log error in production
                }
            }
        }

        public static string GetFileSizeString(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        public static string GetUniqueFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            return $"{Guid.NewGuid()}{extension}";
        }

        public static bool IsValidFileExtension(string fileName, string[] allowedExtensions)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }

        public static bool IsValidFileSize(long fileSize, long maxSizeInBytes = 5 * 1024 * 1024) // 5MB default
        {
            return fileSize > 0 && fileSize <= maxSizeInBytes;
        }
    }
} 