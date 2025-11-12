using CustomerInsights.SubscriptionService.Models;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Subscription = CustomerInsights.SubscriptionService.Models.Subscription;

namespace CustomerInsights.SubscriptionService.Controllers
{
    [ApiController]
    [Route("webhooks/stripe")]
    public sealed class StripeWebhookController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDb _db;
        private readonly ILogger<StripeWebhookController> _logger;

        public StripeWebhookController(IConfiguration configuration, AppDb db, ILogger<StripeWebhookController> logger)
        {
            _configuration = configuration;
            _db = db;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Handle()
        {
            string json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"];
            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(json, signature, _configuration["Stripe:WebhookSecret"]);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Invalid webhook signature");
                return Unauthorized();
            }

            // Idempotenz
            if (await _db.WebhookEvents.AnyAsync(e => e.EventId == stripeEvent.Id))
                return Ok();

            _db.WebhookEvents.Add(new WebhookEvent { EventId = stripeEvent.Id, ReceivedAt = DateTime.UtcNow });

            switch (stripeEvent.Type)
            {
                case "checkout.session.completed":
                {
                    Stripe.Checkout.Session? session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                    if (session == null)
                    {
                        _logger.LogError("Session not found");
                        return BadRequest();
                    }

                    await UpsertFromSubscriptionId(session.CustomerId, session.SubscriptionId);
                    break;
                }
                case "customer.subscription.created":
                case "customer.subscription.updated":
                case "customer.subscription.deleted":
                {
                    Stripe.Subscription subscription = stripeEvent.Data.Object as Stripe.Subscription;
                    await UpsertFromSubscriptionObject(subscription);
                    break;
                }
                case "invoice.paid":
                case "invoice.payment_failed":
                    // optional Dunning/Benachrichtigung
                    break;
            }

            await _db.SaveChangesAsync();
            return Ok();
        }

        private async Task UpsertFromSubscriptionId(string customerId, string subscriptionId)
        {
            Stripe.SubscriptionService subscriptionService = new Stripe.SubscriptionService();
            Stripe.Subscription subscription = await subscriptionService.GetAsync(subscriptionId, new Stripe.SubscriptionGetOptions { Expand = new List<string>{ "items.data.price" }});
            await UpsertFromSubscriptionObject(subscription);
        }

        private async Task UpsertFromSubscriptionObject(Stripe.Subscription s)
        {
            Stripe.CustomerService customerService = new Stripe.CustomerService();
            Customer customer = await customerService.GetAsync(s.CustomerId);
            Guid tenantId = Guid.Parse(customer.Metadata.TryGetValue("orgId", out string? value) ? value : string.Empty);
            if (tenantId == Guid.Empty)
            {
                throw new Exception("orgId missing on customer");
            }

            Price price = s.Items.Data.First().Price;
            string planCode = price.LookupKey ?? price.Nickname ?? price.Id;

            Subscription subscription = await _db.Subscriptions.SingleOrDefaultAsync(x => x.OrgId == tenantId && x.Provider=="stripe");
            if (subscription == null)
            {
                subscription = new Subscription { Id = Guid.NewGuid(), TenantId = tenantId, Provider = "stripe" };
                _db.Subscriptions.Add(subscription);
            }

            subscription.ProviderCustomerId = s.CustomerId;
            subscription.ProviderSubId = s.Id;
            subscription.PlanCode = planCode;
            subscription.Status = MapStatus(s.Status);
            subscription.CurrentPeriodEndUtc = DateTime.SpecifyKind(s.CurrentPeriodEnd.Value, DateTimeKind.Utc);
            subscription.UpdatedUtc = DateTime.UtcNow;
        }

        private static string MapStatus(string stripeStatus) => stripeStatus switch
        {
            "active" => "active",
            "trialing" => "trialing",
            "past_due" => "past_due",
            "unpaid" => "unpaid",
            "canceled" => "canceled",
            _ => stripeStatus
        };
    }
}