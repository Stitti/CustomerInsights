using CustomerInsights.Base.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerInsights.SatisfactionIndexService.Models
{
    public sealed class AggregationConfig
    {
        public double HalfLifeDays { get; init; } = 30;
        public double WeightEmail { get; init; } = 0.9;
        public double WeightTicket { get; init; } = 1.1;
        public double WeightCall { get; init; } = 1.2;
        public double WeightMeeting { get; init; } = 1.3;
        public double WeightChat { get; init; } = 1.0;
        public double WeightSurvey { get; init; } = 0.8;
        public double WeightReview { get; init; } = 0.7;
        public double WeightSocial { get; init; } = 0.6;
        public double WeightVisitReport { get; init; } = 1.0;
        public double WeightCrmNote { get; init; } = 0.9;
        public double WeightNpsSurvey { get; init; } = 1.1;

        public double WeightOther { get; init; } = 1.0;

        public double GetWeight(Channel channel)
        {
            return channel switch
            {
                Channel.Email => WeightEmail,
                Channel.Ticket => WeightTicket,
                Channel.Call => WeightCall,
                Channel.Meeting => WeightMeeting,
                Channel.Chat => WeightChat,

                Channel.Survey => WeightSurvey,
                Channel.Review => WeightReview,
                Channel.Social => WeightSocial,

                Channel.VisitReport => WeightVisitReport,
                Channel.CrmNote => WeightCrmNote,
                Channel.NpsSurvey => WeightNpsSurvey,

                Channel.Other => WeightOther,
                _ => WeightOther
            };
        }
    }
}
