using Data;
using Identity.Seeds;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Models.Identity;
using Models.Seeds;
using Models.Settings;
using Serilog;
using Services.Account;
using Services.File;
using Services.Payment;
using Services.Repos;
using Stripe;
using System.Configuration;
using System.Security.Claims;
using System.Text;
using WebApi.Middlewares;
using AccountService = Services.Account.AccountService;

var builder = WebApplication.CreateBuilder(args);

// Configure logging using Serilog
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));


// Add services to the container.
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // Include XML comments for Swagger/OpenAPI documentation
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "WebApiUdemy.xml"));

    // Configure security for Swagger/OpenAPI
    options.AddSecurityDefinition("WebApiAuthentication", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Description = "Enter a valid JWT token"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement{
    {
        new OpenApiSecurityScheme {
            Reference = new OpenApiReference {
                Type = ReferenceType.SecurityScheme,
                Id = "WebApiAuthentication"
            } },
        new List<string>()
    }});
});

// Configure services
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWTSettings"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));
// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Services
#if DEBUG
builder.Services.AddTransient<IEmailService, MockEmailService>();
#else
builder.Services.AddTransient<IEmailService, EmailService>();
#endif
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IFilesService, FilesService>();
builder.Services.AddScoped<IPaymentService, PaymentService>(); //TTODO
// UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 5;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

//Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWTSettings:Issuer"],
        ValidAudience = builder.Configuration["JWTSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTSettings:Key"]!)),
        RoleClaimType = ClaimTypes.Role
    };
});


// Add Policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Admin", "SuperAdmin");
    });
});



// Configure database context
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Configure options for the IISServer
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = int.MaxValue;
});

// Configure options for the KestrelServer
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue; // if don't set default value is: 30 MB
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();


//This middleware registers a custom error handler middleware to handle exceptions in the application
app.UseCors(x => x.AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials()
    .WithExposedHeaders("x-pagination", "Authorization"));


app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapControllers();

// Create a new scope for resolving dependencies
using var scope = app.Services.CreateScope();
var Services = scope.ServiceProvider;

var userManager = Services.GetRequiredService<UserManager<ApplicationUser>>();
var roleManager = Services.GetRequiredService<RoleManager<ApplicationRole>>();

await DefaultRoles.SeedAsync(roleManager);
await DefaultSuperAdmin.SeedAsync(userManager);

app.Run();
