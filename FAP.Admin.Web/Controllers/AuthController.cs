using FAP.Admin.Web.Infrastructure;
using FAP.Admin.Web.Services;
using FAP.Share.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace FAP.Admin.Web.Controllers;

/// <summary>
/// Controller for authentication operations.
/// Handles login and logout, storing tokens in session.
/// </summary>
public class AuthController : Controller
{
    private readonly ILogger<AuthController> _logger;
    private readonly SessionTokenStore _tokenStore;
    private readonly IAuthApiClient _authApiClient;
    private readonly IConfiguration _configuration;
    private const string PermissionsKey = "Permissions";

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    public AuthController(
        ILogger<AuthController> logger,
        SessionTokenStore tokenStore,
        IAuthApiClient authApiClient,
        IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
        _authApiClient = authApiClient ?? throw new ArgumentNullException(nameof(authApiClient));
        _configuration = configuration;
    }

    /// <summary>
    /// Initiates the FEID login process by redirecting the user to the external authentication provider.
    /// </summary>
    /// <remarks>This method signs out any existing authentication sessions before starting a new FEID login.
    /// Upon successful authentication, the user is redirected to the FeidResponse action. This endpoint is typically
    /// used to begin a single sign-on (SSO) process with an external identity provider.</remarks>
    /// <returns>A challenge result that redirects the user to the FEID authentication provider. The result initiates the
    /// external login flow.</returns>
    [HttpGet]
    [Route("FeidLogin")]
    public async Task<IActionResult> FeidLoginAsync()
    {

        ViewBag.ReCaptchaSiteKey = _configuration["RecaptchaSettings:SiteKey"];

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

        var redirectUrl = Url.Action("FeidResponse", "Login"); // Action đích sau khi đăng nhập xong
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        properties.Items["prompt"] = "login";
        return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
    }


    [HttpGet]
    public async Task<IActionResult> FeidResponse()
    {
        string accessToken = await HttpContext.GetTokenAsync("access_token");

        if (string.IsNullOrEmpty(accessToken))
        {
            ModelState.AddModelError("", "Failed to retrieve access token from FeID.");
            ViewBag.ReCaptchaSiteKey = _configuration["RecaptchaSettings:SiteKey"];
            return View("Index");
        }

        LoginFeIdRequest loginFeIdRequest = new LoginFeIdRequest
        {
            AccessToken = accessToken
        };
        var response = await _authApiClient.LoginFeIdAsync(loginFeIdRequest);
        if (response != null && !string.IsNullOrWhiteSpace(response.Token))
        {
            // Store tokens in session (API returns "Token" not "AccessToken")
            await _tokenStore.SaveTokensAsync(response.Token, response.RefreshToken);

            // Store user information in session for UI
            HttpContext.Session.SetString("UserId", response.UserId);
            HttpContext.Session.SetString("Username", response.Username);
            HttpContext.Session.SetString("Email", response.Email);
            HttpContext.Session.SetString("FullName", response.FullName);
            HttpContext.Session.SetString("CampusCode", response.CampusCode);

            // Store roles in session
            if (response.Roles != null && response.Roles.Any())
            {
                var rolesJson = JsonSerializer.Serialize(response.Roles);
                HttpContext.Session.SetString("Roles", rolesJson);
            }
           
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
        // Nếu đăng nhập không thành công, chuyển hướng về trang đăng nhập
        return RedirectToAction("Login", "Auth");
    }


    /// <summary>
    /// Displays the login page.
    /// </summary>
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        // Provide campus options for the login form (code -> display name)
        ViewBag.Campuses = new Dictionary<string, string>
        {
            { "HN", "Cơ sở Hà Nội" },
            { "HCM", "Cơ sở TP. Hồ Chí Minh" },
            { "DN", "Cơ sở Đà Nẵng" },
            { "CT", "Cơ sở Cần Thơ" },
            { "QN", "Cơ sở Quy Nhơn" }
        };
        return View();
    }

    /// <summary>
    /// Handles login submission.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(UserLoginRequest model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Campuses = new Dictionary<string, string>
            {
                { "HN", "Cơ sở Hà Nội" },
                { "HCM", "Cơ sở TP. Hồ Chí Minh" },
                { "DN", "Cơ sở Đà Nẵng" },
                { "CT", "Cơ sở Cần Thơ" },
                { "QN", "Cơ sở Quy Nhơn" }
            };
            return View(model);
        }

        try
        {
            var loginRequest = new UserLoginRequest
            {
                Username = model.Username,
                Password = model.Password,
                CampusCode = model.CampusCode
            };

            var response = await _authApiClient.LoginAsync(loginRequest);

            if (response != null && !string.IsNullOrWhiteSpace(response.Token))
            {
                // Store tokens in session (API returns "Token" not "AccessToken")
                await _tokenStore.SaveTokensAsync(response.Token, response.RefreshToken);

                // Store user information in session for UI
                HttpContext.Session.SetString("UserId", response.UserId);
                HttpContext.Session.SetString("Username", response.Username);
                HttpContext.Session.SetString("Email", response.Email);
                HttpContext.Session.SetString("FullName", response.FullName);
                HttpContext.Session.SetString("CampusCode", response.CampusCode);

                // Store roles in session
                if (response.Roles != null && response.Roles.Any())
                {
                    var rolesJson = JsonSerializer.Serialize(response.Roles);
                    HttpContext.Session.SetString("Roles", rolesJson);
                }

                _logger.LogInformation("User {Username} logged in successfully", model.Username);

                return RedirectToLocal(returnUrl);
            }

            ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
        }
        catch (HttpRequestException ex) when (ex.Data.Contains("StatusCode") && ex.Data["StatusCode"]?.ToString() == "401")
        {
            _logger.LogWarning("Unauthorized login attempt for user {Username}", model.Username);
            ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {Username}", model.Username);
            ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi đăng nhập. Vui lòng thử lại.");
        }

        return View(model);
    }

    /// <summary>
    /// Handles logout.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _tokenStore.ClearTokensAsync();

        // Clear all session data
        HttpContext.Session.Remove(PermissionsKey);
        HttpContext.Session.Remove("UserId");
        HttpContext.Session.Remove("Username");
        HttpContext.Session.Remove("Email");
        HttpContext.Session.Remove("FullName");
        HttpContext.Session.Remove("CampusCode");
        HttpContext.Session.Remove("Roles");
        HttpContext.Session.Clear();

        _logger.LogInformation("User logged out");

        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(HomeController.Index), "Home");
    }
}
