namespace CustomerInsights.Base.Enums
{
    public enum Channel
    {
        Unknown = 0,

        // Direkte Kommunikationskanäle
        Email = 1,
        Ticket = 2,
        Call = 3,
        Meeting = 4,
        Chat = 5,

        // Indirekte/öffentliche Feedback-Kanäle
        Survey = 10,
        Review = 11,
        Social = 12,

        // Interne / Vertriebsorientierte Quellen
        VisitReport = 20,
        CrmNote = 21,
        NpsSurvey = 22,

        // Erweiterbar für spätere Kanäle
        Other = 99
    }
}
