using CustomerInsights.ApiService.Models.Enums;
using CustomerInsights.ApiService.Patching;

namespace CustomerInsights.ApiService.Models.Contracts
{
    public class UpdateAccountRequest
    {
        public PatchField<string?> Name { get; set; }
        public PatchField<string?> ExternalId { get; set; }
        public PatchField<Guid?> ParentAccountId { get; set; }
        public PatchField<string?> Industry { get; set; }
        public PatchField<string?> Country { get; set; }
        public PatchField<CustomerClassification?> Classification { get; set; }
    }
}
