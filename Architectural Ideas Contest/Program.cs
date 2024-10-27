using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Architectural_Ideas_Contest.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>() // Changed from AddDefaultIdentity to AddIdentity
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders(); // Add default token providers if needed for email confirmation, password reset, etc.

// Add other services if needed, e.g., Razor Pages or MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Enable Authentication
app.UseAuthorization();  // Enable Authorization

app.MapControllers();
app.MapRazorPages(); // Ensure Razor pages are mapped if you're using Identity pages

app.Run();
