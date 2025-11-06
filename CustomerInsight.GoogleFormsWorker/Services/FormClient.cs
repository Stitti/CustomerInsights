using Google.Apis.Forms.v1;
using Google.Apis.Forms.v1.Data;
using Google.Apis.Services;

namespace CustomerInsights.GoogleFormsWorker.Services
{
    public sealed class FormClient
    {
        public async Task<(IEnumerable<FormResponseRow>, string?)> ListResponsesAsync(string accessToken, string formId, DateTimeOffset sinceUtc, int pageSize, string? pageToken, CancellationToken ct)
        {
            FormsService service = new FormsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GoogleCredentialFromAccessToken(accessToken),
                ApplicationName = "YourSaaS-FormsIngest"
            });

            var req = service.Forms.Responses.List(formId);
            req.Filter = $"timestamp >= {sinceUtc:yyyy-MM-ddTHH:mm:ssZ}";
            req.PageSize = pageSize;
            req.PageToken = pageToken;

            var resp = await req.ExecuteAsync(ct);

            var rows = new List<FormResponseRow>();
            foreach (var r in resp.Responses ?? new List<FormResponse>())
            {
                rows.Add(new FormResponseRow(
                    ResponseId: r.ResponseId!,
                    CreateTime: Parse(r.CreateTime),
                    LastSubmittedTime: r.LastSubmittedTime is null ? null : Parse(r.LastSubmittedTime),
                    Answers: FlattenAnswers(r)
                ));
            }

            return (rows, resp.NextPageToken);

            static DateTimeOffset Parse(string iso) => DateTimeOffset.Parse(iso, null, System.Globalization.DateTimeStyles.AdjustToUniversal);

            static IEnumerable<FlatAnswer> FlattenAnswers(FormResponse r)
            {
                if (r.Answers == null || r.Answers.Count == 0)
                    yield break;

                foreach (var kv in r.Answers)
                {
                    var itemId = kv.Key;
                    var ans = kv.Value;
                    var questionText = ans.Question?.Question ?? "";

                    string? value = ans.TextAnswers?.Answers?.FirstOrDefault()?.Value
                                    ?? ans.FileUploadAnswers?.Answers?.FirstOrDefault()?.FileId
                                    ?? ans.Grade?.Grade?.ToString();

                    string? choiceValue = ans.ChoiceAnswers?.Value?.FirstOrDefault();
                    string? fileId = ans.FileUploadAnswers?.Answers?.FirstOrDefault()?.FileId;
                    string? other = ans.ChoiceAnswers?.OtherOption;

                    yield return new FlatAnswer(
                        ResponseId: r.ResponseId!,
                        ItemId: itemId,
                        QuestionText: questionText,
                        Value: value,
                        ChoiceValue: choiceValue,
                        FileId: fileId,
                        Other: other
                    );
                }
            }
        }

        private static Google.Apis.Auth.OAuth2.ICredential GoogleCredentialFromAccessToken(string accessToken)
            => Google.Apis.Auth.OAuth2.GoogleCredential.FromAccessToken(accessToken);
    }
}