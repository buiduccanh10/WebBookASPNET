using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBookk.Data;
using WebBookk.Models;

namespace WebBookk.Data
{

    [Authorize(Roles = "Admin")]
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _iWebHost;

        public BookController(IWebHostEnvironment iwebhost, ApplicationDbContext context)
        {
            _context = context;
            _iWebHost = iwebhost;
        }

        public ActionResult Index()
        {
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "CategoryId", "CategoryId");
            ViewData["CategoryName"] = new SelectList(_context.Set<Category>(), "CategoryName", "CategoryName");
            ProductModel p = new ProductModel();
            p.cat = _context.Category.ToList();
            p.book = _context.Book.ToList();
            return View(p);
        }

        // GET: Book
        public async Task<IActionResult> Search(string Search)
        {
            var books = from m in _context.Book select m;

            if (!String.IsNullOrEmpty(Search))
            {
                books = books.Where(x => x.Title.Contains(Search) || x.Price.ToString().Contains(Search) ||
                x.Genre.Contains(Search) || x.CategoryName.ToString().Contains(Search));
            }
            /*return View(await _context.Movie.ToListAsync());*/
            return View(await books.ToListAsync());
        }

        // GET: Book/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Book/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "CategoryId", "CategoryId");
            ViewData["CategoryName"] = new SelectList(_context.Set<Category>(), "CategoryName", "CategoryName");
            return View();
        }

        // POST: Book/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ReleaseDate,Price,Genre,CategoryId,CategoryName,Image,ImageDescription,Qty")] Book book)
        {
            if (ModelState.IsValid)
            {
                if (book.Image != null)
                {
                    string? uniqueFileName = null;
                    //combine array of string into a path
                    string ImageUploadFolder = Path.Combine(_iWebHost.WebRootPath, "images");
                    // GUID to uniquely identify file name
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + book.Image.FileName;
                    string filepath = Path.Combine(ImageUploadFolder, uniqueFileName);
                    //file stream allow work with file read, write
                    using (var fileStream = new FileStream(filepath, FileMode.Create))
                    {
                        book.Image.CopyTo(fileStream);
                    }
                    book.BookImagePath = "~/wwwroot/images";
                    book.BookFileName = uniqueFileName;
                }
                _context.Add(book);
                SetAlert("Book created successfull!", "success");
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "CategoryId", "CategoryId", book.CategoryId);
            //ViewData["CategoryName"] = new SelectList(_context.Set<Category>(), "CategoryName", "CategoryName", book.CategoryName);
            return View(book);
        }

        // GET: Book/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["CategoryName"] = new SelectList(_context.Set<Category>(), "CategoryName", "CategoryName", book.CategoryName);
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "CategoryId", "CategoryId", book.CategoryId);
            return View(book);
        }

        // POST: Book/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,ReleaseDate,Price,Genre,CategoryId,CategoryName,Image,ImageDescription,Qty")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string? uniqueFileName = null;
                    if (book.Image != null)
                    {
                        //combine array of string into a path
                        string ImageUploadFolder = Path.Combine(_iWebHost.WebRootPath, "images");
                        // GUID to uniquely identify file name
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + book.Image.FileName;
                        string filepath = Path.Combine(ImageUploadFolder, uniqueFileName);
                        //file stream allow work with file read, write
                        using (var fileStream = new FileStream(filepath, FileMode.Create))
                        {
                            book.Image.CopyTo(fileStream);
                        }
                        book.BookImagePath = "~/wwwroot/images";
                        book.BookFileName = uniqueFileName;
                    }
                    _context.Update(book);
                    SetAlert("Book edited successfull!", "success");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
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

            return View(book);
        }

        // GET: Book/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Book == null)
            {
                return Problem("Entity set 'BookContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
                SetAlert("Book removed successfull!", "warning");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return (_context.Book?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        //Show alert
        protected void SetAlert(string message, string type)
        {
            TempData["AlertMessage"] = message;
            if (type == "success")
            {
                TempData["AlertType"] = "alert-success";
            }
            else if (type == "warning")
            {
                TempData["AlertType"] = "alert-warning";
            }
        }
    }
}

