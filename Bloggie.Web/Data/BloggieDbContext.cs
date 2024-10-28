using Microsoft.EntityFrameworkCore;
using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Data
{
    public class BloggieDbContext : DbContext
    {
        public BloggieDbContext(DbContextOptions<BloggieDbContext> options) : base(options) 
        {               
        }

        public DbSet<BlogPost> BlogPosts { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<BlogPostLike> BlogPostLikes { get; set; }

        public DbSet<BlogPostComment> BlogPostComments { get; set; }
    }
}
