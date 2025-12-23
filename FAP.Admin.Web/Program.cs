using FAP.Admin.Web.Extensions;
using FAP.Admin.Web.Infrastructure;
using FAP.Admin.Web.Services;
using FAP.Shared.Http.Abstractions;
using FAP.Shared.Http.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure session for token storage
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// Add HTTP context accessor for session access
builder.Services.AddHttpContextAccessor();

// Register SessionTokenStore both as concrete type and as ITokenStore implementation
builder.Services.AddScoped<SessionTokenStore>();
builder.Services.AddScoped<ITokenStore, SessionTokenStore>(sp => sp.GetRequiredService<SessionTokenStore>());

// Get API base address from configuration
var apiBaseAddress = builder.Configuration["ApiSettings:BaseAddress"]
    ?? throw new InvalidOperationException("ApiSettings:BaseAddress is not configured in appsettings.json");

// Register shared HTTP client infrastructure
builder.Services.AddSharedHttpClients(apiBaseAddress);
// Register authentication API client and all other API clients automatically
// Scans the assembly where API client implementations live and registers typed clients.
builder.Services.AddAuthenticatedHttpClient<IAuthApiClient, AuthApiClient>(apiBaseAddress); // keep explicit for auth client
// Auto-register other API clients found in the FAP.Admin.Web.Services assembly
builder.Services.AddAllAuthenticatedHttpClients(apiBaseAddress, typeof(UserManagementApiClient).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Enable session middleware (must be before UseRouting)
app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
