using CustomerInsights.ApiService.Patching;

namespace CustomerInsights.ApiService.Models.Contracts
{
    public sealed class UpdateSignalRequest
    {
        public PatchField<string?> Description { get; set; }
    }
}
