using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace CustomerInsights.SubscriptionService.Controllers
{
    [ApiController]
    [Route("api/billing")]
    public sealed class BillingController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDb _db;
        private readonly ILogger<BillingController> _logger;

        public BillingController(IConfiguration configuration, AppDb db, ILogger<BillingController> logger)
        {
            _configuration = configuration;
             _db = db;
             _logger = logger;
        }

        // POST /api/billing/checkout?plan=premium_month
        [HttpPost("checkout")]
        public async Task<IActionResult> CreateCheckout([FromQuery] string plan)
        {
            if (Guid.TryParse(User.FindFirst("org_id")?.Value, out Guid tenantId) == false)
                return Unauthorized();

            var org = await _db.Organizations.FindAsync(tenantId);
            if (org == null)
                return BadRequest("Tenant not found");

            var subscription = await _db.Subscriptions.SingleOrDefaultAsync(s => s.OrgId == tenantId && s.Provider=="stripe");

            CustomerService customerService = new Stripe.CustomerService();
            string customerId;
            if (subscription?.ProviderCustomerId is { Length: > 0 })
            {
                customerId = subscription.ProviderCustomerId;
            }
            else
            {
                Customer cust = await customerService.CreateAsync(new CustomerCreateOptions{
                    Name = org.Name,
                    Metadata = new Dictionary<string, string> { ["orgId"] = tenantId.ToString() }
                });

                customerId = cust.Id;
            }

            string priceLookupKey = _configuration[$"Stripe:PriceLookup:{plan}"] ?? plan;
            PriceService priceService = new PriceService();
            Price price = (await priceService.ListAsync(new PriceListOptions { LookupKeys = new List<string>{ priceLookupKey }, Limit = 1 })).FirstOrDefault() ?? throw new Exception($"Price with lookup_key '{priceLookupKey}' not found");

            string domain = $"{Request.Scheme}://{Request.Host}";
            SessionService sessionService = new SessionService();
            Session session = await sessionService.CreateAsync(new Stripe.Checkout.SessionCreateOptions{
                Mode = "subscription",
                Customer = customerId,
                LineItems = new List<SessionLineItemOptions> {
                    new SessionLineItemOptions
                    {
                        Price = price.Id,
                        Quantity = 1
                    }
                },
                SuccessUrl = $"{domain}/billing/success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl  = $"{domain}/billing/cancelled",
                ClientReferenceId = tenantId.ToString(),
                AllowPromotionCodes = true,
                AutomaticTax = new SessionAutomaticTaxOptions { Enabled = true },
            });

            return Ok(new { url = session.Url });
        }

        // POST /api/billing/portal
        [HttpPost("portal")]
        public async Task<IActionResult> CreatePortal()
        {
            if (Guid.TryParse(User.FindFirst("org_id")?.Value, out Guid tenantId) == false)
                return Unauthorized();

            var subscription = await _db.Subscriptions.SingleOrDefaultAsync(s => s.OrgId == tenantId && s.Provider=="stripe");
            if (subscription == null)
            {
                _logger.LogError("Found no subscription for tenant {TenantId}", tenantId);
                return BadRequest();
            }

            string domain = $"{Request.Scheme}://{Request.Host}";
            Stripe.BillingPortal.SessionService portalService = new Stripe.BillingPortal.SessionService();
            Stripe.BillingPortal.Session portal = await portalService.CreateAsync(new Stripe.BillingPortal.SessionCreateOptions
            {
                Customer = subscription.ProviderCustomerId!,
                ReturnUrl = $"{domain}/settings/billing"
            });

            return Ok(new { url = portal.Url });
        }
    }
}