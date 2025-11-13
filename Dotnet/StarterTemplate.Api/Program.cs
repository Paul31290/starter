using StarterTemplate.Application;
using StarterTemplate.Infrastructure;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using StarterTemplate.Application.DTOs;

var builder = WebApplication.CreateBuilder(args);

if (File.Exists("appsettings.template.json"))
{
    builder.Configuration.AddJsonFile("appsettings.template.json", optional: true, reloadOnChange: true);
}
if (File.Exists($"appsettings.{builder.Environment.EnvironmentName}.template.json"))
{
    builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.template.json", optional: true, reloadOnChange: true);
}

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "StarterTemplate API",
        Version = "v1",
        Description = "A Clean Architecture starter template API with comprehensive features",
        Contact = new OpenApiContact
        {
            Name = "StarterTemplate Team",
            Email = "support@startertemplate.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    c.IgnoreObsoleteProperties();
    c.IgnoreObsoleteActions();
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});


builder.Services.AddSignalR();

builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.Configure<JwtSettingsDto>(builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettingsDto>();
if (jwtSettings != null)
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey)),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
}

builder.Services.AddAuthorization();

var allowedOrigins = "_allowedOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowedOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "StarterTemplate API v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "StarterTemplate API Documentation";
        c.DefaultModelsExpandDepth(-1);
        c.DisplayRequestDuration();
    });
}

app.UseHttpsRedirection();

app.UseIpRateLimiting();

app.UseMiddleware<StarterTemplate.Api.Middleware.GlobalExceptionHandlingMiddleware>();

app.UseMiddleware<StarterTemplate.Api.Middleware.AuditLoggingMiddleware>();

app.UseCors(allowedOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();