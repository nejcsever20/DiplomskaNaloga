using diplomska.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Google;
using diplomska.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure the database context (ApplicationDbContext)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services with IdentityUser
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddRoleManager<RoleManager<IdentityRole>>();  // Add RoleManager service if you plan to use roles

builder.Services.AddScoped<UserApprovalService>();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() // Allow any origin
              .AllowAnyMethod()  // Allow any HTTP method
              .AllowAnyHeader(); // Allow any header
    });
});

// Add Razor Pages
builder.Services.AddRazorPages();

// Add Google Authentication
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    });

builder.Services.AddSingleton<IEmailSender, DummyEmailSender>();

builder.Services.AddScoped<CustomEmailSender>();

var app = builder.Build();

// Ensure roles are created when the application starts
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();  // Use IdentityUser here

    // Define roles you want to create
    string[] roleNames = { "Skladiščnik", "Izmenovodja", "Admin" };

    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Create a default Admin user (if needed)
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

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable CORS
app.UseCors("AllowAll");

app.UseAuthentication(); // This should come before UseAuthorization()
app.UseAuthorization();

app.MapRazorPages();
app.Run();
