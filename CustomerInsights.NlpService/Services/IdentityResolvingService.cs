using CustomerInsights.ApiService.Models;
using CustomerInsights.Database;
using CustomerInsights.Models;
using CustomerInsights.NlpService.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CustomerInsights.NlpService.Services
{
    public sealed class IdentityResolvingService
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<IdentityResolvingService> _logger;

        public IdentityResolvingService(AppDbContext appDbContext, ILogger<IdentityResolvingService> logger)
        {
            if (appDbContext == null)
                throw new ArgumentNullException(nameof(appDbContext));

            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task<bool> ResolveAndSetContactAndAccountAsync(Guid interactionId, IList<PresidioAnalyzerEntity> entities, CancellationToken cancellationToken)
        {
            if (entities == null)
            {
                _logger.LogWarning("Entities list is null for interaction {InteractionId}", interactionId);
                return false;
            }

            Interaction? interaction = await _appDbContext.Interactions
                .Include(interactionEntity => interactionEntity.Contact)
                .Include(interactionEntity => interactionEntity.Account)
                .SingleOrDefaultAsync(interactionEntity => interactionEntity.Id == interactionId, cancellationToken);

            if (interaction == null)
                return false;

            Contact? existingContact = interaction.Contact;
            Account? existingAccount = interaction.Account;

            Contact? resolvedContact = await ResolveContactAsync(entities, cancellationToken);
            Account? resolvedAccount = await ResolveAccountAsync(entities, resolvedContact, cancellationToken);

            Contact? effectiveContact = existingContact != null ? existingContact : resolvedContact;
            Account? effectiveAccount = existingAccount != null ? existingAccount : resolvedAccount;

            if (effectiveAccount == null &&
                effectiveContact != null &&
                effectiveContact.AccountId.HasValue)
            {
                Account? accountFromContact = await _appDbContext.Accounts.SingleOrDefaultAsync(accountEntity => accountEntity.Id == effectiveContact.AccountId.Value, cancellationToken);

                if (accountFromContact != null)
                {
                    effectiveAccount = accountFromContact;
                }
            }

            if (effectiveContact != null && effectiveAccount != null && (effectiveContact.AccountId.HasValue == false || effectiveContact.AccountId.Value != effectiveAccount.Id))
                return false;

            if (interaction.ContactId == null && effectiveContact != null)
                interaction.ContactId = effectiveContact.Id;

            if (interaction.AccountId == null && effectiveAccount != null)
                interaction.AccountId = effectiveAccount.Id;

            try
            {
                await _appDbContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes for interaction {InteractionId}", interactionId);
                return false;

            }
        }

        private async Task<Contact?> ResolveContactAsync(IList<PresidioAnalyzerEntity> entities, CancellationToken cancellationToken)
        {
            PresidioAnalyzerEntity? emailEntity = entities
                .Where(entity => entity.EntityType == "EMAIL_ADDRESS")
                .OrderByDescending(entity => entity.Score)
                .FirstOrDefault();

            if (emailEntity != null && string.IsNullOrWhiteSpace(emailEntity.Text) == false)
            {
                string normalizedEmail = emailEntity.Text.Trim().ToLowerInvariant();

                Contact? contactByEmail = await _appDbContext.Contacts
                    .Include(contactEntity => contactEntity.Account)
                    .Where(contactEntity => contactEntity.Email != null && contactEntity.Email.ToLower() == normalizedEmail)
                    .OrderBy(contactEntity => contactEntity.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (contactByEmail != null)
                {
                    return contactByEmail;
                }
            }

            PresidioAnalyzerEntity? personEntity = entities
                .Where(entity => entity.EntityType == "PERSON")
                .OrderByDescending(entity => entity.Score)
                .FirstOrDefault();

            if (personEntity == null || string.IsNullOrWhiteSpace(personEntity.Text))
                return null;

            (string? firstname, string? lastname) = SplitFullName(personEntity.Text);

            if (string.IsNullOrWhiteSpace(firstname) == false && string.IsNullOrWhiteSpace(lastname) == false)
            {
                string normalizedFirstname = firstname.Trim().ToLowerInvariant();
                string normalizedLastname = lastname.Trim().ToLowerInvariant();

                Contact? contactByFullName = await _appDbContext.Contacts
                    .Include(contactEntity => contactEntity.Account)
                    .Where(contactEntity =>
                        contactEntity.Firstname != null &&
                        contactEntity.Lastname != null &&
                        contactEntity.Firstname.ToLower() == normalizedFirstname &&
                        contactEntity.Lastname.ToLower() == normalizedLastname)
                    .OrderBy(contactEntity => contactEntity.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (contactByFullName != null)
                {
                    return contactByFullName;
                }
            }

            if (!string.IsNullOrWhiteSpace(lastname))
            {
                string normalizedLastnameOnly = lastname.Trim().ToLowerInvariant();

                List<Contact> contactsByLastname = await _appDbContext.Contacts
                    .Include(contactEntity => contactEntity.Account)
                    .Where(contactEntity =>
                        contactEntity.Lastname != null &&
                        contactEntity.Lastname.ToLower() == normalizedLastnameOnly)
                    .ToListAsync(cancellationToken);

                if (contactsByLastname.Count == 1)
                {
                    return contactsByLastname[0];
                }
            }

            return null;
        }

        private async Task<Account?> ResolveAccountAsync(IList<PresidioAnalyzerEntity> entities, Contact? resolvedContact, CancellationToken cancellationToken)
        {
            if (resolvedContact != null && resolvedContact.AccountId.HasValue)
            {
                Account? accountFromContact = await _appDbContext.Accounts.SingleOrDefaultAsync(accountEntity => accountEntity.Id == resolvedContact.AccountId.Value, cancellationToken);
                if (accountFromContact != null)
                {
                    return accountFromContact;
                }
            }

            PresidioAnalyzerEntity? organizationEntity = entities
                .Where(entity => entity.EntityType == "ORGANIZATION")
                .OrderByDescending(entity => entity.Score)
                .FirstOrDefault();

            if (organizationEntity == null || string.IsNullOrWhiteSpace(organizationEntity.Text))
                return null;

            string organizationNameFromText = organizationEntity.Text.Trim();
            string normalizedOrganizationName = organizationNameFromText.ToLowerInvariant();

            Account? accountByExactName = await _appDbContext.Accounts
                .Where(accountEntity => accountEntity.Name != null && accountEntity.Name.ToLower() == normalizedOrganizationName)
                .OrderBy(accountEntity => accountEntity.Id)
                .FirstOrDefaultAsync(cancellationToken);

            return accountByExactName;
        }

        private (string? Firstname, string? Lastname) SplitFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return (null, null);

            string[] parts = fullName.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                return (null, null);

            if (parts.Length == 1)
                return (null, parts[0]);

            string firstname = parts[0];
            string lastname = string.Join(" ", parts.Skip(1));
            return (firstname, lastname);
        }
    }
}
