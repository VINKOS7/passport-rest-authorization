namespace Passport.Api.Services;

public interface IEmailService
{
    public Task<bool> SendEmailAsync(string nick, string email, string subject, string message);
}
