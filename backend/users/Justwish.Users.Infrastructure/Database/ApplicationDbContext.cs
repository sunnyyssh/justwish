using Justwish.Users.Domain;
using Microsoft.EntityFrameworkCore;

namespace Justwish.Users.Infrastructure;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }

    public DbSet<User> Users => Set<User>();

    public DbSet<ProfilePhoto> ProfilePhotos => Set<ProfilePhoto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var userEntity = modelBuilder.Entity<User>();
        
        userEntity.Property(x => x.PasswordHash).IsRequired();
        userEntity.Property(x => x.Email).IsRequired();
        userEntity.Property(x => x.Username).IsRequired();
        
        userEntity.HasIndex(x => x.Email).IsUnique();
        userEntity.HasIndex(x => x.Username).IsUnique();

        userEntity.Property(x => x.Gender).HasConversion<string>();

        userEntity.Property(x => x.SocialLinks).HasColumnType("jsonb");

        var photoEntity = modelBuilder.Entity<ProfilePhoto>();

        photoEntity.Property(x => x.Data).IsRequired();
        photoEntity.Property(x => x.ContentType).IsRequired();

        photoEntity.Ignore(x => x.IsShared);
    }
}