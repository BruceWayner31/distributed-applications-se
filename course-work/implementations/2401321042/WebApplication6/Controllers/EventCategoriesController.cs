using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Models;
using static WebApplication6.Models.EventsManagment;

namespace WebApplication6.Controllers
{
    [Authorize(Roles ="Admin,Organizer")]
    public class EventCategoriesController : Controller
    {
        private readonly EventsContext _context;
        public EventCategoriesController(EventsContext context)
        {
            _context = context;
        }
        // GET: Events
        public async Task<IActionResult> Index(
            string? eventId,
            string? categoryId,
            string? sortBy,
            string? sortOrder,
            int page = 1,
            int pageSize = 10)
        {

            var query = _context.EventCategories
              .Include(ec => ec.Events)
              .Include(ec => ec.Categories)
              .AsQueryable();

            if (!string.IsNullOrWhiteSpace(eventId))
                query = query.Where(ec => ec.EventId.ToString() == eventId);


            if (!string.IsNullOrWhiteSpace(categoryId))
                query = query.Where(ec => ec.CategoryId.ToString() == categoryId);
            query = sortBy switch
            {
                "AssignedAt" => query.OrderBy(ec => ec.AssignedAt),
                
                _ => query.OrderBy(ec => ec.AssignedAt)
            };
            int total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
          
            ViewBag.SortBy = sortBy;

            return View(items);
        }
        //Get:/Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var evt = await _context.EventCategories
                .Include(ec => ec.Events)
                .Include(ec => ec.Categories)
                 .FirstOrDefaultAsync(ec => ec.Id == id);
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
        public async Task<IActionResult> Create(EventCategories ec)
        {
            bool exists = await _context.EventCategories
    .AnyAsync(x => x.EventId == ec.EventId
           && x.CategoryId == ec.CategoryId);

            if (exists)
            {
                ModelState.AddModelError("", "This category is already assigned to this event.");
                await PopulateDropdownsAsync();
                return View(ec);
            }

            ec.AssignedAt = DateTime.UtcNow;

            _context.EventCategories.Add(ec);
            await _context.SaveChangesAsync();

            TempData["Success"] = "EventCategory created successfully.";
            return RedirectToAction(nameof(Index));
        }
        // GET: /Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var ec = await _context.EventCategories.FindAsync(id);
            if (ec == null) return NotFound();

            await PopulateDropdownsAsync();
            return View(ec);
        }

        // POST: /Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventCategories ec)
        {
            if (id != ec.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(ec);
            }

            try
            {
                _context.EventCategories.Update(ec);
                await _context.SaveChangesAsync();
                TempData["Success"] = "EventCategory updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.EventCategories.AnyAsync(e => e.Id == id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }
        // GET: /Events/Delete/5 — confirmation page
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var ec = await _context.EventCategories
                .Include(e => e.Events)
                .Include(e => e.Categories)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ec == null) return NotFound();

            return View(ec);
        }

        // POST: /Events/Delete/5 — confirmed delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ec = await _context.EventCategories.FindAsync(id);

            if (ec != null)
            {
                _context.EventCategories.Remove(ec);
                await _context.SaveChangesAsync();
                TempData["Success"] = "EventCategory deleted.";
            }

            return RedirectToAction(nameof(Index));
        }
        // reused in Create GET, Create POST (fail), Edit GET, Edit POST (fail)
        private async Task PopulateDropdownsAsync()
        {

            ViewBag.Events = new SelectList(
                await _context.Events.OrderBy(e => e.Title).ToListAsync(),
                "Id", "Title"
            );

            ViewBag.Categories = new SelectList(
                await _context.Categories
                    
                    .OrderBy(c => c.Name)
                    .ToListAsync(),
                "Id", "Name"
            );
        }

    }
}

