using Models.Dtos.Email;

namespace Services.File
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequest request);
    }
}