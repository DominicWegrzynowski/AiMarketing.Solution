namespace AiMarketing.Blazor.Models.StripeEntities
{
    public class SubscriptionRequest
    {
        public string Email { get; set; } = "";
        public string FullName { get; set; } = "";
        public string PriceId { get; set; } = ""; // From Stripe Dashboard or your DB
    }
}
