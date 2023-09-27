using Microsoft.Extensions.Options;

using MimeKit;
using MailKit.Net.Smtp;

using Passport.Api.Options;

namespace Passport.Api.Services;

public class EmailService : IEmailService
{
    private readonly EmailOptions _emailOptions;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<EmailOptions> emailOptions, 
        ILogger<EmailService> logger)
    {
        _emailOptions = emailOptions.Value;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string nick, string email, string subject, string message)
    {
        try
        {
            using var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Library.Support", _emailOptions.Email));
            emailMessage.To.Add(new MailboxAddress(nick, email));
            emailMessage.Subject = subject;

            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_emailOptions.Host, _emailOptions.Port, true);

                await client.AuthenticateAsync(_emailOptions.Email, _emailOptions.Password);

                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error with attemping send message to email. Ex: {ex}");

            return false;
        }
    }
}
