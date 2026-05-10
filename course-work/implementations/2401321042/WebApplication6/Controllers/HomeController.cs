using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApplication6.Models;
using static WebApplication6.Models.EventsManagment;

namespace WebApplication6.Controllers
{
    public class HomeController : Controller
    {
        private readonly EventsContext _context;

        public HomeController(EventsContext context )
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // вземи предстоящите published events
            var events = await _context.Events
                .Include(e => e.Venues)
                .Include(e => e.EventCategories)
                    .ThenInclude(ec => ec.Categories)
                .Where(e => e.Status == "Published" && e.EndDate >= DateTime.UtcNow)
                .OrderBy(e => e.StartDate)
                .Take(9)
                .ToListAsync();

            return View(events);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int status = 500, string? message = null)
        {
            var response = new ErrorResponse
            {
                Status = status,
                Title = status switch
                {
                    404 => "Not Found",
                    400 => "Bad Request",
                    401 => "Unauthorized",
                    _ => "Internal Server Error"
                },
                Detail = message ?? "An unexpected error occurred.",
                Instance = HttpContext.Request.Path
            };
            return View(response);
        }


    }
}
