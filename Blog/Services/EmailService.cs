using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Blog.Services;

public class SmtpSettings
{
    public string? Host { get; set; }
    public int Port { get; set; } = 587;
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? FromAddress { get; set; }
    public string FromName { get; set; } = "Blog";

    public bool IsConfigured => !string.IsNullOrWhiteSpace(Host) && !string.IsNullOrWhiteSpace(FromAddress);
}

public interface IEmailService
{
    Task SendAsync(string toAddress, string subject, string htmlBody);
}

public class SmtpEmailService(IOptions<SmtpSettings> options, IWebHostEnvironment environment, ILogger<SmtpEmailService> logger) : IEmailService
{
    private readonly SmtpSettings settings = options.Value;

    public async Task SendAsync(string toAddress, string subject, string htmlBody)
    {
        if (!settings.IsConfigured)
        {
            
            if (environment.IsDevelopment())
            {
                logger.LogWarning("SMTP ayarlı değil, e-posta gönderilmedi. Alıcı: {To}, Konu: {Subject}\n{Body}", toAddress, subject, htmlBody);
                return;
            }
            throw new InvalidOperationException("SMTP ayarları (Smtp:Host, Smtp:FromAddress) yapılandırılmamış.");
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(settings.FromName, settings.FromAddress));
        message.To.Add(MailboxAddress.Parse(toAddress));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new SmtpClient();
        var socketOptions = settings.Port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
        await client.ConnectAsync(settings.Host, settings.Port, socketOptions);
        if (!string.IsNullOrWhiteSpace(settings.UserName))
        {
            await client.AuthenticateAsync(settings.UserName, settings.Password);
        }
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
