using CustomerInsights.ApiService.Models.Enums;

namespace CustomerInsights.ApiService.Models.Contracts
{
    public class CreateAccountRequest
    {
        public string Name { get; set; }
        public Guid ParentAccountId { get; set; }
        public string Industry { get; set; }
        public CustomerClassification Classification { get; set; }
    }
}
