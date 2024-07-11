global using Microsoft.AspNetCore.Components.Authorization;
using BlazorApplication;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddScoped<StorageService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<PatientService>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddHttpContextAccessor();
// Configuration de l'authentification
builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("yPkCqn4kSWLtaJwvN2jGzpQRyTZ3gdkt7FeBJP"))
    };
    o.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            Console.WriteLine($"Received token: {accessToken}");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception}");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"OnChallenge: {context.Error}, {context.ErrorDescription}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri("http://host.docker.internal:5071/")
    });

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddAuthorizationCore();

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
