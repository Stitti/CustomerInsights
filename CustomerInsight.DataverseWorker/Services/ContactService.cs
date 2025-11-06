using CustomerInsights.DataverseWorker;
using CustomerInsights.DataverseWorker.Services;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CustomerInsight.DataverseWorker.Services
{
    public sealed class ContactService : DataverseService
    {

        public ContactService(ILogger<ContactService> logger, ServiceClient service) : base(service, logger)
        {

        }

        private async Task<IEnumerable<Entity>> RetrieveContacts(DateTime lastRun)
        {
            ColumnSet columnSet = new ColumnSet("firstname", "lastname", "telephone1", "emailaddress1", "parentcustomerid");
            List<Entity> contacts = await GetAllNewRecords("contact", lastRun, columnSet);
        }
    }
}