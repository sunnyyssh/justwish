using Justwish.Users.Domain;
using Microsoft.EntityFrameworkCore;

namespace Justwish.Users.Infrastructure;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var userEntityBuilder = modelBuilder.Entity<User>();
        
        userEntityBuilder.Property(x => x.PasswordHash).IsRequired();
        userEntityBuilder.Property(x => x.Email).IsRequired();
        userEntityBuilder.Property(x => x.Username).IsRequired();
        
        userEntityBuilder.HasIndex(x => x.Email).IsUnique();
        userEntityBuilder.HasIndex(x => x.Username).IsUnique();
    }
}