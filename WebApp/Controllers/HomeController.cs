using COCOApp.Helpers;
using COCOApp.Models;
using COCOApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace COCOApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserService _userService;

        public HomeController(ILogger<HomeController> logger, UserService userService)
        {
            _logger = logger;
            _userService = userService;
        }
        [Authorize(Roles = "Admin,Seller")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult ViewRegisterStore()
        {
            return View("/Views/Home/RegisterStore.cshtml");
        }
        public async Task<IActionResult> ViewSignIn()
        {
            User authenticatedUser = await _userService.CheckRememberMeTokenAsync();

            if (authenticatedUser != null)
            {
                // Create claims for the authenticated user
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, authenticatedUser.Username),
            new Claim(ClaimTypes.Email, authenticatedUser.Email),
            new Claim(ClaimTypes.Role, GetRoleName((int)authenticatedUser.Role)) // Convert role ID to role name
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // Sign in the user with the claims
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                if (authenticatedUser.Role == 2) // Seller
                {
                    // Store authenticated user in session
                    HttpContext.Session.SetObjectInSession("user", authenticatedUser);
                    return RedirectToAction("Index", "Home");
                }
                else if (authenticatedUser.Role == 1) // Admin
                {
                    int sellerId = authenticatedUser.Id;
                    HttpContext.Session.SetObjectInSession("sellerId", sellerId);
                    authenticatedUser.Id = 0;
                    // Store authenticated user in session
                    HttpContext.Session.SetObjectInSession("user", authenticatedUser);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View("/Views/Home/SignIn.cshtml");
                }
            }
            else
            {
                // Clear any lingering authentication state if the user is not authenticated
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return View("/Views/Home/SignIn.cshtml");
            }
        }

        // Helper method to convert role ID to role name
        private string GetRoleName(int roleId)
        {
            return roleId switch
            {
                1 => "Admin",
                2 => "Seller",
                _ => "Customer"
            };
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
