using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Migrations;
using WebApplication6.Models;
using static WebApplication6.Models.EventsManagment;

namespace WebApplication6.Controllers
{
    [Authorize(Roles = "Admin")]

    public class CategoriesController : Controller
    {
        private readonly EventsContext _context;

        public CategoriesController(EventsContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string? search,
            string? isActive,
            string? sortBy="Name",
            int page=1,
            int pageSize=10)
        {
            //Get:/Categories
           var query = _context.Categories.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
               query = query.Where(c => c.Name.Contains(search));
            }
            if(isActive=="Active")
            {
                query = query.Where(c => c.isActived);
            }
            else if(isActive=="Inactive")
            {
                query = query.Where(c => !c.isActived);
            }

            query = sortBy switch
            {
                "Name" => query.OrderBy(c => c.Name),
                "CreatedAt" => query.OrderBy(c => c.Created),
                _ => query.OrderBy(c => c.Name)
            };
            int total=await query.CountAsync();
            var items= await query
                .Skip((page-1)*pageSize)
                .Take(pageSize)
                .ToListAsync();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.Search = search;
            ViewBag.isActive=isActive;
            ViewBag.SortBy = sortBy;

            return View(items);
        }
        //Get:/Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }
        //Get:/Categories/Create
        public IActionResult Create()
        {
            return View();
        }
        //Post:/Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categories category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            category.Created = DateTime.UtcNow;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            TempData["SuccesMSG"] = "Category Created";
            return RedirectToAction(nameof(Index));

        }
        //Get:/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        { 
        if(id==null)
            {   
                return NotFound();
            }
        var category=await _context.Categories.FindAsync(id);
            if(category==null)
            {
                return NotFound();
            }
            return View(category);


        }
        //Post:/Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Categories category)
        {
            if (id != category.Id)
            {
              return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            try
            {
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMSG"] = "Category Updated";
            }
            catch (DbUpdateConcurrencyException)
            {
                if(!await _context.Categories.AnyAsync(c=>c.Id==id))
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
        //Get:/Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }
            var category=await _context.Categories
                .FirstOrDefaultAsync(c=>c.Id==id);
            if (category == null)
            {
                return NotFound();
            }
                return View(category);
        }
        //Post:/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if(category!=null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMSG"] = "Category Deleted";
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
