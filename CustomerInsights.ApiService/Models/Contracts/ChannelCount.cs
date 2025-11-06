using CustomerInsights.Base.Enums;

namespace CustomerInsights.ApiService.Models.Contracts
{
    public sealed class ChannelCount
    {
        public Channel Channel { get; set; }
        public long InteractionCount { get; set; }
    }

    public enum Period
    {
        LastWeek,      // rollierend: jetzt - 7 Tage
        LastMonth,     // rollierend: jetzt - 30 Tage
        LastYear,      // rollierend: jetzt - 365 Tage
        CalendarWeek,  // aktuelle Kalenderwoche
        CalendarMonth, // aktueller Kalendermonat
        CalendarYear,  // aktuelles Kalenderjahr
    }
}
