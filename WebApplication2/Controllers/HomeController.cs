using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        ApplicationContext db;
        public HomeController(ApplicationContext context)
        {
            db = context;
        }

        //public async Task<IActionResult> Index()
        //{
        //    return View(await db.Books.ToListAsync());
        //}

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Book book)
        {
            db.Books.Add(book);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                Book? user = await db.Books.FirstOrDefaultAsync(p => p.Id == id);
                if (user != null)
                {
                    db.Books.Remove(user);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }

        public async Task<IActionResult> Index(SortState sortOrder = SortState.TitleAsc)
        {
            IQueryable<Book>? books = db.Books; 
            ViewData["TitleSort"] = sortOrder == SortState.TitleAsc ? SortState.TitleDesc : SortState.TitleAsc;
            ViewData["AutorSort"] = sortOrder == SortState.AutorAsc ? SortState.AutorDesc : SortState.AutorAsc;
            ViewData["YearSort"] = sortOrder == SortState.YearAsc ? SortState.YearDesc : SortState.YearAsc;
 
            books = sortOrder switch
            {
                SortState.TitleDesc => books.OrderByDescending(s => s.Title),
                SortState.AutorAsc => books.OrderBy(s => s.Autor),
                SortState.AutorDesc => books.OrderByDescending(s => s.Autor),
                SortState.YearAsc => books.OrderBy(s => s.Year),
                SortState.YearDesc => books.OrderByDescending(s => s.Year),
                _ => books.OrderBy(s => s.Title),
            };
            return View(await books.AsNoTracking().ToListAsync());
        }
    }
}