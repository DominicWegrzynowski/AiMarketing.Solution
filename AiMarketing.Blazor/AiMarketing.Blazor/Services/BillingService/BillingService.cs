using AiMarketing.Blazor.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using AiMarketing.Blazor.Models.Enums;

namespace AiMarketing.Blazor.Services.BillingService
{
    public class BillingService : IBillingService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        public BillingService(UserManager<ApplicationUser> userManager, IConfiguration config, ApplicationDbContext context)
        {
            _userManager = userManager;
            _config = config;
            _context = context;
        }

        public async Task<string?> CreatePortalSessionAsync(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);
            if (user is null) return null;

            var returnUrl = _config["App:Domain"] ?? "https://localhost:7078";

            var options = new Stripe.BillingPortal.SessionCreateOptions
            {
                Customer = user.StripeCustomerId,
                ReturnUrl = returnUrl,
                Locale = "auto",
            };

            var session = await new Stripe.BillingPortal.SessionService().CreateAsync(options);
            return session.Url;
        }
        public async Task<string?> StartCheckoutAsync(string productId, ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);
            if (user == null) return null;

            if (string.IsNullOrEmpty(user.StripeCustomerId))
            {
                var customer = await new CustomerService().CreateAsync(new CustomerCreateOptions
                {
                    Email = user.Email,
                    Name = user.FullName,
                });

                user.StripeCustomerId = customer.Id;
                await _userManager.UpdateAsync(user);
                Console.WriteLine("customer created");
            }

            var domain = _config["App:Domain"] ?? "https://localhost:7078";

            var product = await _context.Products.FirstOrDefaultAsync(p => p.StripeProductId == productId);
            if (product == null || string.IsNullOrEmpty(product.StripeProductId))
            {

                Console.WriteLine($"Product: {product}, product.StripeProductId: {product!.StripeProductId}");
                Console.WriteLine("product is null or product.stripeproductid is null or empty");
                return null;
            }
                

            if (!string.IsNullOrEmpty(user.StripeCustomerId))
            {
                Console.WriteLine("user.StripeCustomerId is not null");
                var subService = new SubscriptionService();
                var existingSubs = await subService.ListAsync(new SubscriptionListOptions
                {
                    Customer = user.StripeCustomerId,
                    Status = "active",
                    Limit = 1
                });

                if (existingSubs.Data.Count > 0)
                {
                    Console.WriteLine("existingSubs.Data.Count > 0");
                    // Sync local DB if missing
                    var sub = existingSubs.Data.First();
                    if (string.IsNullOrEmpty(user.StripeSubscriptionId))
                    {
                        Console.WriteLine("user.StripeSubscriptionId is null or empty, " + sub.Id + " assigned to StripeSubscriptionId");
                        user.StripeSubscriptionId = sub.Id;
                        await _userManager.UpdateAsync(user);
                    }

                    // You can also log or throw a specific flag so the UI knows to redirect to portal
                    Console.WriteLine($"User {user.Email} already has an active subscription: {sub.Id}");
                    return null; // signals "already subscribed"
                }
            }

            var session = await new SessionService().CreateAsync(new SessionCreateOptions
            {
                Customer = user.StripeCustomerId,
                PaymentMethodTypes = new List<string> { "card" },
                Locale = "auto",
                LineItems = new List<SessionLineItemOptions>
            {
                new() { Price = product.StripePriceId, Quantity = 1 }
            },
                Mode = "subscription",
                SuccessUrl = $"{domain}/home",
                CancelUrl = $"{domain}/home",
                Metadata = new Dictionary<string, string> 
                {
                    { "plan", product.Role.ToString() }
                }
            });

            return session.Url;
        }
        public async Task DowngradeExpiredUsersAsync()
        {
            await Task.CompletedTask;
            //var expiredUsers = await _context.Users
            //    .Where(u => u.AllowanceEnd < DateTime.Now &&
            //                u.SubscriptionTier != UserRole.Trial)
            //    .ToListAsync();

            //foreach(var user in expiredUsers)
            //{
            //    user.SubscriptionTier = UserRole.Trial;
            //    user.MonthlyPostAllowance = 3;
            //    user.SubscriptionStatus = "canceled";

            //    var currentRoles = await _userManager.GetRolesAsync(user);
            //    if(currentRoles.Any())
            //    {
            //        await _userManager.RemoveFromRolesAsync(user, currentRoles);
            //    }

            //    await _userManager.AddToRoleAsync(user, UserRole.Trial.ToString());
            //}

            //await _context.SaveChangesAsync();
        }

        public async Task ResetTrialAllowancesAsync()
        {
            var trialUsers = await _context.Users
                .Where(u => u.SubscriptionTier == UserRole.Trial)
                .ToListAsync();

            foreach (var user in trialUsers)
            {
                user.MonthlyPostAllowance = 3;
            }

            await _context.SaveChangesAsync();
        }
        public async Task CheckAndHandleSubscriptionExpirationAsync(ApplicationUser user)
        {
            await Task.CompletedTask;
            //if (user.AllowanceEnd < DateTime.UtcNow && user.SubscriptionTier != UserRole.Trial)
            //{
            //    user.SubscriptionTier = UserRole.Trial;
            //    user.MonthlyPostAllowance = 3;
            //    user.SubscriptionStatus = "canceled";

            //    var currentRoles = await _userManager.GetRolesAsync(user);
            //    if (currentRoles.Any())
            //        await _userManager.RemoveFromRolesAsync(user, currentRoles);

            //    await _userManager.AddToRoleAsync(user, UserRole.Trial.ToString());

            //    await _userManager.UpdateAsync(user);
            //    await _context.SaveChangesAsync();
            //}
        }
        public async Task SyncSubscriptionFromStripe(ApplicationUser user)
        {
            if (string.IsNullOrEmpty(user.StripeCustomerId))
                return;

            var subService = new SubscriptionService();
            var subs = await subService.ListAsync(new SubscriptionListOptions
            {
                Customer = user.StripeCustomerId,
                Status = "active",
                Limit = 1
            });

            var active = subs.Data.FirstOrDefault();
            if (active is null)
            {

                user.StripeSubscriptionId = null;
                user.SubscriptionStatus = null;
                user.CurrentPeriodStartUtc = null;
                user.CurrentPeriodEndUtc = null;
                user.SubscriptionTier = UserRole.Trial;

                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Any())
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, UserRole.Trial.ToString());

                await _userManager.UpdateAsync(user);
                await _context.SaveChangesAsync();
                return;
            }

            user.StripeSubscriptionId = active.Id;
            user.SubscriptionStatus = active.Status;

            var item = active.Items?.Data?.FirstOrDefault();
            if (item != null)
            {
                var periodStart = item.CurrentPeriodStart;
                var periodEnd = item.CurrentPeriodEnd;

                user.CurrentPeriodStartUtc = periodStart;
                user.CurrentPeriodEndUtc = periodEnd;

                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.StripePriceId == item.Price.Id);

                if (product is not null)
                {
                    user.SubscriptionTier = product.Role;
                    user.MonthlyPostAllowance = product.PostAllowance;
                }
            }

            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();
        }

    }
}
