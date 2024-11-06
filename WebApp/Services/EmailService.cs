using COCOApp.Models;
using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

public class EmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(EmailSettings emailSettings)
    {
        _emailSettings = emailSettings;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("CoCo", _emailSettings.FromEmail));
        emailMessage.To.Add(new MailboxAddress("", toEmail));
        emailMessage.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = message };
        emailMessage.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            client.Connect(_emailSettings.SMTPServer, _emailSettings.SMTPPort, MailKit.Security.SecureSocketOptions.StartTls);
            client.Authenticate(_emailSettings.SMTPUser, _emailSettings.SMTPPassword);

            await client.SendAsync(emailMessage);
            client.Disconnect(true);
        }
    }
}
