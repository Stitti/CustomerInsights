using System.Net.Http.Headers;
using System.Text.Json;
using CustomerInsight.GoogleReview.Models;

namespace CustomerInsight.GoogleReview.Services
{
    public sealed class BusinessProfileClient
   {
       private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

       public async Task<IReadOnlyList<GbpAccount>> ListAccountsAsync(string accessToken, CancellationToken ct)
       {
           using HttpClient http = Create(accessToken);
           string url = "https://mybusinessaccountmanagement.googleapis.com/v1/accounts";
           using HttpResponseMessage res = await http.GetAsync(url, ct);
           res.EnsureSuccessStatusCode();
           using Stream stream = await res.Content.ReadAsStreamAsync(ct);
           using JsonDocument doc = await JsonDocument.ParseAsync(stream, default, ct);

           List<GbpAccount> list = new List<GbpAccount>();
           if (doc.RootElement.TryGetProperty("accounts", out JsonElement accounts))
           {
               foreach (JsonElement account in accounts.EnumerateArray())
               {
                   list.Add(new GbpAccount
                   {
                       Name = account.GetProperty("name").GetString()!,
                       DisplayName = account.GetProperty("accountName").GetString() ?? account.GetProperty("name").GetString()!
                   });
               }
           }
           return list;
       }

       public async Task<IReadOnlyList<GbpLocation>> ListLocationsAsync(string accessToken, string accountName, CancellationToken ct)
       {
           using HttpClient http = Create(accessToken);
           string url = $"https://mybusinessbusinessinformation.googleapis.com/v1/{accountName}/locations?pageSize=100";
           using HttpResponseMessage res = await http.GetAsync(url, ct);
           res.EnsureSuccessStatusCode();
           using Stream stream = await res.Content.ReadAsStreamAsync(ct);
           using JsonDocument doc = await JsonDocument.ParseAsync(stream, default, ct);

           List<GbpLocation> list = new List<GbpLocation>();
           if (doc.RootElement.TryGetProperty("locations", out JsonElement locations))
           {
               foreach (JsonElement location in locations.EnumerateArray())
               {
                   list.Add(new GbpLocation
                   {
                       Name = location.GetProperty("name").GetString()!,
                       Title = location.GetProperty("title").GetString() ?? ""
                   });
               }
           }
           return list;
       }

       public async Task<(IEnumerable<ReviewRow>, string?)> ListReviewsAsync(string accessToken, string locationName, int pageSize, string? pageToken, CancellationToken ct)
       {
           using HttpClient http = Create(accessToken);
           // Review-Listing (klassisch v4 Endpoint):
           string url = $"https://mybusiness.googleapis.com/v4/{locationName}/reviews?pageSize={pageSize}";
           if (string.IsNullOrEmpty(pageToken) == false)
               url += $"&pageToken={Uri.EscapeDataString(pageToken)}";

           using HttpResponseMessage res = await http.GetAsync(url, ct);
           res.EnsureSuccessStatusCode();
           using Stream stream = await res.Content.ReadAsStreamAsync(ct);
           using JsonDocument doc = await JsonDocument.ParseAsync(stream, default, ct);

           List<ReviewRow> reviews = new List<ReviewRow>();
           if (doc.RootElement.TryGetProperty("reviews", out JsonElement reviewsElement))
           {
               foreach (var review in reviewsElement.EnumerateArray())
               {
                   reviews.Add(new ReviewRow
                   {
                       ReviewId = review.GetProperty("reviewId").GetString()!,
                       ReviewerDisplayName = review.GetProperty("reviewer").GetProperty("displayName").GetString() ?? "Anonymous",
                       StarRating = ParseStars(review.GetProperty("starRating").GetString()),
                       Comment = review.TryGetProperty("comment", out var c) ? c.GetString() : string.Empty,
                       CreateTime = DateTimeOffset.Parse(review.GetProperty("createTime").GetString()!),
                       UpdateTime = review.TryGetProperty("updateTime", out var u) ? DateTimeOffset.Parse(u.GetString()!) : null
                   });
               }
           }
           string? next = doc.RootElement.TryGetProperty("nextPageToken", out JsonElement npt) ? npt.GetString() : null;
           return (reviews, next);

           static int ParseStars(string? s) => s?.ToUpperInvariant() switch
           {
               "ONE" => 1, "TWO" => 2, "THREE" => 3, "FOUR" => 4, "FIVE" => 5, _ => 0
           };
       }

       private static HttpClient Create(string accessToken)
       {
           HttpClient http = new HttpClient();
           http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
           return http;
       }
   }
}
