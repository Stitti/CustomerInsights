using CustomerInsights.ApiService.Models;
using CustomerInsights.ApiService.Models.DTOs;
using CustomerInsights.ApiService.Models.Enums;
using CustomerInsights.Base.Models;
using CustomerInsights.Database;
using CustomerInsights.Models;
using Microsoft.EntityFrameworkCore;

public sealed class MetricsRepository
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<MetricsRepository> _logger;

    public MetricsRepository(AppDbContext dbContext, ILogger<MetricsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<MetricsDto> GetTenantMetrics(Guid tenantId, TimeInterval interval, CancellationToken cancellationToken)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        (DateTimeOffset currentStart, DateTimeOffset currentEnd) = GetIntervalRange(interval, now);
        (DateTimeOffset previousStart, DateTimeOffset previousEnd) = GetPreviousIntervalRange(interval, now);

        MetricsDto currentMetrics = await CalculateMetricsForPeriodAsync(tenantId, null, currentStart, currentEnd, cancellationToken);
        MetricsDto previousMetrics = await CalculateMetricsForPeriodAsync(tenantId, null, previousStart, previousEnd, cancellationToken);

        return new MetricsDto
        {
            TotalInteractions = currentMetrics.TotalInteractions,
            TotalHighUrgency = currentMetrics.TotalHighUrgency,
            ModelConfidence = currentMetrics.ModelConfidence,
            SatisfactionIndex = currentMetrics.SatisfactionIndex,
            TotalInteractionsTrend = CalculatePercentageChange(previousMetrics.TotalInteractions, currentMetrics.TotalInteractions),
            TotalHighUrgencyTrend = CalculatePercentageChange(previousMetrics.TotalHighUrgency, currentMetrics.TotalHighUrgency),
            ModelConfidenceTrend = CalculatePercentageChange(previousMetrics.ModelConfidence, currentMetrics.ModelConfidence),
            SatisfactionIndexTrend = CalculatePercentageChange(previousMetrics.SatisfactionIndex, currentMetrics.SatisfactionIndex)
        };


    }

    public async Task<MetricsDto?> GetSingleAccountMetricsAsync(Guid tenantId, Guid accountId, TimeInterval interval, CancellationToken cancellationToken)
    {
        Account? account = await _dbContext.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken);

        if (account == null)
            return null;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        (DateTimeOffset currentStart, DateTimeOffset currentEnd) = GetIntervalRange(interval, now);
        (DateTimeOffset previousStart, DateTimeOffset previousEnd) = GetPreviousIntervalRange(interval, now);

        MetricsDto currentMetrics = await CalculateMetricsForPeriodAsync(tenantId, accountId, currentStart, currentEnd, cancellationToken);
        MetricsDto previousMetrics = await CalculateMetricsForPeriodAsync( tenantId, accountId, previousStart, previousEnd, cancellationToken);


        return new MetricsDto
        {
            TotalInteractions = currentMetrics.TotalInteractions,
            TotalHighUrgency = currentMetrics.TotalHighUrgency,
            ModelConfidence = currentMetrics.ModelConfidence,
            SatisfactionIndex = currentMetrics.SatisfactionIndex,
            TotalInteractionsTrend = CalculatePercentageChange(previousMetrics.TotalInteractions, currentMetrics.TotalInteractions),
            TotalHighUrgencyTrend = CalculatePercentageChange(previousMetrics.TotalHighUrgency, currentMetrics.TotalHighUrgency),
            ModelConfidenceTrend = CalculatePercentageChange(previousMetrics.ModelConfidence, currentMetrics.ModelConfidence),
            SatisfactionIndexTrend = CalculatePercentageChange(previousMetrics.SatisfactionIndex, currentMetrics.SatisfactionIndex)
        };
    }

    /// <summary>
    /// Berechnet Metriken für einen bestimmten Zeitraum.
    /// </summary>
    private async Task<MetricsDto> CalculateMetricsForPeriodAsync(Guid tenantId, Guid? accountId, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken)
    {
        IQueryable<Interaction> interactions = _dbContext.Interactions
            .AsNoTracking()
            .Where(i => i.TenantId == tenantId && i.OccurredAt >= startDate && i.OccurredAt < endDate);

        if (accountId.HasValue)
        {
            interactions = interactions.Where(i => i.AccountId == accountId.Value);
        }

        int totalInteractions = await interactions.CountAsync(cancellationToken);

        List<InteractionWithInference> interactionsWithInference = await interactions
            .GroupJoin(
                _dbContext.TextInferences.AsNoTracking(),
                i => i.Id,
                ti => ti.InteractionId,
                (i, tiGroup) => new InteractionWithInference
                {
                    Interaction = i,
                    Inference = tiGroup.FirstOrDefault()
                })
            .ToListAsync(cancellationToken);

        int totalHighUrgency = interactionsWithInference.Count(x => x.Inference != null && x.Inference.Urgency == "HIGH");

        List<double> confidenceValues = interactionsWithInference
            .Where(x => x.Inference != null)
            .Select(x => CalculateInteractionConfidence(x.Inference!))
            .Where(c => c.HasValue)
            .Select(c => c!.Value)
            .ToList();

        int modelConfidence = confidenceValues.Any()
            ? Convert.ToInt32(confidenceValues.Average() * 100)
            : 0;

        IQueryable<SatisfactionState> satisfactionQuery = _dbContext.SatisfactionStates
            .AsNoTracking()
            .Where(s => s.TenantId == tenantId &&
                       s.LastUpdatedUtc >= startDate &&
                       s.LastUpdatedUtc < endDate);

        if (accountId.HasValue)
        {
            satisfactionQuery = satisfactionQuery.Where(s => s.AccountId == accountId.Value);
        }

        List<SatisfactionStateData> satisfactionStates = await satisfactionQuery
            .Select(s => new SatisfactionStateData
            {
                DecayedWeightedSum = s.DecayedWeightedSum,
                DecayedWeightSum = s.DecayedWeightSum,
                SatisfactionIndex = s.SatisfactionIndex
            })
            .ToListAsync(cancellationToken);

        double sumNumerator = satisfactionStates.Sum(x => x.DecayedWeightedSum);
        double sumDenominator = satisfactionStates.Sum(x => x.DecayedWeightSum);
        int averageFallback = satisfactionStates.Any()
            ? Convert.ToInt32(satisfactionStates.Average(x => x.SatisfactionIndex))
            : 50;

        int satisfactionIndex = sumDenominator > 0.0d
            ? Convert.ToInt32((sumNumerator / sumDenominator) * 100)
            : averageFallback;

        return new MetricsDto
        {
            TotalInteractions = totalInteractions,
            TotalHighUrgency = totalHighUrgency,
            ModelConfidence = modelConfidence,
            SatisfactionIndex = satisfactionIndex
        };
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

    private (DateTimeOffset Start, DateTimeOffset End) GetPreviousIntervalRange(TimeInterval interval, DateTimeOffset now)
    {
        return interval switch
        {
            TimeInterval.ThisWeek => (StartOfWeek(now.AddDays(-7)), StartOfWeek(now)),
            TimeInterval.ThisMonth => (StartOfMonth(now.AddMonths(-1)), StartOfMonth(now)),
            TimeInterval.ThisYear => (StartOfYear(now.AddYears(-1)), StartOfYear(now)),
            TimeInterval.LastWeek => (StartOfWeek(now.AddDays(-14)), StartOfWeek(now.AddDays(-7))),
            TimeInterval.LastMonth => (StartOfMonth(now.AddMonths(-2)), StartOfMonth(now.AddMonths(-1))),
            TimeInterval.LastYear => (StartOfYear(now.AddYears(-2)), StartOfYear(now.AddYears(-1))),
            _ => throw new ArgumentException($"Ungültiges Intervall: {interval}")
        };
    }

    private int CalculatePercentageChange(int previousValue, int currentValue)
    {
        if (previousValue == 0)
        {
            return currentValue > 0 ? 100 : 0;
        }

        return Convert.ToInt32((currentValue - previousValue) / previousValue * 100);
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

    private static double? CalculateInteractionConfidence(TextInference inference)
    {
        List<double> scores = new List<double>();

        scores.Add(inference.SentimentScore);
        scores.Add(inference.UrgencyScore);

        if (inference.Aspects != null && inference.Aspects.Any())
            scores.Add(inference.Aspects.Average(a => a.Score));

        if (inference.Emotions != null && inference.Emotions.Any())
            scores.Add(inference.Emotions.Average(e => e.Score));

        return scores.Any() ? scores.Average() : null;
    }
}