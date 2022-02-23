using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ProductsAndCategories.Models;
using Microsoft.AspNetCore.Http;

namespace ProductsAndCategories.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MyContext _context;

        public HomeController(ILogger<HomeController> logger, MyContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            List<Product> AllProducts = _context.Products.ToList();
            ViewBag.allproducts = AllProducts;
            return View();
        }
        [HttpGet]
        [Route("/Product/{productId}")]
        public IActionResult ShowProduct(int productId)
        {
            Product this_product = _context.Products.Include(a => a.Associations).ThenInclude(c => c.Categories).FirstOrDefault(p => p.ProductId == productId);
            if (this_product == null)
                return RedirectToAction("Index");

            IEnumerable<Category> UsedCategories = this_product.Associations.Select(a => a.Categories);
            // 
            IEnumerable<Category> UnusedCategories = _context.Categories
            // Including the Category Associations
                .Include(a => a.Associations)
                // Only include Categories whose associations have no productId that matches 'x' or productId for the view, wont be included in the result
                .Where(c => c.Associations.All(a => a.ProductId != productId));
            ViewBag.this_product = this_product;
            ViewBag.UsedCategories = UsedCategories;
            ViewBag.UnusedCategories = UnusedCategories;

            return View("ShowProduct");
        }
        [HttpPost]
        [Route("/")]
        public IActionResult CreateProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                _context.SaveChanges();
                return RedirectToAction("CreateProduct");
            }
            return View("Index");
        }
        [HttpPost]
        [Route("/Products/{productId}")]

        public IActionResult UpdateProduct(int productId, int CategoryId)
        {
            Product this_product = _context.Products.Include(a => a.Associations).ThenInclude(p => p.Products).FirstOrDefault(c => c.ProductId == productId);
            Boolean tf = this_product.Associations.Any(a => a.CategoryId == CategoryId);
            if (!tf)
            {
                Association connection = new Association()
                {
                    CategoryId = CategoryId,
                    ProductId = productId
                };
                this_product.Associations.Add(connection);
                _context.SaveChanges();
                return RedirectToAction("ShowProduct", new { productId = productId });
            }
            return View("Index", new { productId = productId });
        }
        [HttpGet]
        [Route("/Category")]
        public IActionResult Category()
        {
            List<Category> AllCategories = _context.Categories.ToList();
            ViewBag.allCategories = AllCategories;
            return View();
        }
        [HttpGet]
        [Route("/Category/{CategoryId}")]
        public IActionResult ShowCategory(int CategoryId)
        {
            Category this_Category = _context.Categories.Include(a => a.Associations).ThenInclude(c => c.Products).FirstOrDefault(p => p.CategoryId == CategoryId);
            if (this_Category == null)
                return RedirectToAction("Index");

            IEnumerable<Product> UsedProducts = this_Category.Associations.Select(a => a.Products);
            IEnumerable<Product> UnusedProducts = _context.Products.Include(a => a.Associations).Where(b => b.Associations.All(c => c.CategoryId != CategoryId));
            ViewBag.this_Category = this_Category;
            ViewBag.UsedProducts = UsedProducts;
            ViewBag.UnusedProducts = UnusedProducts;
            return View("ShowCategory");
        }
        [HttpPost]
        [Route("/Category")]
        public IActionResult CreateCategory(Category Category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(Category);
                _context.SaveChanges();
                return RedirectToAction("CreateCategory");
            }
            return View("Category");
        }
        [HttpPost]
        [Route("/Category/{CategoryId}")]
        public IActionResult UpdateCategory(int CategoryId, int productId)
        {
            Category this_Category = _context.Categories.Include(a => a.Associations).FirstOrDefault(c => c.CategoryId == CategoryId);
            Boolean tf = this_Category.Associations.Any(a => a.ProductId == productId);
            if (!tf)
            {
                Association connection = new Association()
                {
                    CategoryId = CategoryId,
                    ProductId = productId
                };
                this_Category.Associations.Add(connection);
                _context.SaveChanges();
                return RedirectToAction("Category", new { CategoryId = CategoryId });
            }
            return View("Category", new { CategoryId = CategoryId });
        }
    }
}
