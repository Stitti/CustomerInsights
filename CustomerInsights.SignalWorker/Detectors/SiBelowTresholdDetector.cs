using CustomerInsights.ApiService.Models.Enums;
using CustomerInsights.SignalWorker.Models;

namespace CustomerInsights.SignalWorker.Detectors;

public sealed class SiBelowThresholdDetector
{
    private readonly SiBelowTresholdSignalConfig _configuration;

    public SiBelowThresholdDetector(SiBelowTresholdSignalConfig configuration)
    {
        _configuration = configuration ?? new SiBelowTresholdSignalConfig();
    }

    /// <summary>
    /// Erzeugt ein Signal, wenn currentSi &lt; Threshold. Sonst null.
    /// </summary>
    public Signal? Detect(Guid tenantId, Guid accountId, double currentSi, DateTime nowUtc)
    {
        if (currentSi >= _configuration.ThresholdPoints)
        {
            return null;
        }

        SeverityLevel severity = currentSi <= (_configuration.ThresholdPoints - _configuration.HighSeverityGap)
            ? SeverityLevel.High
            : SeverityLevel.Medium;

        string dayKey = nowUtc.ToString("yyyyMMdd");
        string dedupeKey = _configuration.DailyDedupe
            ? $"si_below_threshold:{tenantId}:{accountId}:{dayKey}"
            : $"si_below_threshold:{tenantId}:{accountId}:{nowUtc:yyyyMMddHHmm}";

        Signal record = new Signal
        {
            TenantId = tenantId,
            AccountId = accountId,
            Type = "si_below_threshold",
            Severity = severity,
            CreatedUtc = nowUtc,
            TtlDays = _configuration.DefaultTtlDays,
            DedupeKey = dedupeKey,
            AccountSatisfactionIndex = Math.Round(currentSi, 1),
            Threshold = _configuration.ThresholdPoints
        };

        return record;
    }
}
