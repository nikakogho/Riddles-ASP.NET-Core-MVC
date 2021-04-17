using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Riddles.Models;

namespace Riddles.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Riddle>().HasIndex(r => r.Question).IsUnique();

            builder.Entity<SolvingStatus>().HasKey(ss => new { ss.RiddleID, ss.UserID });

            builder.Entity<SolvingStatus>().HasOne(ss => ss.Riddle).WithMany(r => r.Solvers)
                .HasForeignKey("RiddleID").OnDelete(DeleteBehavior.Restrict);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseLazyLoadingProxies();
        }

        public DbSet<Riddle> Riddle { get; set; }
        public DbSet<SolvingStatus> SolvingStatuses { get; set; }
    }
}
