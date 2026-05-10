using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Models;
using static WebApplication6.Models.EventsManagment;

namespace WebApplication6.Controllers
{
    [Authorize(Roles = "Admin,Organizer")]

    public class VenuesController : Controller
    {
        private readonly EventsContext _context;
        public VenuesController(EventsContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string? search,
            string? city,
            string? sortBy = "Name",
            int page = 1,
        int pageSize = 10)
        {
            var query = _context.Venues.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(v => v.Name.Contains(search));
            }
            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(v => v.City.Contains(city));
            }
            query = sortBy switch
            {
                "Name" => query.OrderBy(v => v.Name),
                "City" => query.OrderBy(v => v.City),
                _ => query.OrderBy(v => v.Name)
            };
            int total=await query.CountAsync();
            var items= await query
                .Skip((page-1)*pageSize)
                .Take(pageSize)
                .ToListAsync();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.Search = search;
            ViewBag.City = city;
            ViewBag.SortBy = sortBy;
            return View(items);
        }
        //Get:/Venues/Details/5
        public async Task<IActionResult> Details(int?id)
        {
            if(id==null)
            {
                return NotFound();
                
            }
            var venues = await _context.Venues
                .FirstOrDefaultAsync(v => v.Id == id);
             if (venues == null)
            {
                return NotFound();
            }
            return View(venues);

        }
        //Get:Venues/Create
        public IActionResult Create()
        {
            return View();
        }
        //Post:/Venues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Create(Venues venues)
        {
            if (!ModelState.IsValid)
            {
                return View(venues);
            }
            venues.CreatedAt = DateTime.UtcNow;
            _context.Venues.Add(venues);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Venue Created Successfully";
            return RedirectToAction(nameof(Index));
        }
        //Get:/Venues/Edit/5
        public async Task<IActionResult>Edit(int?id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var venues = await _context.Venues.FindAsync(id);
            if (venues == null)
            {
                return NotFound();
            }
            return View(venues);
        }
        //Post:/Venues/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Edit(int id,Venues venues)
        {
            if(id!=venues.Id)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View(venues);
            }
            try
            {
                _context.Venues.Update(venues);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Successfully Edit";
            }
            catch(DbUpdateConcurrencyException)
            {
                if (!await _context.Venues.AnyAsync(v => v.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }


            }
            return RedirectToAction(nameof(Index));
        }
        //Get:/Venues/Delete/5
        public async Task<IActionResult>Delete(int?id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var venues = await _context.Venues.FirstOrDefaultAsync(v => v.Id == id);
            if (venues == null)
            {
                return NotFound();
            }
            return View(venues);
        }
        //Post:/Venues/Delete/5
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>DeleteConfirmed(int id)
        {
            var venues = await _context.Venues.FindAsync(id);
            if (venues != null)
            {
                _context.Venues.Remove(venues);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Succesfully Removed";
            }
            return RedirectToAction(nameof(Index));
        }
       
    }
}
