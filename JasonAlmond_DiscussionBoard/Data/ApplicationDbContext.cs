using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using JasonAlmond_DiscussionBoard.Models;

namespace JasonAlmond_DiscussionBoard.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly IConfiguration _config;

    public DbSet<DiscussionThread> DiscussionThreads { get; set; }
    public DbSet<Post> Posts { get; set; }

    // Inject both DbContext options and IConfiguration
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration config)
        : base(options)
    {
        _config = config;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Need a configuration object
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite(_config.GetConnectionString("DefaultConnection"));
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Create identity models
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<DiscussionThread>()
            .Property(e => e.Timestamp)
            .IsConcurrencyToken();
        modelBuilder.Entity<Post>()
            .Property(e => e.Timestamp)
            .IsConcurrencyToken();
        
        /*foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(EntityBase).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.Name)
                    .Property<long>("Timestamp")
                    .IsConcurrencyToken();
            }
        }*/
        
        modelBuilder.Entity<DiscussionThread>()
            .HasIndex(x => x.CreatedAt)
            .IsUnique();
        modelBuilder.Entity<Post>()
            .HasIndex(x => new { x.ApplicationUserId, x.CreatedAt })
            .IsUnique();

    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
        foreach (var entry in entries)
        {
            entry.Property("Timestamp").CurrentValue = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        try
        {
            return base.SaveChanges();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            foreach (var entry in ex.Entries)
            {
                var proposedValues = entry.CurrentValues;
                var databaseValues = entry.GetDatabaseValues();
                bool identicalValues = true;
                foreach (var property in proposedValues.Properties)
                {
                    var proposedValue = proposedValues[property];
                    var databaseValue = databaseValues[property];
                    if (!proposedValue.Equals(databaseValue))
                    {
                        identicalValues = false;
                        break;
                    }
                }
                if (identicalValues) //Values were assigned the same values, false alarm.
                {
                    return base.SaveChanges();
                }
                //Refresh original values to bypass next concurrency check
                entry.OriginalValues.SetValues(databaseValues);
            }
            throw;
        }

    }
}