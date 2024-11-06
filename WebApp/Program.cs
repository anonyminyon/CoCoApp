using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using COCOApp.Models;
using Newtonsoft.Json;
using COCOApp.Services;
using Microsoft.Extensions.Configuration;
using COCOApp.Repositories;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using COCOApp.Repositories.Implementation;
using COCOApp.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Get the connection string from configuration
string connectionStr = builder.Configuration.GetConnectionString("MyConStr");

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

builder.Services.AddDbContext<StoreManagerContext>(opt =>
    opt.UseSqlServer(connectionStr));

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

// Correctly get the email settings from the configuration
var emailSettings = builder.Configuration.GetSection("EmailSettings").Get<EmailSettings>();
builder.Services.AddSingleton(emailSettings);
builder.Services.AddTransient<EmailService>();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IExportOrderRepository, ExportOrderRepository>();
builder.Services.AddScoped<IImportOrderRepository, ImportOrderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IUserDetailRepository, UserDetailRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IExportOrderItemRepository,ExportOrderItemRepository>();
builder.Services.AddScoped<ICategoryRepository,CategoryRepository>();
builder.Services.AddScoped<IProductStatisticsRepository, ProductStatisticsRepository>();
builder.Services.AddScoped<IImportOrderItemRepository,ImportOrderItemRepository>();
builder.Services.AddScoped<IInventoryManagementRepository, InventoryManagementRepository>();
builder.Services.AddScoped<ISellerDetailRepository, SellerDetailRepository>();
builder.Services.AddScoped<ICategoryRepository,CategoryRepository>();
builder.Services.AddScoped<IProductStatisticsRepository, ProductStatisticsRepository>();

// Register your custom services here
builder.Services.AddScoped<ExportOrderService>();
builder.Services.AddScoped<ImportOrderService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<SupplierService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<UserDetailsService>();
builder.Services.AddScoped<ExportOrderItemService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ProductStatisticService>();
builder.Services.AddScoped<ImportOrderItemService>();
builder.Services.AddScoped<InventoryMangementService>();
builder.Services.AddScoped<SellerDetailsService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ProductStatisticService>();

// Configure SignalR to handle cyclic references
builder.Services.AddSignalR().AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    options.PayloadSerializerOptions.WriteIndented = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/ViewSignIn"; // Redirect to login page
        options.LogoutPath = "/Account/Logout"; // Redirect to logout page
        options.AccessDeniedPath = "/Home/AccessDenied"; // Redirect to access denied page
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("SellerOnly", policy => policy.RequireRole("Seller"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=ViewSignIn}");
    endpoints.MapHub<ProductHub>("/productHub");
    endpoints.MapHub<CustomerHub>("/customerHub");
    endpoints.MapHub<SupplierHub>("/supplierHub");
    endpoints.MapHub<OrderHub>("/orderHub");
    endpoints.MapHub<CategoryHub>("/categoryHub");
});
app.Run();
