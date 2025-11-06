using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CustomerInsights.DataverseWorker.Services
{
    public class DataverseService
    {
        protected readonly ServiceClient _service;
        protected readonly ILogger<DataverseService> _logger;

        public DataverseService(ServiceClient service, ILogger<DataverseService> logger)
        {
            _service = service;
            _logger = logger;
        }

        protected async Task<List<Entity>> GetAllNewRecords(string entityName, DateTime lastRun, ColumnSet columnSet)
        {
            FilterExpression filter = new FilterExpression(LogicalOperator.And);
            filter.AddCondition(new ConditionExpression("modifiedon", ConditionOperator.GreaterThan, lastRun));
            filter.AddCondition(new ConditionExpression("statecode", ConditionOperator.Equal, 0));
            QueryExpression query = new QueryExpression
            {
                EntityName = entityName,
                Criteria = filter,
                ColumnSet = columnSet,
                PageInfo = new PagingInfo
                {
                    Count = 5000,
                    ReturnTotalRecordCount = true,
                    PageNumber = 1
                }
            };

            EntityCollection entityCollection;
            List<Entity> entities = new List<Entity>();
            int count = 0;

            do
            {
                try
                {
                    entityCollection = await _service.RetrieveMultipleAsync(query);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    return new List<Entity>();
                }

                entities.AddRange(entityCollection.Entities);
                query.PageInfo.PageNumber = query.PageInfo.PageNumber + 1;
                query.PageInfo.PagingCookie = entityCollection.PagingCookie;
                count += entityCollection.Entities.Count;
            }
            while (entityCollection.Entities.Count == 5000);

            _logger.LogInformation("EntityName: {EntityName}; TotalCounts: {Count}", "contact", count);
            return entities;
        }
    }
}

