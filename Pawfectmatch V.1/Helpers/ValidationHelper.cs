using System.Text.RegularExpressions;

namespace Pawfectmatch_V._1.Helpers
{
    public static class ValidationHelper
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            // Remove all non-digit characters
            var digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());
            
            // Check if it's a valid length (7-15 digits)
            return digitsOnly.Length >= 7 && digitsOnly.Length <= 15;
        }

        public static bool IsValidAge(int age)
        {
            return age >= 0 && age <= 100;
        }

        public static bool IsValidPetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // Check if name contains only letters, spaces, and common pet name characters
            var regex = new Regex(@"^[a-zA-Z\s\-']+$");
            return regex.IsMatch(name) && name.Length <= 100;
        }

        public static bool IsValidBreed(string breed)
        {
            if (string.IsNullOrWhiteSpace(breed))
                return false;

            // Check if breed contains only letters, spaces, and common breed name characters
            var regex = new Regex(@"^[a-zA-Z\s\-']+$");
            return regex.IsMatch(breed) && breed.Length <= 100;
        }

        public static bool IsValidMicrochipNumber(string microchipNumber)
        {
            if (string.IsNullOrWhiteSpace(microchipNumber))
                return false;

            // Check if it follows the pattern MC-XXXX
            var regex = new Regex(@"^MC-\d{4}$");
            return regex.IsMatch(microchipNumber);
        }

        public static bool IsValidAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return false;

            // Basic address validation - should not be empty and reasonable length
            return address.Length >= 5 && address.Length <= 500;
        }

        public static bool IsValidDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return true; // Description is optional

            return description.Length <= 1000; // Max 1000 characters
        }

        public static string SanitizeInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            // Remove potentially dangerous characters
            return input.Replace("<", "&lt;")
                       .Replace(">", "&gt;")
                       .Replace("\"", "&quot;")
                       .Replace("'", "&#x27;")
                       .Replace("&", "&amp;");
        }

        public static bool IsValidImageFileExtension(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            
            return allowedExtensions.Contains(extension);
        }

        public static bool IsValidFileSize(long fileSize, long maxSizeInBytes = 5 * 1024 * 1024) // 5MB default
        {
            return fileSize > 0 && fileSize <= maxSizeInBytes;
        }

        public static string CapitalizeWords(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return string.Join(' ', words.Select(word =>
                char.ToUpper(word[0]) + word.Substring(1).ToLower()
            ));
        }

        public static bool IsValidDateRange(DateTime startDate, DateTime endDate)
        {
            return startDate <= endDate;
        }

        public static bool IsValidDateInPast(DateTime date)
        {
            return date <= DateTime.Now;
        }

        public static bool IsValidDateInFuture(DateTime date)
        {
            return date > DateTime.Now;
        }
    }
} 