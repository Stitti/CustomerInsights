using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerInsights.Base.Models
{
    public sealed class OutboxMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid TenantId { get; set; }

        public Guid TargetId { get; set; }

        public string Type { get; set; } = string.Empty; // z.B. "InteractionCreated"

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedUtc { get; set; }

        public int RetryCount { get; set; } = 0;
        public string? ErrorMessage { get; set; }
    }
}
