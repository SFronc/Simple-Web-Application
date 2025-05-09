using Microsoft.EntityFrameworkCore;
namespace WebApp.Models;

public class AppDbContext : DbContext{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

    public DbSet<User> Users {get; set;}
    public DbSet<Note> Notes {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Note>()
        .HasOne(n => n.user)
        .WithMany()
        .HasForeignKey(n => n.userId);
    }
}