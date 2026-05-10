using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication6.Models;
using static WebApplication6.Models.EventsManagment;

namespace WebApplication6.Controllers
{
    public class AuthController : Controller
    {
        private readonly EventsContext _context;
        public AuthController(EventsContext context)
            {
            _context = context;
            }

        //GET:/Auth/Login
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        //POST:/Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Login(string username,string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password,user.Password))
            {
                ViewBag.Error = "Invalid username or password";
                return View();
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.Username),
                new Claim(ClaimTypes.Role,user.Role),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())

            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            TempData["Success"] = $"Welcome {user.Username}";
            return RedirectToAction("Index", "Home");
        }
        //POST:/Auth/Logout
        public async Task<IActionResult>Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        //GET:/Auth/Register
        public IActionResult Register()
        {
            return View();
        }
        //POST:/Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Register(string username,string email,string password,string firstname,string lastname)
        {
            if (await _context.Users.AnyAsync(u => u.Username == username))
            {
                ViewBag.Error = "Username already taken";
                return View();
            }
            if(await _context.Users.AnyAsync(u=>u.Email==email))
            {
                ViewBag.Error = "Email already taken";
                return View();
            }
            var user = new Users
            {
                Username = username,
                Email=email,
                Password=BCrypt.Net.BCrypt.HashPassword(password),
                Fname=firstname,
                Lname=lastname,
                Role="Attendee",
                CreatedAt=DateTime.UtcNow

            };
            _context.Add(user);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Account created! Please Login";
            return RedirectToAction("Login");
        }
        //GET:/Auth/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }

       
    }
}
