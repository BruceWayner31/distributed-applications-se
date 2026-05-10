using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static WebApplication6.Models.EventsManagment;

namespace WebApplication6.Controllers
{
    [Authorize]
    public class RegistrationsController : Controller
    {
        private readonly EventsContext _context;
        public RegistrationsController(EventsContext context)
        {
            _context = context;
        }
        // GET: Registrations
        public async Task<IActionResult> Index(
            string? search,
            string? status,
            string? sortBy,
            string? userId,
            int page = 1,
            int pageSize = 10)
        {

            var query = _context.Registrations
              .Include(r => r.Users)
              .Include(r => r.Events)
              .AsQueryable();
            if (User.IsInRole("Attendee"))
            {
                var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
                query = query.Where(r => r.UserId == currentUserId);
            }

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(r => r.Status == status);
          
            if (!string.IsNullOrWhiteSpace(userId))
                query = query.Where(r => r.UserId.ToString() == userId);
            query = sortBy switch
            {
                "UserId" => query.OrderBy(r => r.UserId),
                "TotalPrice" => query.OrderBy(r => r.TotalPrice),
                _ => query.OrderBy(r => r.UserId)
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
        //Get:/Registrations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var reg = await _context.Registrations
                .Include(r => r.Users)
                .Include(r => r.Events)
               
                 .FirstOrDefaultAsync(r => r.Id == id);
            if (reg == null) return NotFound();
            if (User.IsInRole("Attendee"))
            {
                var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
                if (reg.UserId != currentUserId) return Forbid();
            }
            return View(reg);

        }
        // GET: /Registrations/Create
        public async Task<IActionResult> Create()

        {
            if (!User.IsInRole("Admin") && !User.IsInRole("Organizer"))
            {
                var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
                ViewBag.CurrentUserId = currentUserId;
            }
            await PopulateDropdownsAsync();
            return View();
        }

        // POST: /Registrations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Registrations reg)
        {
            if (!User.IsInRole("Admin") && !User.IsInRole("Organizer"))
            {
                reg.UserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(reg);
            }
            var evnt = await _context.Events.FindAsync(reg.EventId);
            reg.RegisteredAt = DateTime.UtcNow;
            reg.TotalPrice = reg.TicketCount * evnt!.TicketPrice;
            reg.Status = "Pending";
            _context.Registrations.Add(reg);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Registration created successfully.";
            return RedirectToAction(nameof(Index));
        }
        // GET: /Registrations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var reg = await _context.Registrations.FindAsync(id);
            if (reg == null) return NotFound();

            await PopulateDropdownsAsync();
            return View(reg);
        }

        // POST: /Registrations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Registrations reg)
        {
            if (id != reg.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(reg);
            }
            var original = await _context.Registrations
        .AsNoTracking()
        .FirstOrDefaultAsync(r => r.Id == id);

            if (original == null) return NotFound();

            // ако е Attendee — провери дали е негова
            if (User.IsInRole("Attendee"))
            {
                var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
                if (original.UserId != currentUserId) return Forbid();
                reg.Status = original.Status; // не може да сменя статус
            }
            // запази полетата които не се редактират
            reg.RegisteredAt = original.RegisteredAt;

            // преизчисли TotalPrice от базата
            var evnt = await _context.Events.FindAsync(reg.EventId);
            reg.TotalPrice = reg.TicketCount * evnt!.TicketPrice;

            try
            {
                _context.Registrations.Update(reg);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Event updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Registrations.AnyAsync(r =>r.Id == id))
                    return NotFound();
                throw;
            }
           

            return RedirectToAction(nameof(Index));
        }
        // GET: /Registrations/Delete/5 — confirmation page
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var reg = await _context.Registrations
                .Include(r => r.Events)
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reg == null) return NotFound();
            if (User.IsInRole("Attendee"))
            {
                var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
                if (reg.UserId != currentUserId) return Forbid();
            }
            return View(reg);
        }

        // POST: /Registrations/Delete/5 — confirmed delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reg = await _context.Registrations.FindAsync(id);

            if (reg != null)
            {
                _context.Registrations.Remove(reg);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Event deleted.";
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

            ViewBag.Users = new SelectList(
                await _context.Users
                    .Where(u => u.Role == "Attendee" || u.Role == "Admin")
                    .OrderBy(u => u.Lname)
                    .ToListAsync(),
                "Id", "Username"
            );
        }
    }
}
