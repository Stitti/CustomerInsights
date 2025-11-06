using CustomerInsights.DataverseWorker;
using CustomerInsights.DataverseWorker.Services;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CustomerInsight.DataverseWorker.Services
{
    public sealed class EmailService : DataverseService
    {

        public EmailService(ILogger<EmailService> logger, ServiceClient service) : base(service, logger)
        {

        }

        private async Task<IEnumerable<Entity>> RetrieveEmails(DateTime lastRun)
        {

        }
    }
}