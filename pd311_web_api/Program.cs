using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using pd311_web_api.BLL;
using pd311_web_api.BLL.DTOs.Account;
using pd311_web_api.BLL.Services.Account;
using pd311_web_api.BLL.Services.Cars;
using pd311_web_api.BLL.Services.Email;
using pd311_web_api.BLL.Services.Image;
using pd311_web_api.BLL.Services.JwtService;
using pd311_web_api.BLL.Services.Manufactures;
using pd311_web_api.BLL.Services.Role;
using pd311_web_api.BLL.Services.User;
using pd311_web_api.DAL;
using pd311_web_api.DAL.Repositories.Cars;
using pd311_web_api.DAL.Repositories.JwtRepository;
using pd311_web_api.DAL.Repositories.Manufactures;
using pd311_web_api.Middlewares;
using Serilog;
using System.Text;
using static pd311_web_api.DAL.Entities.IdentityEntities;

var builder = WebApplication.CreateBuilder(args);

// Logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log_.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            RequireExpirationTime = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"] ?? "")),
            ClockSkew = TimeSpan.Zero
        };
    });

// Add services to the container.
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IManufactureService, ManufactureService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Add repositories
builder.Services.AddScoped<IManufactureRepository, ManufactureRepository>();
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<IJwtRepository, JwtRepository>();

builder.Services.AddControllers();

// Add fluent validation
builder.Services.AddValidatorsFromAssemblyContaining<LoginValidator>();

// Add automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// Add database context
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql("name=NpgsqlLocal");
});

// Add identity
builder.Services
    .AddIdentity<AppUser, AppRole>(options =>
    {
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredUniqueChars = 0;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("localhost3000", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

// JWT bearer authorization
builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "PD311_API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new []{Settings.RoleAdmin}
        }
    });
});

var app = builder.Build();

// Middlewares
app.UseMiddleware<MiddlewareExceptionHandler>();
app.UseMiddleware<MiddlewareNullExceptionHandler>();
app.UseMiddleware<MiddlewareLogger>();

app.UseHttpsRedirection();

app.UseCors("localhost3000");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

Settings.RootPath = builder.Environment.ContentRootPath;
string rootPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot");
string imagesPath = Path.Combine(rootPath, Settings.RootImagesPath);

if(!Directory.Exists(rootPath))
{
    Directory.CreateDirectory(rootPath);
}

if (!Directory.Exists(imagesPath))
{
    Directory.CreateDirectory(imagesPath);
}

// static files
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagesPath),
    RequestPath = "/images"
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();