using CustomerInsights.Database;
using CustomerInsights.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerInsights.EmailService.Services
{
    public sealed class EmailTemplateProvider
    {
        private readonly AppDbContext _db;
        private readonly IMemoryCache _cache;
        private readonly ILogger<EmailTemplateProvider> _logger;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

        public EmailTemplateProvider(AppDbContext db, IMemoryCache cache, ILogger<EmailTemplateProvider> logger)
        {
            _db = db;
            _cache = cache;
            _logger = logger;
        }

        public async Task<string?> GetTemplateAsync(string templateKey, string languageCode)
        {
            if (string.IsNullOrWhiteSpace(templateKey))
            {
                _logger.LogError("Template key is emtpy");
                return null;
            }

            string cacheKey = $"email-template:{templateKey}:{languageCode}";

            if (_cache.TryGetValue(cacheKey, out string? cachedTemplate) && string.IsNullOrEmpty(cachedTemplate) == false)
            {
                _logger.LogInformation("Found cached email template {EmailTemplateKey}, Language: {EmailLanguageCode}", templateKey,  languageCode);
                return cachedTemplate;
            }

            EmailTemplate? emailTemplate = await _db.EmailTemplates.AsNoTracking()
                                                                   .Where(t => t.Key == templateKey && t.LanguageCode == languageCode)
                                                                   .FirstOrDefaultAsync();

            if (emailTemplate == null)
            {
                _logger.LogError("Found no email template. Key={Key}, Culture={Culture}",  templateKey, languageCode);
                return null;
            }

            _cache.Set(cacheKey, emailTemplate.Content, CacheDuration);

            return emailTemplate.Content;
        }
    }
}
