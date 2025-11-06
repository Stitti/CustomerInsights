using CustomerInsight.GoogleAnalyticsWorker.Models;
using Google.Analytics.Data.V1;
using Google.Apis.Auth.OAuth2;
using Grpc.Auth;

namespace CustomerInsight.GoogleAnalyticsWorker.Services
{
    public sealed class GaDataClient
    {
        public async Task<IEnumerable<GaRow>> GetDailyReportAsync(string accessToken, string propertyId, DateOnly day, CancellationToken ct)
        {
            // Access-Token des MANDANTEN verwenden
            var channelCreds = GoogleCredential.FromAccessToken(accessToken).ToChannelCredentials();

            // Nutze V1, fallback auf V1Beta, je nach Paketverfügbarkeit
            try
            {
                AnalyticsDataClientBuilder client = new AnalyticsDataClientBuilder
                {
                    ChannelCredentials = channelCreds
                }.Build();

                RunReportRequest req = new RunReportRequest
                {
                    Property = $"properties/{propertyId.Value}",
                    Dimensions = { new Dimension { Name = "date" }, new Dimension { Name = "country" } },
                    Metrics = { new Metric { Name = "sessions" }, new Metric { Name = "totalUsers" } },
                    DateRanges = { new DateRange { StartDate = day.ToString("yyyy-MM-dd"), EndDate = day.ToString("yyyy-MM-dd") } },
                    Limit = 100000
                };

                var res = await client.RunReportAsync(req, cancellationToken: ct);
                return res.Rows.Select(r => new GaRow(
                    Date: r.DimensionValues[0].Value,
                    Country: r.DimensionValues[1].Value,
                    Sessions: long.Parse(r.MetricValues[0].Value),
                    TotalUsers: long.Parse(r.MetricValues[1].Value)
                ));
            }
            catch (System.TypeLoadException)
            {
                // Falls nur V1Beta verfügbar ist
                var client = new BetaAnalyticsDataClientBuilder
                {
                    ChannelCredentials = channelCreds
                }.Build();

                var req = new Google.Analytics.Data.V1Beta.RunReportRequest
                {
                    Property = $"properties/{propertyId.Value}",
                    Dimensions = { new Google.Analytics.Data.V1Beta.Dimension { Name = "date" }, new Google.Analytics.Data.V1Beta.Dimension { Name = "country" } },
                    Metrics = { new Google.Analytics.Data.V1Beta.Metric { Name = "sessions" }, new Google.Analytics.Data.V1Beta.Metric { Name = "totalUsers" } },
                    DateRanges = { new Google.Analytics.Data.V1Beta.DateRange { StartDate = day.ToString("yyyy-MM-dd"), EndDate = day.ToString("yyyy-MM-dd") } },
                    Limit = 100000
                };

                var res = await client.RunReportAsync(req, cancellationToken: ct);
                return res.Rows.Select(r => new GaRow(
                    Date: r.DimensionValues[0].Value,
                    Country: r.DimensionValues[1].Value,
                    Sessions: long.Parse(r.MetricValues[0].Value),
                    TotalUsers: long.Parse(r.MetricValues[1].Value)
                ));
            }
        }
    }
}