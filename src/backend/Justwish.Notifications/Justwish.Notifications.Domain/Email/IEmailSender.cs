using Ardalis.Result;

namespace Justwish.Notifications.Domain;

public interface IEmailSender
{
    public Result SendAsync(EmailMessage message);
}
