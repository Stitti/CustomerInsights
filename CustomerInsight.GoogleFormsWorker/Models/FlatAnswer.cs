public sealed class FlatAnswer
{
    public string ResponseId { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string? ChoiceValue { get; set; }
    public string? FileId { get; set; }
    public string? Other { get; set; }
}