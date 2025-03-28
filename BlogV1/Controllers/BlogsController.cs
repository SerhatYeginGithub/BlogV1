using BlogV1.Context;
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
    }
}
