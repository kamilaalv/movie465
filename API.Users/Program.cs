using APP.Users;
using APP.Users.Domain;
using APP.Users.Features;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults (if using Aspire)
// builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<UsersDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UsersConnection")));

// Add MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(UsersDb).Assembly);
});

// APP SETTINGS
var appSettingsSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettingsSection);

// Create singleton instance of AppSettings for injection
var appSettings = appSettingsSection.Get<AppSettings>();
builder.Services.AddSingleton(appSettings);

// Add UsersDbHandler
builder.Services.AddScoped<UsersDbHandler>();

// AUTHENTICATION
var key = Encoding.ASCII.GetBytes(appSettings.Secret);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true
    };
});

// SWAGGER
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// Add authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map default endpoints (if using Aspire)
// app.MapDefaultEndpoints();

// Initialize the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<UsersDb>();
        dbContext.Database.Migrate();
        await SeedUsers(dbContext);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

app.Run();

// Seed initial users and roles
async Task SeedUsers(UsersDb context)
{
    // Only seed if there are no roles
    if (!await context.Roles.AnyAsync())
    {
        // Add roles
        var adminRole = new APP.Users.Domain.Role { Name = "Admin" };
        var userRole = new APP.Users.Domain.Role { Name = "User" };

        context.Roles.Add(adminRole);
        context.Roles.Add(userRole);
        await context.SaveChangesAsync();

        // Add admin user
        var adminUser = new APP.Users.Domain.User
        {
            UserName = "admin",
            Password = "admin123", // In a real app, hash this password
            Name = "Admin",
            Surname = "User",
            IsActive = true,
            RegistrationDate = DateTime.UtcNow,
            RoleId = adminRole.Id
        };

        context.Users.Add(adminUser);
        await context.SaveChangesAsync();

        // Add a few skills
        var skills = new[]
        {
            new APP.Users.Domain.Skill { Name = "C#" },
            new APP.Users.Domain.Skill { Name = "JavaScript" },
            new APP.Users.Domain.Skill { Name = "SQL" },
            new APP.Users.Domain.Skill { Name = "Azure" }
        };

        context.Skills.AddRange(skills);
        await context.SaveChangesAsync();
    }
}