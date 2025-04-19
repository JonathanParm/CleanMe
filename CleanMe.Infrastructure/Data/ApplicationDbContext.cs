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

    public DbSet<Staff> Staff { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<ClientContact> ClientContacts { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<Area> Areas { get; set; }
    public DbSet<CleanFrequency> CleanFrequencies { get; set; }
    public DbSet<AssetType> AssetTypes { get; set; }
    public DbSet<AssetLocation> AssetLocations { get; set; }

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
            .HasMany(c => c.ClientContacts)
            .WithOne(cc => cc.Client)
            .HasForeignKey(cc => cc.clientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Region>()
            .HasMany(r => r.Areas)
            .WithOne(a => a.Region)
            .HasForeignKey(a => a.regionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Area>()
            .HasMany(a => a.AssetLocations)
            .WithOne(al => al.Area)
            .HasForeignKey(al => al.areaId)
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