using AiMarketing.Blazor.Services.BillingService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AiMarketing.Blazor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IBillingService _billingService;
        private readonly IConfiguration _config;

        public AdminController(IBillingService billingService, IConfiguration config)
        {
            _billingService = billingService;
            _config = config;
        }

        [HttpPost("monthly-reset")]
        public async Task<IActionResult> MonthlyReset([FromHeader(Name = "Authorization")] string? authHeader)
        {
            var secretToken = _config["ResetApiToken"];
            if (authHeader != $"Bearer {secretToken}")
            {
                return Unauthorized("Invalid token");
            }

            await _billingService.ResetTrialAllowancesAsync();

            return Ok("Monthly reset completed.");
        }
    }
}
