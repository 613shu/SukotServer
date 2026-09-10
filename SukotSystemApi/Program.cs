using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SukotSystemApi.Middlewares;
using SukotSystemCore.DTOs;
using SukotSystemCore.Repositories;
using SukotSystemCore.Services;
using SukotSystemData;
using SukotSystemService;
using System.Text;
using SukotSystemData.Repositories;

var builder = WebApplication.CreateBuilder(args);


// Local dev only: the React client runs on a different origin/port
// (Vite's dev server) than this API, so the browser blocks the
// requests unless the server explicitly allows that origin.
builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientDev", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://localhost:4173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Tell Swagger this API uses a JWT bearer token, so it renders an
    // "Authorize" button in the UI instead of having no place at all
    // to attach a token to a request.
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "הדבקו רק את הטוקן עצמו (ללא המילה Bearer) - Swagger מוסיף אותה בעצמו."
    });

    // This is what actually makes Swagger send the Authorization header on
    // every request once you click "Authorize" and paste the token in -
    // without it, the button can exist but nothing gets attached.
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    JwtBearerDefaults.AuthenticationScheme;
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
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new
        SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
    };
});
builder.Services.AddDbContext<DataContex>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});
builder.Services.AddScoped<ICustomerRepository, CustometRepository>();
builder.Services.AddScoped<IRabbiRepository, RabbiRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<ISecretaryRepository, SecretaryRepository>();

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IRabbiService, RabbiService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<ISecreratyService,SecretaryService>();

// Drop the default console provider so every log line is written exactly once,
// through NLog (per nlog.config), instead of once from each provider.
builder.Logging.ClearProviders();
builder.Logging.AddNLog();
var app = builder.Build();

// Pipeline order matters (requirement 10): error handling wraps EVERYTHING
// below it, so it must be registered first. CorrelationId comes right after,
// so the id exists before any other middleware/controller tries to log.
app.UseGlobalExceptionHandling();
app.UseCorrelationId();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Must run before Authentication/Authorization, and before MapControllers.
app.UseCors("ClientDev");

// Authentication (who are you) must run before Authorization (what are you
// allowed to do) - Authorization reads the identity Authentication sets up.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
