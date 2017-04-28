namespace BlogProject.Models
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Data.Entity;

    public class BlogDbContext : IdentityDbContext<ApplicationUser>
    {
        public BlogDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Article> Articles { get; set; }

        public DbSet<ArticleComment> Comments { get; set; }

        public static BlogDbContext Create()
        {
            return new BlogDbContext();
        }
    }
}