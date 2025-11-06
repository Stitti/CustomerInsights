public sealed class FormResponseRow
{
    public string ResponseId { get; set; } = string.Empty;
    public DateTimeOffset CreateTime { get; set; }
    public DateTimeOffset? LastSubmittedTime { get; set; }
    public IEnumerable<FlatAnswer> Answers { get; set; } = Enumerable.Empty<FlatAnswer>();
}