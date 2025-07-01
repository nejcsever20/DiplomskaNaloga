using diplomska.Data;
using diplomska.Services;
using Microsoft.AspNetCore.Authentication.Cookies;  
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Added logging
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.RequestPath | HttpLoggingFields.RequestMethod | HttpLoggingFields.RequestHeaders;

    // Log only specific headers if needed
    logging.RequestHeaders.Add("User-Agent");
    logging.RequestHeaders.Add("sec-ch-ua");

    // Do NOT log body content to limit noise
    logging.RequestBodyLogLimit = 0;
    logging.ResponseBodyLogLimit = 0;

    // Optional: combine logs into one entry per request
    logging.CombineLogs = true;
});

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

// Localization configuration
var supportedCultures = new[]
{
    "en", "sl-si", "de", "es", "fr", "sq-Al", "bg-BG", "cs-CZ", "it-IT", "mk-MK", "pl-PL", "ro-RO", "ru", "sr", "sk-SK", "tr-TR"
};
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var cultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();

    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = cultures;
    options.SupportedUICultures = cultures;

    options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
});

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
})
.AddYahoo(options =>
{
    options.ClientId = builder.Configuration["Authentication:Yahoo:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Yahoo:ClientSecret"];

})
.AddFacebook(options =>
{
    options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
    options.Scope.Add("email");// needed
    options.Fields.Add("name");// needed
    options.Fields.Add("email");// needed
    options.Fields.Add("picture"); // needed
    options.SaveTokens = true; // save access token if you want to use graph api later on
    options.CallbackPath = "/signin-facebook"; //set correct callback path
})
.AddGitLab(options =>
{
    options.ClientId = builder.Configuration["Authentication:Gitlab:ApplicationId"];
    options.ClientSecret = builder.Configuration["Authentication:GitLab:Secret"];
    options.CallbackPath = new PathString("/signin-gitlab");

    options.AuthorizationEndpoint = "https://gitlab.com/oauth/authorize";
    options.TokenEndpoint = "https://gitlab.com/oauth/token";
    options.UserInformationEndpoint = "https://gitlab.com/api/v4/user";

    options.Scope.Add("read_user"); // Required for email + profile access

    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "username");
    options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");

    options.SaveTokens = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();
var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(locOptions);
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

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

    var path = context.Request.Path;
    var method = context.Request.Method;
    var query = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : "none";
    var referer = context.Request.Headers["Referer"].ToString() ?? "none";

    logger.LogInformation("Visited: {Method} {Path}{Query} | From: {Referer}",
        method,
        path,
        query,
        referer);

    await next();
});

app.UseCookiePolicy(); 
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseHttpLogging();

app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthentication(); // Always before authorization
app.UseAuthorization();

app.MapRazorPages();
app.Run();
