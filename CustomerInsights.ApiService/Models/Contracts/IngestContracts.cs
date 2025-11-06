using CustomerInsights.Base.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CustomerInsights.ApiService.Models.Contracts;

/// <summary>
/// Request body for ingesting a new customer interaction
/// </summary>
public sealed class IngestInteractionRequest
{
    /// <summary>
    /// Unique identifier of the customer account
    /// </summary>
    [Required]
    public Guid AccountId { get; set; }

    /// <summary>
    /// Optional: Specific contact person within the account
    /// </summary>
    public Guid? ContactId { get; set; }

    /// <summary>
    /// Optional: Thread/Conversation ID to group related interactions
    /// </summary>
    public Guid? ThreadId { get; set; }

    /// <summary>
    /// Communication channel
    /// </summary>
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Channel Channel { get; set; }

    /// <summary>
    /// When this interaction occurred (ISO 8601 format)
    /// </summary>
    [Required]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// The actual text content to analyze
    /// </summary>
    [Required]
    [MinLength(1)]
    [MaxLength(50000)]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Optional: Language code (ISO 639-1, e.g. "en", "de", "fr")
    /// If not provided, will be auto-detected
    /// </summary>
    [StringLength(10)]
    public string? Language { get; set; }

    /// <summary>
    /// Optional: Subject line (for emails, tickets)
    /// </summary>
    [MaxLength(500)]
    public string? Subject { get; set; }

    /// <summary>
    /// Optional: External ID from source system (e.g. Ticket-ID, Email-ID)
    /// </summary>
    [MaxLength(200)]
    public string? ExternalId { get; set; }

    /// <summary>
    /// Optional: Metadata as key-value pairs (e.g. product, region, agent)
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }
}

public enum ProcessingStatus
{
    /// <summary>
    /// Interaction saved, waiting for analysis
    /// </summary>
    Queued = 0,

    /// <summary>
    /// Currently being analyzed
    /// </summary>
    Processing = 1,

    /// <summary>
    /// Analysis complete
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Analysis failed (will be retried)
    /// </summary>
    Failed = 3
}

/// <summary>
/// Example request payloads for different channels
/// </summary>
public static class IngestExamples
{
    public static readonly IngestInteractionRequest EmailExample = new()
    {
        AccountId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
        ContactId = Guid.Parse("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
        ThreadId = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"),
        Channel = Channel.Email,
        Timestamp = DateTime.UtcNow.AddHours(-2),
        Subject = "Issues with recent shipment",
        Text = "Hi support team, we received our order yesterday but 3 items were damaged. " +
               "This is the second time this has happened. We need a replacement ASAP as " +
               "production is delayed. Order #12345.",
        Language = "en",
        ExternalId = "email-abc123",
        Metadata = new Dictionary<string, string>
        {
            ["product"] = "industrial-pump-x200",
            ["order_id"] = "12345",
            ["priority"] = "high"
        }
    };

    public static readonly IngestInteractionRequest ReviewExample = new()
    {
        AccountId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
        Channel = Channel.Review,
        Timestamp = DateTime.UtcNow.AddDays(-1),
        Text = "Great product but shipping took way too long. " +
               "The quality is excellent but 3 weeks delivery time is unacceptable for B2B.",
        Language = "en",
        ExternalId = "google-review-xyz789",
        Metadata = new Dictionary<string, string>
        {
            ["source"] = "google",
            ["rating"] = "3"
        }
    };

    public static readonly IngestInteractionRequest TicketExample = new()
    {
        AccountId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
        ContactId = Guid.Parse("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
        ThreadId = Guid.Parse("8d0e7690-8536-51ef-b05c-f18ed2g41bf8"),
        Channel = Channel.Ticket,
        Timestamp = DateTime.UtcNow,
        Subject = "Software license not activating",
        Text = "We purchased 50 licenses yesterday but none of them are activating. " +
               "Our team cannot work. Please fix this immediately!",
        Language = "en",
        ExternalId = "ticket-5678",
        Metadata = new Dictionary<string, string>
        {
            ["ticket_system"] = "zendesk",
            ["priority"] = "urgent",
            ["category"] = "licensing"
        }
    };
}