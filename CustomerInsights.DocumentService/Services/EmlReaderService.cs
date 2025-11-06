using CustomerInsights.DocumentService.Models;
using MimeKit;

namespace CustomerInsights.DocumentService.Services
{
    public sealed class EmlReaderService
    {
        public Task<EmlMessage> ReadAsync(Stream emlStream, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var message = MimeMessage.Load(emlStream);

                string subject = message.Subject ?? string.Empty;
                string from = message.From?.ToString() ?? string.Empty;
                string[] ToArr = message.To?.Select(a => a.ToString()).ToArray() ?? Array.Empty<string>();
                string[] CcArr = message.Cc?.Select(a => a.ToString()).ToArray() ?? Array.Empty<string>();
                DateTimeOffset date = message.Date;
                string textBody = message.GetTextBody(MimeKit.Text.TextFormat.Plain) ?? string.Empty;

                return new EmlMessage
                {
                    Subject = subject,
                    From = from,
                    To = ToArr,
                    Cc = CcArr,
                    Date = date,
                    TextBody = textBody
                };
            }, cancellationToken);
        }
    }
}