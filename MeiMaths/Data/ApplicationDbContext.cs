using MeiMaths.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MeiMaths.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<QuizResult> QuizResults { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
}
