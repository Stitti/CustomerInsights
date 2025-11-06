using CustomerInsights.DataverseWorker;
using CustomerInsights.DataverseWorker.Services;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CustomerInsight.DataverseWorker.Services
{
    public sealed class AccountService : DataverseService
    {

        public AccountService(ILogger<AccountService> logger, ServiceClient service) : base(service, logger)
        {

        }

        private async Task<IEnumerable<Entity>> RetrieveAccounts(DateTime lastRun)
        {
            ColumnSet columnSet = new ColumnSet("name", "parentcustomerid");
            List<Entity> contacts = await GetAllNewRecords("account", lastRun, columnSet);
        }
    }
}