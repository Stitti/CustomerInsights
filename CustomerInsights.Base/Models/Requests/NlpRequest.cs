namespace CustomerInsights.Base.Models.Requests
{
    public sealed class NlpRequest
    {
        public string Text { get; set; } = "";
        public string? HintLanguage { get; set; } // "de","en",...
        public string? Channel { get; set; }      // optional: Email/Ticket/Survey ...
    }
}

