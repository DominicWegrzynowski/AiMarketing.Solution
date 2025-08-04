namespace AiMarketing.Blazor.Models.StripeEntities
{
    public class CheckoutRequest
    {
        public string PriceId { get; set; } = string.Empty;
        public string SubscrptionTier { get; set; } = string.Empty; // e.g.  "Starter", "Pro"
    }
}
