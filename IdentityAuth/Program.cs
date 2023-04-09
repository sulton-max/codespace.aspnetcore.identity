using IdentityAuth.Dal.Contexts;
using IdentityAuth.Models.Configuration;
using IdentityAuth.Models.Entities;
using IdentityAuth.Services.IdentityServices;
using IdentityAuth.Services.IdentityServices.Interfaces;
using IdentityAuth.Services.NotificationServices;
using IdentityAuth.Services.NotificationServices.Interfaces;
using IdentityAuth.Services.RequestServices;
using IdentityAuth.Services.RequestServices.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddOptions<EmailSenderConfiguration>().Bind(builder.Configuration.GetSection(EmailSenderConfiguration.Position)).ValidateOnStart();
builder.Services.AddSingleton<IEmailSender, EmailSender>();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(("IdentityAuth")));

// Registering identity
builder.Services.AddIdentity<User, Role>(options =>
    {
        // Password settings
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;

        // Sign in settings
        options.SignIn.RequireConfirmedEmail = true;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(5);
        options.Lockout.MaxFailedAccessAttempts = 3;
        options.Lockout.AllowedForNewUsers = true;

        // User settings
        options.User.RequireUniqueEmail = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Registering authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
        options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/api/auth/signin";
        options.LogoutPath = "/api/auth/signout";
    });

builder.Services.AddScoped<IIdentityManagerService, IdentityManagerService>();
builder.Services.AddScoped<IAccountConfirmationService, AccountConfirmationService>();
builder.Services.AddScoped<IAuthenticationCredentialsService, AuthenticationCredentialsService>();
builder.Services.AddScoped<ITwoFactorAuthenticationService, TwoFactorAuthenticationService>();

builder.Services.AddRouting(x => x.LowercaseUrls = true);
builder.Services.AddControllers().AddMvcOptions(x => x.Filters.Add(new AuthorizeFilter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();