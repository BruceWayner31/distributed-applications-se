using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Models;
using static WebApplication6.Models.EventsManagment;

namespace WebApplication6.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly EventsContext _context;
        public EventsController(EventsContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index(
            string? search,
            string? status,
            string? sortBy,
            string? sortOrder,
            int page = 1,
            int pageSize = 10)
        {

            var query = _context.Events
              .Include(e => e.Venues)
              .Include(e => e.Users)
              .AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(e => e.Title.Contains(search));

         
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(e => e.Status == status);
            query = sortBy switch
            {
                "Title" => query.OrderBy(e => e.Title),
                "Price" => query.OrderBy(e => e.TicketPrice),
                "StartDate" => query.OrderBy(e => e.StartDate),
                "Capacity" => query.OrderBy(e => e.MaxCapacity),
                _ => query.OrderBy(e => e.StartDate)
            };
            int total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.SortBy = sortBy;

            return View(items);
        }
        //Get:/Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if(id==null) return NotFound();
            var evt = await _context.Events
                .Include(e => e.Venues)
                .Include(e => e.Users)
                .Include(e => e.EventCategories)
                 .ThenInclude(ec => ec.Categories)
                 .FirstOrDefaultAsync(e => e.Id == id);
                  if (evt == null) return NotFound();
                return View(evt);

        }
        // GET: /Events/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View();
        }

        // POST: /Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Events evt, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(evt);
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(stream);

                evt.ImageUrl = "/uploads/" + fileName;
            }
            evt.CreatedAt = DateTime.UtcNow;

                _context.Events.Add(evt);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Event created successfully.";
            return RedirectToAction(nameof(Index));
        }
        // GET: /Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var evt = await _context.Events.FindAsync(id);
            if (evt == null) return NotFound();

            await PopulateDropdownsAsync();
            return View(evt);
        }

        // POST: /Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Events evt, IFormFile? imageFile)
        {
            if (id != evt.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(evt);
            }
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(stream);

                evt.ImageUrl = "/uploads/" + fileName;
            }
            else
            {
                // запази старото ImageUrl
                var original = await _context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
                evt.ImageUrl = original?.ImageUrl;
            }

            try
            {
                _context.Events.Update(evt);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Event updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Events.AnyAsync(e => e.Id == id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }
        // GET: /Events/Delete/5 — confirmation page
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var evt = await _context.Events
                .Include(e => e.Venues)
                .Include(e => e.Users)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evt == null) return NotFound();

            return View(evt);
        }

        // POST: /Events/Delete/5 — confirmed delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evt = await _context.Events.FindAsync(id);

            if (evt != null)
            {
                _context.    Events.Remove(evt);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Event deleted.";
            }

            return RedirectToAction(nameof(Index));
        }
        // reused in Create GET, Create POST (fail), Edit GET, Edit POST (fail)
        private async Task PopulateDropdownsAsync()
        {
            ViewBag.Venues = new SelectList(
                await _context.Venues.OrderBy(v => v.Name).ToListAsync(),
                "Id", "Name"
            );

            ViewBag.Organizers = new SelectList(
                await _context.Users
                    .Where(u => u.Role == "Organizer" || u.Role == "Admin")
                    .OrderBy(u => u.Lname)
                    .ToListAsync(),
                "Id", "Username"
            );
        }

    }

}

