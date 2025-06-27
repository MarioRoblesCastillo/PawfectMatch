namespace Pawfectmatch_V._1.Services
{
    public interface IEmailService
    {
        void SendEmail(string to, string subject, string body);
    }
} 