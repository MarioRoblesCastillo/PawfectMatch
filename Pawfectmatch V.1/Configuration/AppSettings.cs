namespace Pawfectmatch_V._1.Configuration
{
    public class AppSettings
    {
        public DatabaseSettings Database { get; set; } = new();
        public FileUploadSettings FileUpload { get; set; } = new();
        public PaginationSettings Pagination { get; set; } = new();
        public EmailSettings Email { get; set; } = new();
    }

    public class DatabaseSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public int CommandTimeout { get; set; } = 30;
        public bool EnableSensitiveDataLogging { get; set; } = false;
    }

    public class FileUploadSettings
    {
        public string UploadPath { get; set; } = "uploads";
        public long MaxFileSize { get; set; } = 5 * 1024 * 1024; // 5MB
        public string[] AllowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        public int MaxImageWidth { get; set; } = 800;
        public int MaxImageHeight { get; set; } = 600;
    }

    public class PaginationSettings
    {
        public int DefaultPageSize { get; set; } = 10;
        public int MaxPageSize { get; set; } = 100;
        public int HomePagePetCount { get; set; } = 6;
        public int SearchResultsPageSize { get; set; } = 12;
    }

    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public string SmtpUsername { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
    }
} 