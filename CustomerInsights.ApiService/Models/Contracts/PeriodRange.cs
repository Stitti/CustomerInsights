using NodaTime;

namespace CustomerInsights.ApiService.Models.Contracts
{
    public static class PeriodRange
    {
        public static (DateTimeOffset from, DateTimeOffset to) GetRange(Period period, DateTimeOffset nowUtc, DateTimeZone? tenantTz = null)
        {
            // Optional: mit NodaTime arbeiten – hier simpel in UTC
            var now = nowUtc;

            switch (period)
            {
                case Period.LastWeek:
                    return (now.AddDays(-7), now);
                case Period.LastMonth:
                    return (now.AddDays(-30), now);
                case Period.LastYear:
                    return (now.AddDays(-365), now);

                case Period.CalendarWeek:
                    {
                        // Woche: Montag 00:00 bis nächster Montag 00:00 (ISO-8601)
                        var monday = now.Date.AddDays(-(int)((7 + (int)now.DayOfWeek - (int)DayOfWeek.Monday) % 7));
                        var from = new DateTimeOffset(monday, TimeSpan.Zero);
                        var to = from.AddDays(7);
                        return (from, to);
                    }
                case Period.CalendarMonth:
                    {
                        var from = new DateTimeOffset(new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc));
                        var to = from.AddMonths(1);
                        return (from, to);
                    }
                case Period.CalendarYear:
                    {
                        var from = new DateTimeOffset(new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc));
                        var to = from.AddYears(1);
                        return (from, to);
                    }
                default:
                    throw new InvalidOperationException("Custom erfordert explizite From/To.");
            }
        }
    }

}
