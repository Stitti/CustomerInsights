using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerInsights.NlpService.Contracts
{
    public sealed class PresidioAnonymizeRequest
    {
        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;

        [JsonProperty("analyzer_results")]
        public List<PresidioAnalyzerEntity> AnalyzerResults { get; set; } = new List<PresidioAnalyzerEntity>();

        [JsonProperty("anonymizers")]
        public object Anonymizers { get; set; }
    }
}
