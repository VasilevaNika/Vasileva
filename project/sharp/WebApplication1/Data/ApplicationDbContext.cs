using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WebApplication1.Models.Entities;

namespace WebApplication1.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public override int SaveChanges()
    {
        ConvertDateTimeToUtc();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ConvertDateTimeToUtc();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ConvertDateTimeToUtc()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            foreach (var property in entry.Properties)
            {
                var propertyType = property.Metadata.ClrType;
                
                // Handle DateTime and DateTime?
                if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
                {
                    if (property.CurrentValue != null)
                    {
                        DateTime dateTime;
                        if (property.CurrentValue is DateTime dt)
                        {
                            dateTime = dt;
                        }
                        else
                        {
                            var nullableDt = property.CurrentValue as DateTime?;
                            if (nullableDt.HasValue)
                            {
                                dateTime = nullableDt.Value;
                            }
                            else
                            {
                                continue;
                            }
                        }

                        if (dateTime.Kind == DateTimeKind.Unspecified)
                        {
                            property.CurrentValue = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                        }
                        else if (dateTime.Kind == DateTimeKind.Local)
                        {
                            property.CurrentValue = dateTime.ToUniversalTime();
                        }
                        // If already UTC, leave it as is
                    }
                }
            }
        }
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }
    public DbSet<TravelJournal> TravelJournals { get; set; }
    public DbSet<Trip> Trips { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TripTag> TripTags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure table names to use lowercase (matching Liquibase schema)
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<Role>().ToTable("roles");
        modelBuilder.Entity<Permission>().ToTable("permissions");
        modelBuilder.Entity<RolePermission>().ToTable("role_permissions");
        modelBuilder.Entity<ApiKey>().ToTable("api_keys");
        modelBuilder.Entity<TravelJournal>().ToTable("travel_journals");
        modelBuilder.Entity<Trip>().ToTable("trips");
        modelBuilder.Entity<Location>().ToTable("locations");
        modelBuilder.Entity<Tag>().ToTable("tags");
        modelBuilder.Entity<TripTag>().ToTable("trip_tags");

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Username).HasColumnName("username");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasOne(e => e.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Role configuration
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Permission configuration
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // RolePermission many-to-many
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.PermissionId });
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.HasOne(e => e.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(e => e.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ApiKey configuration
        modelBuilder.Entity<ApiKey>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.KeyHash).HasColumnName("key_hash");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.HasIndex(e => e.KeyHash).IsUnique();
        });

        // TravelJournal configuration
        modelBuilder.Entity<TravelJournal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.HasOne(e => e.User)
                .WithMany(u => u.TravelJournals)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Trip configuration
        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TravelJournalId).HasColumnName("travel_journal_id");
            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.PhotoUrls)
                .HasColumnName("photos_urls")
                .HasColumnType("text[]");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.HasOne(e => e.TravelJournal)
                .WithMany(j => j.Trips)
                .HasForeignKey(e => e.TravelJournalId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Location)
                .WithMany(l => l.Trips)
                .HasForeignKey(e => e.LocationId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.ToTable(t => t.HasCheckConstraint("CK_Trip_Rating", "rating IS NULL OR (rating >= 1 AND rating <= 5)"));
        });

        // Location configuration
        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Country).HasColumnName("country");
            entity.Property(e => e.City).HasColumnName("city");
            entity.Property(e => e.Latitude).HasColumnName("latitude");
            entity.Property(e => e.Longitude).HasColumnName("longitude");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });

        // Tag configuration
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Color).HasColumnName("color");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // TripTag many-to-many
        modelBuilder.Entity<TripTag>(entity =>
        {
            entity.HasKey(e => new { e.TripId, e.TagId });
            entity.Property(e => e.TripId).HasColumnName("trip_id");
            entity.Property(e => e.TagId).HasColumnName("tag_id");
            entity.HasOne(e => e.Trip)
                .WithMany(t => t.TripTags)
                .HasForeignKey(e => e.TripId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Tag)
                .WithMany(t => t.TripTags)
                .HasForeignKey(e => e.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

