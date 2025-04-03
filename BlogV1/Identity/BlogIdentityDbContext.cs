using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BlogV1.Identity
{
    public class BlogIdentityDbContext: IdentityDbContext<BlogIdentityUser, BlogIdentityRole, string>
    {
        public BlogIdentityDbContext(DbContextOptions<BlogIdentityDbContext> options) :base(options)
        {

        }

    }
}
