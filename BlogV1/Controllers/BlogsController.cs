using BlogV1.Context;
using BlogV1.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogV1.Controllers
{
    public class BlogsController : Controller
    {
        private readonly BlogDbContext _context;

        public BlogsController(BlogDbContext context) 
        {
            _context = context;
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
    }
}
