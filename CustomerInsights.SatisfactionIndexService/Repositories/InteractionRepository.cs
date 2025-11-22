using CustomerInsights.Database;
using CustomerInsights.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerInsights.SatisfactionIndexService.Repositories
{
    public sealed class InteractionRepository
    {
        private readonly AppDbContext _db;
        private readonly ILogger<InteractionRepository> _logger;

        public InteractionRepository(AppDbContext db, ILogger<InteractionRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Interaction?> GetInteractionByIdAsync(Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty)
                return null;

            return await _db.Interactions
                .AsNoTracking()
                .AsSplitQuery()
                .Include(i => i.TextInference)
                    .ThenInclude(ti => ti.Aspects)
                .Include(i => i.TextInference)
                    .ThenInclude(ti => ti.Emotions)
                .Include(i => i.Account)
                .Include(i => i.Contact)
                .SingleOrDefaultAsync(i => i.Id == id, ct);
        }
    }
}
