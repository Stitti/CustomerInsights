using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerInsights.SignalWorker.Models
{
    public class SiBelowTresholdSignalConfig
    {
        public double ThresholdPoints { get; init; } = 50.0d;
        public double HighSeverityGap { get; init; } = 10.0d; // High, wenn SI <= Threshold - Gap
        public int DefaultTtlDays { get; init; } = 7;
        public bool DailyDedupe { get; init; } = true;        // einmal pro Tag und Account
    }
}
