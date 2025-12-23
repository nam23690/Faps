using System.Diagnostics;
using FAP.Admin.Web.Infrastructure;
using FAP.Share.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FAP.Admin.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SessionTokenStore _tokenStore;

    public HomeController(ILogger<HomeController> logger, SessionTokenStore tokenStore)
    {
        _logger = logger;
        _tokenStore = tokenStore;
    }

    public async Task<IActionResult> Index()
    {
        // Check if user is logged in
        var accessToken = await _tokenStore.GetAccessTokenAsync();
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            // Not logged in, redirect to login
            return RedirectToAction(nameof(AuthController.Login), "Auth");
        }

        // User is logged in, show home page
        ViewData["Username"] = HttpContext.Session.GetString("Username");
        ViewData["FullName"] = HttpContext.Session.GetString("FullName");
        ViewData["Email"] = HttpContext.Session.GetString("Email");
        
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
