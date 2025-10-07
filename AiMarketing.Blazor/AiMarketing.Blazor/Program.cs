using AiMarketing.Blazor.Components;
using AiMarketing.Blazor.Components.Account;
using AiMarketing.Blazor.Data;
using AiMarketing.Blazor.Services.AllowanceService;
using AiMarketing.Blazor.Services.BillingService;
using AiMarketing.Blazor.Services.BusinessProfileService;
using AiMarketing.Blazor.Services.EmailSender;
using AiMarketing.Blazor.Services.EmailService;
using AiMarketing.Blazor.Services.OpenAiService;
using AiMarketing.Blazor.Services.PostService;
using AiMarketing.Blazor.Services.ProductService;
using AiMarketing.Blazor.Services.SeedService;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

// **Remove the manual AddAuthentication() and AddIdentityCookies() calls.**
// Identity automatically configures the scheme.
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true;
}).AddEntityFrameworkStores<ApplicationDbContext>()
  .AddSignInManager()
  .AddDefaultTokenProviders();  // Full Identity setup including RoleManager and UserManager



var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Register SeedService after Identity services
builder.Services.AddScoped<ISeedService, SeedService>();  // Registering ISeedService with the interface

// General services
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IBusinessProfileService, BusinessProfileService>();
builder.Services.AddScoped<IAllowanceService, AllowanceService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IBillingService, BillingService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailSender<ApplicationUser>, IdentityEmailSender>();
builder.Services.AddControllers();

// Register OpenAI service
var openAiApiKey = builder.Configuration["OpenAI:ApiKey"];
builder.Services.AddSingleton<IOpenAiService>(provider => new OpenAiService(openAiApiKey!));

// Stripe configuration
var stripeApiSecretKey = builder.Configuration["Stripe:SecretKey"];
Stripe.StripeConfiguration.ApiKey = stripeApiSecretKey;

var app = builder.Build();

// Seed the roles during startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    var services = scope.ServiceProvider;
    var seedService = services.GetRequiredService<ISeedService>();

    await seedService.SeedRolesAsync();  // Seeding roles
    await seedService.SeedProductsAsync(db);  // Seeding products
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
