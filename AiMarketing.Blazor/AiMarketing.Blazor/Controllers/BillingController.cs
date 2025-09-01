using Microsoft.AspNetCore.Mvc; 
using Stripe;
using Stripe.Checkout;
using AiMarketing.Blazor.Data;
using Microsoft.AspNetCore.Identity;
using AiMarketing.Blazor.Models.StripeEntities;
using Microsoft.AspNetCore.Authorization;
using AiMarketing.Blazor.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace AiMarketing.Blazor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BillingController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public BillingController(IConfiguration config, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _config = config;
        _userManager = userManager;
        _context = context;
    }

    [HttpPost("create-checkout-session")]
    [Authorize]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        // Check if user already has a StripeCustomerId
        if (string.IsNullOrEmpty(user.StripeCustomerId))
        {
            var customerOptions = new CustomerCreateOptions
            {
                Email = user.Email,
                Name = user.FullName
            };

            var customerService = new CustomerService();
            var customer = await customerService.CreateAsync(customerOptions);
            user.StripeCustomerId = customer.Id;
            await _userManager.UpdateAsync(user); // Save it to the DB
        }

        var domain = _config["App:Domain"] ?? "http://localhost:7078";

        var options = new Stripe.Checkout.SessionCreateOptions
        {
            Customer = user.StripeCustomerId, // You should create/save this when user signs up
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Price = request.PriceId,
                    Quantity = 1,
                },
            },
            Mode = "subscription",
            SuccessUrl = $"{domain}/payment-success?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{domain}/payment-cancelled",
            Metadata = new Dictionary<string, string>
            {
                {"plan", request.PriceId }
            }
        };

        var service = new Stripe.Checkout.SessionService();
        var session = await service.CreateAsync(options);
        return Ok(new { sessionUrl = session.Url });
    }

    [HttpPost("create-portal-session")]
    [Authorize]
    public async Task<IActionResult> CreatePortalSession()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var returnUrl = _config["App:Domain"] ?? "http://localhost:5000";

        var options = new Stripe.BillingPortal.SessionCreateOptions
        {
            Customer = user.StripeCustomerId,
            ReturnUrl = returnUrl,
        };

        var service = new Stripe.BillingPortal.SessionService();
        var session = service.Create(options);
        return Ok(new { sessionUrl = session.Url });
    }

    [HttpPost("webhook")]
    [Route("api/billing/webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _config["Stripe:WebhookSecret"]
            );

            //if(stripeEvent.Type == "checkout.session.completed" && stripeEvent.Data.Object is Session session)
            //{
            //    var customerId = session.CustomerId;
            //    var subscriptionId = session.SubscriptionId;

            //    // example: find user by StripeCustomerId and update db
            //    var user = _userManager.Users.FirstOrDefault(u => u.StripeCustomerId == customerId);

            //    if(user is not null)
            //    {
            //        user.StripeSubscriptionId = subscriptionId;
            //        user.SubscriptionStatus = session.Status;
            //        user.SubscriptionStart = DateTime.UtcNow;
            //        user.SubscriptionEnd = DateTime.UtcNow.AddMonths(1);



            //        if (session.Metadata.TryGetValue("plan", out var planName) &&
            //            Enum.TryParse<UserRole>(planName, ignoreCase: true, out var tier ))
            //        {
            //            user.SubscriptionTier = tier;

            //            var product = await _context.Products.FirstOrDefaultAsync(p => p.Role == tier);

            //            if(product != null)
            //            {
            //                user.MonthlyPostAllowance = product.PostAllowance;

            //                var currentRoles = await _userManager.GetRolesAsync(user);
            //                if(currentRoles.Any())
            //                {
            //                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
            //                }

            //                var roleName = tier.ToString();
            //                await _userManager.AddToRoleAsync(user, roleName);
            //            }

            //        }

            //        await _userManager.UpdateAsync(user);
            //        await _context.SaveChangesAsync();
            //    }
            //}

            switch(stripeEvent.Type)
            {
                case "checkout.session.completed":
                    if (stripeEvent.Data.Object is Session session)
                    {
                        var user = _userManager.Users.FirstOrDefault(u => u.StripeCustomerId == session.CustomerId);
                        if (user is null) break;

                        var subService = new SubscriptionService();
                        var sub = await subService.GetAsync(session.SubscriptionId);

                        // Only proceed if usable now (active or trialing)
                        if (!(sub.Status == "active" || sub.Status == "trialing"))
                            break;

                        var item = sub.Items?.Data?.FirstOrDefault();
                        if (item is null) break;

                        var periodStart = item.CurrentPeriodStart; // DateTime (UTC)
                        var periodEnd = item.CurrentPeriodEnd;   // DateTime (UTC)

                        // Idempotency: skip if we already set this exact period
                        if (user.CurrentPeriodEndUtc.HasValue && user.CurrentPeriodEndUtc.Value == periodEnd)
                            break;

                        user.StripeSubscriptionId = sub.Id;
                        user.SubscriptionStatus = sub.Status;
                        user.SubscriptionStart = DateTime.UtcNow;

                        var priceId = item.Price.Id;
                        var product = await _context.Products.FirstOrDefaultAsync(p => p.StripePriceId == priceId);
                        if (product is not null)
                        {
                            await ApplyCurrentCycleEntitlementsAsync(user, product.Role, periodStart, periodEnd);
                            user.EntitlementTierNext = null;
                        }

                        await _userManager.UpdateAsync(user);
                        await _context.SaveChangesAsync();
                    }
                    break;
                case "customer.subscription.updated":
                    if (stripeEvent.Data.Object is Subscription updatedSubscription)
                    {
                        var user = _userManager.Users.FirstOrDefault(u => u.StripeCustomerId == updatedSubscription.CustomerId);
                        if (user is null) break;

                        user.SubscriptionStatus = updatedSubscription.Status;

                        var item = updatedSubscription.Items?.Data?.FirstOrDefault();
                        if (item is null) break;

                        var periodStart = item.CurrentPeriodStart; // DateTime (UTC)
                        var periodEnd = item.CurrentPeriodEnd;   // DateTime (UTC)

                        var newPriceId = item.Price.Id;
                        var newProduct = await _context.Products.FirstOrDefaultAsync(p => p.StripePriceId == newPriceId);

                        // Keep dates fresh for UI regardless
                        user.CurrentPeriodStartUtc = periodStart;
                        user.CurrentPeriodEndUtc = periodEnd;

                        // If we can't resolve the product, just persist dates/status and bail
                        if (newProduct is null)
                        {
                            await _userManager.UpdateAsync(user);
                            await _context.SaveChangesAsync();
                            break;
                        }

                        var currentProduct = await _context.Products.FirstOrDefaultAsync(p => p.Role == user.SubscriptionTier);
                        var isUpgrade = currentProduct != null && newProduct.PostAllowance > currentProduct.PostAllowance;
                        var isDowngrade = currentProduct != null && newProduct.PostAllowance < currentProduct.PostAllowance;

                        // ---- FIRST SIGHT (baseline init): if we have no stored period yet, DO NOT treat as rollover ----
                        if (!user.CurrentPeriodStartUtc.HasValue || user.CurrentPeriodStartUtc.Value == default)
                        {
                            // If you want instant upgrades, you can apply them even on first sight:
                            if (isUpgrade)
                            {
                                await ApplyCurrentCycleEntitlementsAsync(user, newProduct.Role, periodStart, periodEnd);
                                user.EntitlementTierNext = null;
                            }
                            else if (isDowngrade)
                            {
                                // Schedule the downgrade for next cycle
                                user.EntitlementTierNext = newProduct.Role;
                            }

                            await _userManager.UpdateAsync(user);
                            await _context.SaveChangesAsync();
                            break;
                        }

                        // ---- REAL ROLLOVER: only when Stripe actually advances the periodStart ----
                        var isRollover = periodStart > user.CurrentPeriodStartUtc.Value;
                        if (isRollover)
                        {
                            // Apply scheduled next tier if present; otherwise apply the tier Stripe is billing now
                            var tierToApply = user.EntitlementTierNext ?? newProduct.Role;

                            await ApplyCurrentCycleEntitlementsAsync(user, tierToApply, periodStart, periodEnd);
                            user.EntitlementTierNext = null;

                            await _userManager.UpdateAsync(user);
                            await _context.SaveChangesAsync();
                            break;
                        }

                        // ---- MID-CYCLE CHANGE, no rollover ----
                        if (isUpgrade)
                        {
                            // Instant upgrades (optional)
                            await ApplyCurrentCycleEntitlementsAsync(user, newProduct.Role, periodStart, periodEnd);
                            user.EntitlementTierNext = null;
                        }
                        else if (isDowngrade)
                        {
                            // Defer downgrades
                            user.EntitlementTierNext = newProduct.Role;
                        }
                        // lateral changes: do nothing

                        await _userManager.UpdateAsync(user);
                        await _context.SaveChangesAsync();
                    }
                    break;

                case "customer.subscription.deleted":
                    if (stripeEvent.Data.Object is Subscription deletedSubscription)
                    {
                        var user = _userManager.Users.FirstOrDefault(u => u.StripeCustomerId == deletedSubscription.CustomerId);
                        if (user is not null)
                        {
                            user.SubscriptionStatus = deletedSubscription.Status;

                            // If you want to end access immediately on hard cancel:
                            if (deletedSubscription.CancelAt == null && deletedSubscription.CanceledAt != null)
                            {
                                // Hard cancel now
                                user.SubscriptionTier = UserRole.Trial;
                                user.MonthlyPostAllowance = 0;
                                user.PostsUsedThisPeriod = 0;
                                user.EntitlementTierNext = null;

                                var currentRoles = await _userManager.GetRolesAsync(user);
                                if (currentRoles.Any())
                                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                                await _userManager.AddToRoleAsync(user, UserRole.Trial.ToString());
                            }
                            // If it was "cancel at period end", do nothing here—access remains until invoice not paid at next cycle.

                            await _userManager.UpdateAsync(user);
                            await _context.SaveChangesAsync();
                        }
                    }
                    break;
                case "invoice.payment.failed":
                    // Handle payment failure
                    if(stripeEvent.Data.Object is Invoice failedInvoice)
                    {
                        var user = _userManager.Users.FirstOrDefault(u => u.StripeCustomerId == failedInvoice.CustomerId);
                        if (user is not null)
                        {
                            user.SubscriptionStatus = failedInvoice.Status;
                            await _userManager.UpdateAsync(user);
                            await _context.SaveChangesAsync();
                            Console.WriteLine($"Payment Failed for user {user.Email}");
                            // Optionally notify the user about the payment failure
                        }
                    }
                    break;
                case "invoice.paid":
                    Console.WriteLine("Invoice Paid Hit");
                    break;

                default:
                    // Unhandled event type
                    Console.WriteLine($"Unhandled event type: {stripeEvent.Type}");
                    break;
            }
            return Ok();
        }
        catch (StripeException e)
        {
            Console.WriteLine("Stripe verification failed: " + e.Message);
            return BadRequest(new { error = e.Message });
        }
    }

    private async Task UpdateUserPlanAsync(ApplicationUser user, UserRole tier)
    {
        user.SubscriptionTier = tier;

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Role == tier);
        if (product != null)
        {
            user.MonthlyPostAllowance = product.PostAllowance;

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

            await _userManager.AddToRoleAsync(user, tier.ToString());
        }
    }


    private async Task ApplyCurrentCycleEntitlementsAsync(ApplicationUser user, UserRole tier, DateTime periodStartUtc, DateTime periodEndUtc)
    {
        user.SubscriptionTier = tier;
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Role == tier);
        if(product is not null)
        {
            user.MonthlyPostAllowance = product.PostAllowance;
        }

        user.PostsUsedThisPeriod = 0;
        user.CurrentPeriodStartUtc = periodStartUtc;
        user.CurrentPeriodEndUtc = periodEndUtc;

        // Reflect current tier in roles for feature gating
        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Any())
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

        await _userManager.AddToRoleAsync(user, tier.ToString());
    }

    private void SetNextCycleTierAsync(ApplicationUser user, UserRole nextTier)
    {
        user.EntitlementTierNext = nextTier;
    }
}
