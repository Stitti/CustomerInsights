using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerInsights.SatisfactionIndexService.Models
{
    public sealed class WeightedInteraction
    {
        public double SatisfactionZeroOne { get; init; }
        public double Weight { get; init; }
    }
}
