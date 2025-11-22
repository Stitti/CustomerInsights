using CustomerInsights.ApiService.Patching;

namespace CustomerInsights.ApiService.Models.Contracts
{
    public sealed class UpdateApiKeyRequest
    {
        public PatchField<string?> Name { get; set; }
        public PatchField<string?> Description { get; set; }
    }
}
