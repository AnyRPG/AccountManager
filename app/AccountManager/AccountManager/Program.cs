using AccountManager.Database;
using AccountManager.Models;
using AccountManager.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// bind configuration data to settings class and add it for dependency injection
var settings = new AccountManagerSettings();
builder.Configuration.Bind("AccountManagerSettings", settings);
builder.Services.AddSingleton(settings);

// configure logging
builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

// use mysql
builder.Services.AddDbContext<GameDbContext>(o=>
    o.UseMySQL(builder.Configuration.GetConnectionString("Db"))
);

// setup controllers
builder.Services.AddControllersWithViews();

// allow this app to run in a lambda
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

// make authentication service available to dependency injection
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// configure site to use jwt for authentication
/*
builder.Services.AddAuthentication(opt => {
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //ValidateIssuer = true,
        //ValidateAudience = true,
        //ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        //ValidIssuer = ConfigurationManager.AppSetting["JWT:ValidIssuer"],
        //ValidAudience = ConfigurationManager.AppSetting["JWT:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.BearerKey))
    };
});
*/

// configure site to use cookies for authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
                options.SlidingExpiration = true;
            });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
