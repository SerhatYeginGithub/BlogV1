using BlogV1.Context;
using BlogV1.Identity;
using BlogV1.Models;
using BlogV1.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogV1.Controllers
{
    public class BlogsController : Controller
    {
        private readonly BlogDbContext _context;
        private readonly UserManager<BlogIdentityUser> _userManager;
        private readonly SignInManager<BlogIdentityUser> _signInManager;
        public BlogsController(BlogDbContext context, SignInManager<BlogIdentityUser> signInManager, UserManager<BlogIdentityUser> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var blogs = _context.Blogs.Where(x => x.Status == 1).ToList();
            return View(blogs);
        }

        public IActionResult Details(int id)
        {
            var blog = _context.Blogs.Find(id);
            blog.ViewCount++;
            _context.SaveChanges();
            var comments = _context.Comments.Where(x => x.BlogId == id);
            ViewBag.Comments = comments.ToList();
            return View(blog);
        }
        [HttpPost]
        public IActionResult CreateComment(Comment model)
        {
            model.PublishDate = DateTime.Now;
            _context.Comments.Add(model);
            var blog = _context.Blogs.Find(model.BlogId);
            blog.CommentCount++;
            _context.SaveChanges();
            return RedirectToAction("Details", new { id = model.BlogId });
        }

        public IActionResult About()
        {
            return View();
        }


        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Support()
        {
            return View();  
        }

        [HttpPost]
        
        public IActionResult CreateContact(Contact model)
        {
            model.CreatedAt = DateTime.Now;
            _context.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
