using System.IdentityModel.Tokens.Jwt;
using ClaimsManagement.Models.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();
builder.Services.AddRouting(x => x.LowercaseUrls = true);
builder.Services.AddAuthentication(options => { options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; })
    .AddCookie(options =>
    {
        options.LoginPath = "/signin";
        options.LogoutPath = "/signout";
    })
    .AddGitHub(options =>
    {
        options.ClientId = builder.Configuration["github:clientId"]!;
        options.ClientSecret = builder.Configuration["github:clientSecret"]!;
        options.CallbackPath = "/signin-github";

        options.Scope.Add("read:user");

        options.Events.OnCreatingTicket += context =>
        {
            if (context.AccessToken is { })
                context.Identity?.AddClaim(new("access_token", context.AccessToken));

            return Task.CompletedTask;
        };
    })
    .AddOpenIdConnect(options =>
    {
        options.SignInScheme = "Cookies";
        options.Authority = "https://github.com";
        options.RequireHttpsMetadata = true;
        options.ClientId = builder.Configuration["github:clientId"]!;
        options.ClientSecret = builder.Configuration["github:clientSecret"]!;
        options.ResponseType = "code";
        options.UsePkce = true;
        options.Scope.Add("profile");
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.ClaimActions.MapUniqueJsonKey("preferred_username", "preferred_username");
    });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();