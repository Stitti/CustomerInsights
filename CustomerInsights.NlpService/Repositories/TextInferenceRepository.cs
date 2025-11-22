using CustomerInsights.Database;
using CustomerInsights.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerInsights.NlpService.Repositories
{
    public sealed class TextInferenceRepository
    {
        private readonly AppDbContext _appDbContext;

        public TextInferenceRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
        }

        public async Task<bool> CreateTextInferenceAsync(Guid interactionId, TextInference textInference, CancellationToken cancellationToken)
        {
            textInference.InteractionId = interactionId;

            TextInference? existingTextInference = await _appDbContext.TextInferences
                .Include(textInferenceEntity => textInferenceEntity.Aspects)
                .Include(textInferenceEntity => textInferenceEntity.Emotions)
                .SingleOrDefaultAsync(textInferenceEntity => textInferenceEntity.InteractionId == interactionId, cancellationToken);

            if (existingTextInference != null)
            {
                _appDbContext.Entry(existingTextInference).CurrentValues.SetValues(textInference);

                existingTextInference.Aspects.Clear();
                foreach (AspectRating aspect in textInference.Aspects)
                {
                    existingTextInference.Aspects.Add(aspect);
                }

                existingTextInference.Emotions.Clear();
                foreach (EmotionRating emotion in textInference.Emotions)
                {
                    existingTextInference.Emotions.Add(emotion);
                }
            }
            else
            {
                await _appDbContext.TextInferences.AddAsync(textInference, cancellationToken);
            }

            int affectedRows = await _appDbContext.SaveChangesAsync(cancellationToken);
            return affectedRows > 0;
        }
    }
}
