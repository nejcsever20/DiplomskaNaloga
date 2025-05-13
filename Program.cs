using diplomska.Data;
using diplomska.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using AspNet.Security.OAuth.GitHub;

var builder = WebApplication.CreateBuilder(args);

// Configure the database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddRoleManager<RoleManager<IdentityRole>>();

// Custom services
builder.Services.AddScoped<UserApprovalService>();
builder.Services.AddScoped<CustomEmailSender>();
builder.Services.AddScoped<ISystemLogs, SystemLogs>();
builder.Services.AddSingleton<IEmailSender, DummyEmailSender>();

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Razor Pages
builder.Services.AddRazorPages();

// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
})
.AddGitHub(options =>
{
    options.ClientId = builder.Configuration["Authentication:GitHub:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"];
    options.CallbackPath = "/signin-github"; // Must match GitHub app settings
    options.Scope.Add("user:email"); // Optional: get verified email
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Role and admin setup
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string[] roleNames = { "Skladiščnik", "Izmenovodja", "Admin" };

    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    var defaultAdminEmail = "admin@yourdomain.com";
    var defaultAdminPassword = "Admin@123";

    var adminUser = await userManager.FindByEmailAsync(defaultAdminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser { UserName = defaultAdminEmail, Email = defaultAdminEmail };
        var result = await userManager.CreateAsync(adminUser, defaultAdminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthentication(); // Always before authorization
app.UseAuthorization();

app.MapRazorPages();
app.Run();
