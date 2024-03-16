using Instagram.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Instagram.Database;

public class InstagramDbContext : IdentityDbContext<User>
{
    public DbSet<User>? User { get; set; }
    public DbSet<Post>? Posts { get; set; }
    public DbSet<Like>? Likes { get; set; }
    public DbSet<Comment>? Comments { get; set; }

    public InstagramDbContext(DbContextOptions<InstagramDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Post>()
            .HasMany(p => p.Likes)
            .WithOne(l => l.Post)
            .HasForeignKey(l => l.PostId);

        builder.Entity<Post>()
            .HasMany(p => p.Comments)
            .WithOne(c => c.Post)
            .HasForeignKey(c => c.PostId);

        builder.Entity<User>()
            .HasMany(u => u.Posts)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.Entity<User>()
            .HasMany(u => u.Likes)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.Entity<User>()
            .HasMany(u => u.Comments)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.Entity<User>()
            .HasMany(u => u.Subscribed)
            .WithMany(u => u.Subscribers);
        
        base.OnModelCreating(builder);
    }
}