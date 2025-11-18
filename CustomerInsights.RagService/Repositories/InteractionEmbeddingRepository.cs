using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CustomerInsights.Database;
using CustomerInsights.Models;
using CustomerInsights.RagService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pgvector;

namespace CustomerInsights.RagService.Services
{
    public class InteractionEmbeddingRepository
    {
        private readonly AppDbContext appDbContext;
        private readonly ILogger<InteractionEmbeddingRepository> logger;
        private readonly int defaultMaxDocuments;

        public InteractionEmbeddingRepository(
            AppDbContext appDbContext,
            IConfiguration configuration,
            ILogger<InteractionEmbeddingRepository> logger)
        {
            this.appDbContext = appDbContext;
            this.logger = logger;

            int configuredMaxDocuments = configuration.GetValue<int>("Rag:MaxDocuments", 20);
            this.defaultMaxDocuments = configuredMaxDocuments > 0 ? configuredMaxDocuments : 20;
        }

        public async Task<IList<RagDocument>> GetRelevantDocumentsAsync(
            double[] queryEmbedding,
            int? accountId,
            string? product,
            string? sentiment,
            DateTime? from,
            DateTime? to,
            int maxDocuments,
            CancellationToken cancellationToken = default)
        {
            if (queryEmbedding == null || queryEmbedding.Length == 0)
            {
                throw new ArgumentException("queryEmbedding must not be null or empty.", nameof(queryEmbedding));
            }

            int effectiveMaxDocuments = maxDocuments > 0 ? maxDocuments : this.defaultMaxDocuments;

            float[] embeddingValues = new float[queryEmbedding.Length];
            for (int index = 0; index < queryEmbedding.Length; index++)
            {
                embeddingValues[index] = (float)queryEmbedding[index];
            }

            Vector embeddingVector = new Vector(embeddingValues);

            // Alle Filter als Parameter, damit alles schön parameterisiert ist.
            int? accountIdParameter = accountId;
            string? productParameter = product;
            string? sentimentParameter = sentiment;
            DateTime? fromParameter = from;
            DateTime? toParameter = to;
            int limitParameter = effectiveMaxDocuments;

            FormattableString sql = $@"
                SELECT 
                    id,
                    interaction_id,
                    account_id,
                    contact_id,
                    channel,
                    emotion,
                    products,
                    tags,
                    created_at,
                    text_full,
                    embedding
                FROM interaction_embeddings
                WHERE (@accountIdParameter IS NULL OR account_id = @accountIdParameter)
                  AND (@productParameter IS NULL OR @productParameter = ANY(products))
                  AND (@sentimentParameter IS NULL OR emotion = @sentimentParameter)
                  AND (@fromParameter IS NULL OR created_at >= @fromParameter)
                  AND (@toParameter IS NULL OR created_at <= @toParameter)
                ORDER BY embedding <-> {embeddingVector}
                LIMIT {limitParameter};
            ";

            List<InteractionEmbedding> entities = await this.appDbContext.InteractionEmbeddings
                .FromSqlInterpolated(sql)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            List<RagDocument> documents = new List<RagDocument>();

            foreach (InteractionEmbedding entity in entities)
            {
                RagDocument document = new RagDocument();
                document.Id = entity.Id;
                document.InteractionId = entity.InteractionId;
                document.CompanyId = entity.AccountId;
                document.ContactId = entity.ContactId;
                document.Channel = entity.Channel;
                document.Emotion = entity.Emotion;
                document.Products = entity.Products;
                document.Tags = entity.Tags;
                document.CreatedAt = entity.CreatedAt;
                document.TextFull = entity.TextFull;

                documents.Add(document);
            }

            this.logger.LogDebug(
                "RAG query returned {Count} documents (AccountId={AccountId}, Product={Product}, Sentiment={Sentiment}, From={From}, To={To})",
                documents.Count,
                accountId,
                product,
                sentiment,
                from,
                to);

            return documents;
        }
    }
}
