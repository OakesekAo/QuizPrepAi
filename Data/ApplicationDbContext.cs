using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuizPrepAi.Models;

namespace QuizPrepAi.Data
{
    public class ApplicationDbContext : IdentityDbContext<QPUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<QuizPrepAi.Models.Quiz>? Quiz { get; set; }
        public DbSet<QuizPrepAi.Models.Question>? Question { get; set; }
    }
}