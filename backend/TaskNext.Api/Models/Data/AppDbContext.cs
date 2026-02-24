using Microsoft.EntityFrameworkCore;
using TaskNext.Api.Models; // Namespace update kiya

namespace TaskNext.Api.Data; // Namespace update kiya

public class AppDbContext : DbContext
{
    //public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Purana constructor:
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

// Naya Temporary Constructor (Migration ke liye):
//public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

//public AppDbContext() : base(new DbContextOptionsBuilder<AppDbContext>()
   // .UseNpgsql("Host=localhost;Port=5432;Database=tasknext;Username=postgres;Password=postgres").Options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.UserId);
    }
}