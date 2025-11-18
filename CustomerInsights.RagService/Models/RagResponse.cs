using System.Collections.Generic;

namespace CustomerInsights.RagService.Models
{
    public class RagResponse
    {
        public string Answer { get; set; } = string.Empty;
        public List<RagDocument> Documents { get; set; } = new List<RagDocument>();
    }
}