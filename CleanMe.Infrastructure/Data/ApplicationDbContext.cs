using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CleanMe.Shared.Models;
using CleanMe.Domain.Entities;
using CleanMe.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CleanMe.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Amendment> Amendments { get; set; }
    public DbSet<AmendmentType> AmendmentTypes { get; set; }
    public DbSet<Area> Areas { get; set; }
    public DbSet<AssetLocation> AssetLocations { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<AssetType> AssetTypes { get; set; }
    public DbSet<AssetTypeRate> AssetTypeRates { get; set; }
    public DbSet<CleanFrequency> CleanFrequencies { get; set; }
    public DbSet<ClientContact> ClientContacts { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<StockCode> StockCodes { get; set; }
    public DbSet<Staff> Staff { get; set; }

    public DbSet<ErrorExceptionsLog> ErrorExceptionsLogs { get; set; } = null!;

    // Add DbSets for other entities if needed

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Define relationship between Staff and ApplicationUser
        builder.Entity<Staff>()
            .HasOne<ApplicationUser>()
            .WithOne()
            .HasForeignKey<Staff>(s => s.ApplicationUserId)
            .IsRequired(false); // Optional login account

        // Configure Address as an Owned Type (Embedded Value Object)
        builder.Entity<Staff>().OwnsOne(s => s.Address, address =>
        {
            address.Property(a => a.Line1).HasColumnName("Address_Line1");
            address.Property(a => a.Line2).HasColumnName("Address_Line2");
            address.Property(a => a.Suburb).HasColumnName("Address_Suburb");
            address.Property(a => a.TownOrCity).HasColumnName("Address_TownOrCity");
            address.Property(a => a.Postcode).HasColumnName("Address_Postcode");
        });

        builder.Entity<Staff>(entity =>
        {
            entity.Property(s => s.WorkRole)
                  .HasConversion(new EnumToStringConverter<WorkRole>()) // Store WorkRole as string
                  .HasMaxLength(50);
        });

        // Configure Address as an Owned Type (Embedded Value Object)
        builder.Entity<Client>().OwnsOne(c => c.Address, address =>
        {
            address.Property(a => a.Line1).HasColumnName("Address_Line1");
            address.Property(a => a.Line2).HasColumnName("Address_Line2");
            address.Property(a => a.Suburb).HasColumnName("Address_Suburb");
            address.Property(a => a.TownOrCity).HasColumnName("Address_TownOrCity");
            address.Property(a => a.Postcode).HasColumnName("Address_Postcode");
        });

        builder.Entity<Client>()
            .HasMany(p => p.ClientContacts)
            .WithOne(c => c.Client)
            .HasForeignKey(c => c.clientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Client>()
            .HasMany(p => p.Assets)
            .WithOne(c => c.Client)
            .HasForeignKey(c => c.clientId)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Region>()
            .HasMany(c => c.Areas)
            .WithOne(p => p.Region)
            .HasForeignKey(p => p.regionId)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Area>()
            .HasMany(c => c.AssetLocations)
            .WithOne(p => p.Area)
            .HasForeignKey(p => p.areaId)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<AssetLocation>()
            .HasMany(c => c.Assets)
            .WithOne(p => p.AssetLocation)
            .HasForeignKey(p => p.assetLocationId)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<AssetType>()
            .HasMany(c => c.Assets)
            .WithOne(p => p.AssetType)
            .HasForeignKey(p => p.assetTypeId)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<AssetType>()
            .HasMany(c => c.AssetTypeRates)
            .WithOne(p => p.AssetType)
            .HasForeignKey(p => p.assetTypeId)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Client>()
            .HasMany(c => c.AssetTypeRates)
            .WithOne(p => p.Client)
            .HasForeignKey(p => p.clientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CleanFrequency>()
            .HasMany(c => c.AssetTypeRates)
            .WithOne(p => p.CleanFrequency)
            .HasForeignKey(p => p.cleanFrequencyId)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Client>()
            .HasMany(c => c.Amendments)
            .WithOne(p => p.Client)
            .HasForeignKey(p => p.clientId)
            .IsRequired(false)                     // marks the FK as optional
            .OnDelete(DeleteBehavior.Restrict);    // prevents cascade deletes

        builder.Entity<AssetLocation>()
            .HasMany(c => c.Amendments)
            .WithOne(p => p.AssetLocation)
            .HasForeignKey(p => p.assetLocationId)
            .IsRequired(false)                     // marks the FK as optional
            .OnDelete(DeleteBehavior.Restrict);    // prevents cascade deletes

        builder.Entity<Area>()
            .HasMany(c => c.Amendments)
            .WithOne(p => p.Area)
            .HasForeignKey(p => p.areaId)
            .IsRequired(false)                     // marks the FK as optional
            .OnDelete(DeleteBehavior.Restrict);    // prevents cascade deletes

        builder.Entity<Asset>()
            .HasMany(c => c.Amendments)
            .WithOne(p => p.Asset)
            .HasForeignKey(p => p.assetId)
            .IsRequired(false)                     // marks the FK as optional
            .OnDelete(DeleteBehavior.Restrict);    // prevents cascade deletes

        builder.Entity<Staff>()
            .HasMany(c => c.Amendments)
            .WithOne(p => p.Staff)
            .HasForeignKey(p => p.staffId)
            .IsRequired(false)                     // marks the FK as optional
            .OnDelete(DeleteBehavior.Restrict);    // prevents cascade deletes

        builder.Entity<CleanFrequency>()
            .HasMany(c => c.Amendments)
            .WithOne(p => p.CleanFrequency)
            .HasForeignKey(p => p.cleanFrequencyId)
            .IsRequired(false)                     // marks the FK as optional
            .OnDelete(DeleteBehavior.Restrict);    // prevents cascade deletes


        builder.Entity<AmendmentType>()
            .HasMany(p => p.Amendments)
            .WithOne(c => c.AmendmentType)
            .HasForeignKey(c => c.amendmentTypeId)
            .IsRequired(true)                     
            .OnDelete(DeleteBehavior.Restrict);

        // Configure ErrorExceptionsLog table (if needed)
        builder.Entity<ErrorExceptionsLog>()
            .Property(e => e.Message)
            .HasMaxLength(2000);

        // Configure Address as an Owned Type (Embedded Value Object)
        builder.Entity<AssetLocation>().OwnsOne(c => c.Address, address =>
        {
            address.Property(a => a.Line1).HasColumnName("Address_Line1");
            address.Property(a => a.Line2).HasColumnName("Address_Line2");
            address.Property(a => a.Suburb).HasColumnName("Address_Suburb");
            address.Property(a => a.TownOrCity).HasColumnName("Address_TownOrCity");
            address.Property(a => a.Postcode).HasColumnName("Address_Postcode");
        });

    }

    public static async Task SeedAdminUser(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            string adminEmail = "aparminter.ap@gmail.com";
            string adminPassword = "Admin123*";

            // Check if the role exists, otherwise create it
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Check if an Admin user exists
            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin == null)
            {
                // Create ApplicationUser
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    // Assign role
                    await userManager.AddToRoleAsync(adminUser, "Admin");

                    // Create Staff entry linked to ApplicationUser
                    var adminStaff = new Staff
                    {
                        StaffNo = 331,
                        FirstName = "Annette",
                        FamilyName = "Parminter",
                        WorkRole = WorkRole.Admin,
                        ApplicationUserId = adminUser.Id,
                        AddedAt = DateTime.Now,
                        AddedById = "1",
                        UpdatedAt = DateTime.Now,
                        UpdatedById = "1"
                    };

                    dbContext.Staff.Add(adminStaff);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}