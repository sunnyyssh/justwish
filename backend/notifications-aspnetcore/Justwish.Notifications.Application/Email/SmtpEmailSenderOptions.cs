using System.Security;

namespace Justwish.Notifications.Application;

public class SmtpEmailSenderOptions
{
    public string Password { get; set; } = string.Empty;

    public string NoreplyEmail { get; set; } = string.Empty;

    public int Port { get; set; }

    public string Host { get; set; } = string.Empty;

    public string FromName { get; set; } = string.Empty;
}
