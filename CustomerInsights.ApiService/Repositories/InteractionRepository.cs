using CustomerInsights.ApiService.Models.Contracts;
using CustomerInsights.ApiService.Models.DTOs;
using CustomerInsights.ApiService.Models.Enums;
using CustomerInsights.ApiService.Patching;
using CustomerInsights.Database;
using CustomerInsights.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerInsights.ApiService.Database.Repositories;

public sealed class InteractionRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<InteractionRepository> _logger;

    public InteractionRepository(AppDbContext db, ILogger<InteractionRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<InteractionDto?> GetInteractionByIdAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
            return null;

        InteractionDto? entity = await _db.Interactions
            .AsNoTracking()
            .AsSplitQuery()
            .Include(i => i.TextInference)
                .ThenInclude(ti => ti.Aspects)
            .Include(i => i.TextInference)
                .ThenInclude(ti => ti.Emotions)
            .Include(i => i.Account)
            .Include(i => i.Contact)
            .Select(i => new InteractionDto
            {
                Id = i.Id,
                ExternalId = i.ExternalId,
                Subject = i.Subject,
                Channel = i.Channel,
                Source = i.Source,
                AnalyzedAt = i.AnalyzedAt,
                OccurredAt = i.OccurredAt,
                Text = i.Text,
                Account = i.Account != null ? new AccountListDto { Id = i.Account.Id, Name = i.Account.Name } : null,
                Contact = i.Contact != null ? new ContactListDto { Id = i.Contact.Id, Firstname = i.Contact.Firstname, Lastname = i.Contact.Lastname } : null,
                TextInference = i.TextInference != null ? new TextInferenceDto
                {
                    InferredAt = i.TextInference.InferredAt,
                    Sentiment = i.TextInference.Sentiment,
                    SentimentScore = i.TextInference.SentimentScore,
                    Urgency = i.TextInference.Urgency,
                    UrgencyScore = i.TextInference.UrgencyScore,
                    Aspects = i.TextInference.Aspects,
                    Emotions = i.TextInference.Emotions,
                } : null
            })
            .SingleOrDefaultAsync(i => i.Id == id, ct);

        return entity;
    }

    public async Task<IEnumerable<InteractionListDto>> GetAllInteractionsAsync(CancellationToken ct)
    {
        List<InteractionListDto> list = await _db.Interactions
            .AsNoTracking()
            .AsSingleQuery()
            .OrderByDescending(i => i.OccurredAt)
            .Include(i => i.TextInference)
            .Include(i => i.TextInference)
            .Include(i => i.Account)
            .Include(i => i.Contact)
            .Select(i => new InteractionListDto
            {
                Id = i.Id,
                ExternalId = i.ExternalId,
                Subject = i.Subject,
                Channel = i.Channel,
                Source = i.Source,
                AnalyzedAt = i.AnalyzedAt,
                OccurredAt = i.OccurredAt,
                Account = i.Account != null ? new AccountListDto { Id = i.Account.Id, Name = i.Account.Name } : null,
                Contact = i.Contact != null ? new ContactListDto { Id = i.Contact.Id, Firstname = i.Contact.Firstname, Lastname = i.Contact.Lastname } : null,
                TextInference = i.TextInference != null ? new TextInferenceListDto
                {
                    InferredAt = i.TextInference.InferredAt,
                    Sentiment = i.TextInference.Sentiment,
                    SentimentScore = i.TextInference.SentimentScore,
                    Urgency = i.TextInference.Urgency,
                    UrgencyScore = i.TextInference.UrgencyScore,
                } : null
            })
            .ToListAsync(ct);

        return list;
    }

    public async Task<Guid> InsertAsync(Interaction interaction, CancellationToken ct)
    {
        if (interaction.Id == Guid.Empty)
            interaction.Id = Guid.NewGuid();

        if (interaction.OccurredAt == default)
            interaction.OccurredAt = DateTime.UtcNow;

        _db.Interactions.Add(interaction);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Inserted interaction {Id}", interaction.Id);
        return interaction.Id;
    }

    public async Task<Guid[]> InsertBatchAsync(IEnumerable<Interaction> interactions, CancellationToken ct)
    {
        List<Interaction> list = interactions?.ToList() ?? new List<Interaction>();

        foreach (Interaction i in list)
        {
            if (i.Id == Guid.Empty)
                i.Id = Guid.NewGuid();

            if (i.OccurredAt == default)
                i.OccurredAt = DateTime.UtcNow;
        }

        await _db.Interactions.AddRangeAsync(list, ct);
        await _db.SaveChangesAsync(ct);

        Guid[] ids = list.Select(i => i.Id).ToArray();
        _logger.LogInformation("Inserted {Count} interactions", ids.Length);
        return ids;
    }

    public async Task<IReadOnlyList<ChannelCount>> GetTopChannelsAsync(TimeInterval interval, CancellationToken ct)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        (DateTimeOffset from, DateTimeOffset to) = GetIntervalRange(interval, now);

        IQueryable<Interaction> query = _db.Interactions
            .AsNoTracking()
            .Where(i => i.OccurredAt >= from && i.OccurredAt < to);

        List<ChannelCount> result = await query
            .GroupBy(i => i.Channel)
            .Select(g => new ChannelCount
            {
                Channel = g.Key,
                InteractionCount = g.Count()
            })
            .OrderByDescending(x => x.InteractionCount)
            .Take(5)
            .ToListAsync(ct);

        return result;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        int affectedRows = await _db.Interactions
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return affectedRows > 0;
    }

    public async Task<bool> PatchAsync(Guid id, UpdateInteractionRequest request, CancellationToken cancellationToken)
    {
        Interaction? interaction = await _db.Interactions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (interaction == null)
            return false;

        interaction.ApplyPatch(request);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }


    private DateTimeOffset StartOfWeek(DateTimeOffset date)
    {
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-1 * diff).Date;
    }

    private DateTimeOffset StartOfMonth(DateTimeOffset date)
    {
        return new DateTimeOffset(date.Year, date.Month, 1, 0, 0, 0, date.Offset);
    }

    private DateTimeOffset StartOfYear(DateTimeOffset date)
    {
        return new DateTimeOffset(date.Year, 1, 1, 0, 0, 0, date.Offset);
    }

    private (DateTimeOffset Start, DateTimeOffset End) GetIntervalRange(TimeInterval interval, DateTimeOffset now)
    {
        return interval switch
        {
            TimeInterval.ThisWeek => (StartOfWeek(now), now),
            TimeInterval.ThisMonth => (StartOfMonth(now), now),
            TimeInterval.ThisYear => (StartOfYear(now), now),
            TimeInterval.LastWeek => (StartOfWeek(now.AddDays(-7)), StartOfWeek(now)),
            TimeInterval.LastMonth => (StartOfMonth(now.AddMonths(-1)), StartOfMonth(now)),
            TimeInterval.LastYear => (StartOfYear(now.AddYears(-1)), StartOfYear(now)),
            _ => throw new ArgumentException($"Ungültiges Intervall: {interval}")
        };
    }
}