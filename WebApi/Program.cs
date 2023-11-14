using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using WebStore.Data.Context;
using WebStore.Data.Entitties.Identity;
using WebStore.Data.Seeder;
using WebStore.Mapper;
using WebStore.Repository.Security;
using WebStore.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebStore.Business_logic.Authentication;
using WebStore.Business_logic.Category;
using WebStore.Business_logic.Files;
using WebStore.Repository.User;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "You api title", Version = "v1" });
    c.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter into field the word 'Bearer' following by space and JWT",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Name = "Bearer",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

builder.Services.AddCors();

builder.Services.AddAutoMapper(typeof(MapperProfile));

// Database context
var conStr = builder.Configuration.GetConnectionString("WebStoreConnection");
Console.WriteLine(conStr);
builder.Services.AddDbContext<StoreDbContext>(opts => opts.UseNpgsql(conStr));

// Identity configurations
builder.Services.AddIdentity<UserEntity, RoleEntity>(opts =>
{
    opts.Stores.MaxLengthForKeys = 512;
    // Password requirements
    opts.Password.RequiredLength = 8;
    opts.Password.RequireDigit = true;
    opts.Password.RequireUppercase = true;
    opts.Password.RequireLowercase = true;
    opts.Password.RequireNonAlphanumeric = true;
}).AddEntityFrameworkStores<StoreDbContext>().AddDefaultTokenProviders();

// Auth configurations
var keyBytes = Encoding.UTF8.GetBytes(builder.Configuration["JwtSecretKey"]);
SymmetricSecurityKey signingKey = new(keyBytes);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = signingKey,
        ValidateAudience = false, // on production make true
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtIssuer"],
        ClockSkew = TimeSpan.Zero
    };
    cfg.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context => {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
            }
            return Task.CompletedTask;
        }
    };
});

// JWT token service injection
builder.Services.AddScoped<IJwtTokenService, JwtTokenServiceImpl>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();

// Other services injection
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPictureService, LocalPictureService>();

// Configure https on production
if (!builder.Environment.IsDevelopment())
    builder.WebHost.ConfigureKestrel(s =>
    {
        string certPath = builder.Configuration["CertPath"];
        var certPem = File.ReadAllText(Path.Combine(certPath, "fullchain.pem"));
        var keyPem = File.ReadAllText(Path.Combine(certPath, "privkey.pem"));
        var x509 = X509Certificate2.CreateFromPem(certPem, keyPem);

        s.ListenAnyIP(443, opts => opts.UseHttps(x509));
    });

var app = builder.Build();

app.UseCors(policyBuilder => policyBuilder
    .AllowAnyHeader()
    .AllowAnyOrigin()
    .AllowAnyMethod()
);

await app.SeedDatabase();

var path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
if (!Directory.Exists(path)) Directory.CreateDirectory(path);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(path),
    RequestPath = "/uploads",
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStatusCodePages(async context =>
{
    context.HttpContext.Response.ContentType = MediaTypeNames.Text.Plain;
    await context.HttpContext.Response.WriteAsync(
        $"{context.HttpContext.Response.StatusCode} Error");
});

app.UseHsts();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();