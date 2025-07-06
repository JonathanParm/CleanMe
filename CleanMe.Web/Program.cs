using CleanMe.Application.Services;
using CleanMe.Domain.Entities;
using CleanMe.Infrastructure.Data;
using CleanMe.Infrastructure.Repositories;
using CleanMe.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CleanMe.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;
using CleanMe.Application.Interfaces;
using CleanMe.Web.Middlewares;
using CleanMe.Infrastructure.Logging;
using Serilog;
using Serilog.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Ensure Serilog is configured before adding other services
builder.Host.UseSerilog((context, config) =>
{
    config
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day);
});

// Ensure Logging is correctly added
builder.Services.AddSingleton<Serilog.Extensions.Hosting.DiagnosticContext>();

// Add Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//// use built-in Identity UI pages
//builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddRoles<IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>();

// Set login path for unauthenticated users
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Add controllers with views
builder.Services.AddControllersWithViews();

// Add Razor Pages (needed for Identity UI)
builder.Services.AddRazorPages();

// Register EF Core Repository
builder.Services.AddScoped(serviceType: typeof(IRepository<>), implementationType: typeof(Repository<>));

// Register Dapper Repository with IDbConnection
builder.Services.AddScoped(typeof(IDapperRepository), typeof(DapperRepository));
builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IErrorLoggingService, ErrorLoggingService>();
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<IDapperRepository, DapperRepository>();

builder.Services.AddScoped<IAmendmentService, AmendmentService>();
builder.Services.AddScoped<IAmendmentRepository, AmendmentRepository>();
builder.Services.AddScoped<IAmendmentTypeService, AmendmentTypeService>();
builder.Services.AddScoped<IAmendmentTypeRepository, AmendmentTypeRepository>();
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IAssetLocationService, AssetLocationService>();
builder.Services.AddScoped<IAssetLocationRepository, AssetLocationRepository>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<IItemCodeService, ItemCodeService>();
builder.Services.AddScoped<IItemCodeRepository, ItemCodeRepository>();
builder.Services.AddScoped<IItemCodeRateService, ItemCodeRateService>();
builder.Services.AddScoped<IItemCodeRateRepository, ItemCodeRateRepository>();
builder.Services.AddScoped<ICleanFrequencyService, CleanFrequencyService>();
builder.Services.AddScoped<ICleanFrequencyRepository, CleanFrequencyRepository>();
builder.Services.AddScoped<IClientContactService, ClientContactService>();
builder.Services.AddScoped<IClientContactRepository, ClientContactRepository>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<ICompanyInfoRepository, CompanyInfoRepository>();
builder.Services.AddScoped<ICompanyInfoService, CompanyInfoService>();
builder.Services.AddScoped<IRegionService, RegionService>();
builder.Services.AddScoped<IRegionRepository, RegionRepository>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ISettingService, SettingService>();
builder.Services.AddScoped<ISettingRepository, SettingRepository>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ILookupService, LookupService>();

builder.Services.AddScoped<zzIReportOutputService, zzReportOutputService>();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

var app = builder.Build();

// Enable request logging. Ensure logs are flushed when the app exits
app.UseSerilogRequestLogging();

// Run Admin Seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await ApplicationDbContext.SeedAdminUser(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    //app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Register our custom middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Ensure authentication and authorization are applied
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
{
    FileName = "http://localhost:5000",
    UseShellExecute = true
});

app.Run();
