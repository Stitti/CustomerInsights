using Azure.Identity;
using CustomerInsights.EmailService.Models;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.DeviceManagement.VirtualEndpoint.ProvisioningPolicies.Item.Assignments.Item.AssignedUsers.Item;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using EmailSettings = CustomerInsights.EmailService.Models.EmailSettings;

namespace CustomerInsights.EmailService.Services
{
    public sealed class EmailSendingService
    {
        private readonly GraphServiceClient _graphClient;
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailSendingService> _logger;

        public EmailSendingService(IOptions<EmailSettings> settings, ILogger<EmailSendingService> logger)
        {
            _settings = settings.Value;
            _logger = logger;

            string[] scopes = new string[] { "https://graph.microsoft.com/.default" };

            ClientSecretCredential clientSecretCredential = new ClientSecretCredential(_settings.TenantId, _settings.ClientId, _settings.ClientSecret);

            _graphClient = new GraphServiceClient(clientSecretCredential, scopes);
        }

        public async Task<bool> SendEmailAsync(string subject, string body, string[] recipients, bool isBodyHtml, CancellationToken cancellationToken)
        {
            if (recipients == null || recipients.Length == 0)
            {
                _logger.LogError("No recipients provided for email with subject '{Subject}'", subject);
                return false; 
            }

            List<Recipient> toRecipients = recipients.Where(r => string.IsNullOrWhiteSpace(r) == false)
                                                     .Select(r => new Recipient
                                                     {
                                                         EmailAddress = new EmailAddress
                                                         {
                                                             Address = r.Trim()
                                                         }
                                                     })
                                                     .ToList();

            if (toRecipients.Any() == false)
            {
                _logger.LogError("No valid recipients found for email with subject '{Subject}'", subject);
                return false;
            }

            Message message = new Message
            {
                ToRecipients = toRecipients,
                Subject = subject,
                Body = new ItemBody
                {
                    ContentType = isBodyHtml ? BodyType.Html : BodyType.Text,
                    Content = body
                }
            };

            var user = _graphClient.Users[_settings.FromAddress];
            if (user == null)
            {
                _logger.LogError("Failed to find user with email '{FromAddress}' to send email with subject '{Subject}'", _settings.FromAddress, subject);
                return false;
            }


            try
            {
                await user.SendMail.PostAsync(new SendMailPostRequestBody { Message = message, SaveToSentItems = true }, cancellationToken: cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email with subject '{Subject}' to recipients: {Recipients}", subject, string.Join(", ", toRecipients.Select(r => r.EmailAddress?.Address)));
                return false;
            }
        }
    }
}
