using System.Threading.Tasks;
using BlogV1.Context;
using BlogV1.Identity;
using BlogV1.Models;
using BlogV1.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogV1.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly BlogDbContext _context;
        private readonly UserManager<BlogIdentityUser> _userManager;
        private readonly SignInManager<BlogIdentityUser> _signInManager;
        public AdminController(BlogDbContext context, UserManager<BlogIdentityUser> userManager, SignInManager<BlogIdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            var dashboard = new DashboardViewModel();

            var toplamblogsayisi = _context.Blogs.Count();
            var toplamgoruntulenme = _context.Blogs.Select(x => x.ViewCount).Sum();
            var encokgoruntulneneblog = _context.Blogs.OrderByDescending(x => x.ViewCount).FirstOrDefault();
            var ensonyayinlananblog = _context.Blogs.OrderByDescending(x => x.PublishDate).FirstOrDefault();
            var toplamyorumsayisi = _context.Comments.Count();
            var encokyorumalanblogId = _context.Comments
                                        .GroupBy(x => x.BlogId) // BlogId'ye göre grupla
                                        .OrderByDescending(g => g.Count()) // Grupları yorum sayısına göre azalan sırala
                                        .Select(g => g.Key) // En çok yorumu olan BlogId'yi al
                                        .FirstOrDefault(); // İlk sonucu getir
            var encokyorumalanblog = _context.Blogs.Where(x => x.Id == encokyorumalanblogId).FirstOrDefault();

            var bugunyapilanyorumsayisi = _context.Comments.Where(x => x.PublishDate.Date == DateTime.Now.Date).Count();

            dashboard.TotalBlogCount = toplamblogsayisi;
            dashboard.TotalViewCount = toplamgoruntulenme;
            dashboard.MostViewedBlog = encokgoruntulneneblog;
            dashboard.LatestBlog = ensonyayinlananblog;
            dashboard.TotalCommentCount = toplamyorumsayisi;
            dashboard.MostCommentedBlog = encokyorumalanblog;
            dashboard.TodayCommentCount = bugunyapilanyorumsayisi;

            return View(dashboard);
        }

        public IActionResult Blogs()
        {
            var blogs = _context.Blogs.ToList();
            return View(blogs);
        }


        public IActionResult EditBlog(int id)
        {
            var blog = _context.Blogs.Find(id);
            return View(blog);
        }

        public IActionResult DeleteBlog(int id)
        {
            var blog = _context.Blogs.Find(id);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
                _context.SaveChanges();
                return RedirectToAction("Blogs");
            } else
            {
                return NotFound("Not found");
            }
               
        }

        [HttpPost]
        public IActionResult EditBlog(Blog model)
        {
            var blog = _context.Blogs.Find(model.Id);
            blog.Name = model.Name;
            blog.Description = model.Description;
            blog.Tags = model.Tags;
            blog.ImageUrl = model.ImageUrl;

            _context.SaveChanges();

            return RedirectToAction("Blogs");
        }
        public IActionResult ToggleStatus(int id)
        {
            var blog = _context.Blogs.Find(id);
            if (blog != null)
            {
                if (blog.Status == 1)
                {
                    blog.Status = 0;
                }
                else
                {
                    blog.Status = 1;
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Blogs");
        }



        public IActionResult CreateBlog()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateBlog(Blog model)
        {
            model.PublishDate = DateTime.Now;
            model.Status = 1;
            _context.Blogs.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Blogs");
        }

        public IActionResult Comments(int? blogId)
        {
            var comments = new List<Comment>();
            if (blogId != null && blogId != 0)
            {
                comments = _context.Comments.Where(x => x.BlogId == blogId).ToList();
            }
            return View(comments);
        }

        public IActionResult DeleteComment(int id)
        {
            var comment = _context.Comments.Find(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                _context.SaveChanges();
            }

            return RedirectToAction("Comments");
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (model.Password != model.RePassword)
            {
                return View();
            }

            var user = new BlogIdentityUser
            {
                Name = model.Name,
                Surname = model.Surname,
                Email = model.Email,
                UserName = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if(result.Succeeded)
            {
                return RedirectToAction("Index");
            } else
            {
                return View();
            }
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Blogs");
        }
    }
}
