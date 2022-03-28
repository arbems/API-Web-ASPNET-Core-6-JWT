using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalAPIASPNETCore6JWT;
using MinimalAPIASPNETCore6JWT.Authorization;
using MinimalAPIASPNETCore6JWT.Identity;
using MinimalAPIASPNETCore6JWT.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppIdentityDbContext>(options => options.UseInMemoryDatabase("TestDatabase"))
    .AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppIdentityDbContext>();

builder.Services.AddScoped<ITokenClaimsService, IdentityTokenClaimService>();

builder.Services
    .AddAuthorization()
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
        };
    });

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo JSON Web Tokens Minimal API ASPNET Core 6", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Introduce un token válido",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.Logger.LogInformation("Seeding Database...");
await AppIdentityDbContextSeed.SeedAsync(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/currentUser", async (HttpContext context, ITokenClaimsService tokenClaimsService) =>
{
    return context.User.Identity.IsAuthenticated ?
            await UserInfo.CreateUserInfo(context.User, await tokenClaimsService.GetTokenAsync(context.User.Identity.Name))
            : UserInfo.Anonymous;
})
.RequireAuthorization();

app.MapPost("/authenticate", async ([FromBody] AuthenticateRequest request,
    UserManager<ApplicationUser> userManager,
    ITokenClaimsService tokenClaimsService) =>
{
    var response = new AuthenticateResponse();

    var user = await userManager.FindByNameAsync(request.UserName);

    var succeeded = await userManager.CheckPasswordAsync(user, request.Password);

    if (succeeded)
    {
        response.Succeeded = succeeded;
        response.Token = await tokenClaimsService.GetTokenAsync(request.UserName);
    }

    return response;
});

app.Run();