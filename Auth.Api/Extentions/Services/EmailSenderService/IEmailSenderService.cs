namespace Auth.Api.Extentions.Services.EmailSenderService
{
    public interface IEmailSenderService
    {
        Task SendEmail(string receptor, string subject, string body);
    }
}
