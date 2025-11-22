namespace CustomerInsights.Models.Models.Requests
{
    public sealed class EmailJobMessage
    {
        public string TemplateKey { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string LanguageCode { get; set; } = "en";
        public string[] Recipients { get; set; } = Array.Empty<string>();
        public Dictionary<string, object> Model { get; set; } = new Dictionary<string, object>();
    }

}
