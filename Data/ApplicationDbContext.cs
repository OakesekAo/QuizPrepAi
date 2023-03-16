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
    }
}