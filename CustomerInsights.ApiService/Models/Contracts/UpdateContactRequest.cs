using CustomerInsights.ApiService.Patching;

namespace CustomerInsights.ApiService.Models.Contracts
{
    public sealed class UpdateContactRequest
    {
        public PatchField<string?> Firstname { get; set; }
        public PatchField<string?> Lastname { get; set; }
        public PatchField<string?> Email { get; set; }
        public PatchField<string?> Phone { get; set; }
        public PatchField<Guid?> AccountId { get; set; }
        public PatchField<string?> ExternalId { get; set; }
    }
}
