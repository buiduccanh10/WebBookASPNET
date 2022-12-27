using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebBookk.Data;
using WebBookk.Models;

namespace WebBookk.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;


    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;

    }

    //public ActionResult Index()
    //{
    //   var _book = getAllProduct();
    //    ViewBag.book = _book;
    //    return View();
    //}
    ////GET ALL PRODUCT
    //public List<Book> getAllProduct()
    //{
    //    return _context.Book.ToList();
    //}
    public ActionResult Index()
    {
        ProductModel p = new ProductModel();
        p.cat = _context.Category.ToList();
        p.book = _context.Book.ToList();
        return View(p);
    }

    //get category
    public ActionResult GetCategory(int? id)
    {
        ProductModel p = new ProductModel();
        p.cat = _context.Category.ToList();
        if (id == null)
        {
            p.book = _context.Book.ToList();
        }
        else
        {
            p.book = _context.Book.Where(z => z.CategoryId == id).ToList();
        }
        return View(p);
    }

    //GET DETAIL PRODUCT
    public Book getDetailBook(int id)
    {
        var book = _context.Book.Find(id);
        return book;
    }

    public async Task<IActionResult> Search(string Search)
    {
        var books = from m in _context.Book select m;

        if (!String.IsNullOrEmpty(Search))
        {
            books = books.Where(x => x.Title.Contains(Search) || x.Price.ToString().Contains(Search) ||
            x.Genre.Contains(Search));
        }
        return View(await books.ToListAsync());
    }

    // GET: Movie/Details/5
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

    //Add cart
    public IActionResult AddCart(int id)
    {
        var cart = HttpContext.Session.GetString("cart"); //get key cart
        if (cart == null)
        {
            var book = getDetailBook(id);
            List<CartItem> listcart = new List<CartItem>()
                {
                    new CartItem
                    {
                        Book = book,
                        Qty = 1
                    }
                };
            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(listcart));
        }
        else
        {
            List<CartItem> dataCart = JsonConvert.DeserializeObject<List<CartItem>>(cart);
            bool check = true;
            for (int i = 0; i < dataCart.Count; i++)
            {
                if (dataCart[i].Book.Id == id)
                {
                    dataCart[i].Qty++;
                    check = false;
                }
            }
            if (check)
            {
                dataCart.Add(new CartItem
                {
                    Book = getDetailBook(id),
                    Qty = 1
                });
            }
            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
        }
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Cart()
    {
        var cart = HttpContext.Session.GetString("cart");//get key cart
        if (cart != null)
        {
            List<CartItem> dataCart = JsonConvert.DeserializeObject<List<CartItem>>(cart);
            if (dataCart.Count > 0)
            {
                ViewBag.carts = dataCart;
                return View();
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
        return RedirectToAction(nameof(Index));
    }
    // update cart
    [HttpPost]
    public IActionResult UpdateCart(int id, int quantity)
    {
        var cart = HttpContext.Session.GetString("cart");
        if (cart != null)
        {
            List<CartItem> dataCart = JsonConvert.DeserializeObject<List<CartItem>>(cart);
            if (quantity > 0)
            {
                for (int i = 0; i < dataCart.Count; i++)
                {
                    if (dataCart[i].Book.Id == id)
                    {
                        dataCart[i].Qty = quantity;
                    }
                }

                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
            }
            var cart2 = HttpContext.Session.GetString("cart");
            return Ok(quantity);
        }
        return BadRequest();

    }
    //delete cart
    public IActionResult DeleteCart(int id)
    {
        var cart = HttpContext.Session.GetString("cart");
        if (cart != null)
        {
            List<CartItem> dataCart = JsonConvert.DeserializeObject<List<CartItem>>(cart);

            for (int i = 0; i < dataCart.Count; i++)
            {
                if (dataCart[i].Book.Id == id)
                {
                    dataCart.RemoveAt(i);
                }
            }
            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
            return RedirectToAction(nameof(Cart));
        }
        return RedirectToAction(nameof(Index));
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

