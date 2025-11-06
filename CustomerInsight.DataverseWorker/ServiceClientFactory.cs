using Microsoft.PowerPlatform.Dataverse.Client;

namespace CustomerInsights.DataverseWorker
{
    public static class ServiceClientFactory
    {
        public static ServiceClient CreateService(string orgUrl, string tenantId, string clientId, string clientSecret, ILogger logger)
        {
            string connectionString = $"AuthType=ClientSecret;Url={orgUrl};ClientId={clientId};ClientSecret={clientSecret};TenantId={tenantId};";
            ServiceClient serviceClient = new ServiceClient(connectionString, logger);
            return serviceClient;
        }
    }
}

