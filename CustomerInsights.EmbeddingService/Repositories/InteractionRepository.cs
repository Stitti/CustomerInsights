using CustomerInsights.ApiService.Models;
using CustomerInsights.Database;
using CustomerInsights.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerInsights.EmbeddingService.Repositories
{
    public class InteractionRepository
    {
        private readonly AppDbContext _db;
        private readonly ILogger<InteractionRepository> _logger;

        public InteractionRepository(AppDbContext db, ILogger<InteractionRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Interaction?> GetInteractionByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty || tenantId == Guid.Empty)
                return null;

            Interaction? entity = await _db.Interactions
                .AsNoTracking()
                .AsSplitQuery()
                .Include(i => i.TextInference)
                    .ThenInclude(ti => ti.Aspects)
                .Include(i => i.TextInference)
                    .ThenInclude(ti => ti.Emotions)
                .Include(i => i.Account)
                .Include(i => i.Contact)
                .Select(i => new Interaction
                {
                    Id = i.Id,
                    ExternalId = i.ExternalId,
                    Subject = i.Subject,
                    Channel = i.Channel,
                    OccurredAt = i.OccurredAt,
                    Text = i.Text,
                    Account = i.Account != null ? new Account { Id = i.Account.Id, Name = i.Account.Name } : null,
                    Contact = i.Contact != null ? new Contact { Id = i.Contact.Id, Firstname = i.Contact.Firstname, Lastname = i.Contact.Lastname } : null,
                    TextInference = i.TextInference != null ? new TextInference
                    {
                        Sentiment = i.TextInference.Sentiment,
                        Urgency = i.TextInference.Urgency,
                        Aspects = i.TextInference.Aspects,
                        Emotions = i.TextInference.Emotions.Select(x => new EmotionRating { Label = x.Label}).ToList(),
                    } : null
                })
                .SingleOrDefaultAsync(i => i.TenantId == tenantId && i.Id == id, cancellationToken);

            return entity;
        }
    }
}
