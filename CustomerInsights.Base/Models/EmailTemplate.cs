namespace CustomerInsights.Models.Models
{
    public sealed class EmailTemplate
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string LanguageCode { get; set; }
        public bool IsHtml { get; set; } = true;
    }

}
