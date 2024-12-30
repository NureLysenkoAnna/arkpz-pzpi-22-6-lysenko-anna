using GasDec.Models;
using GasDec.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net;
using System.Net.Mail;
using Microsoft.OpenApi.Models;
using MQTTnet;
using MQTTnet.Client;

var builder = WebApplication.CreateBuilder(args);

// Налаштування DbContext з використанням рядка підключення з appsettings.json
builder.Services.AddDbContext<GasLeakDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddControllers();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<LocationService>();
builder.Services.AddScoped<SensorService>();
builder.Services.AddScoped<SensorCheckService>();
builder.Services.AddScoped<SensorDataService>();
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<MqttService>();

builder.Services.AddScoped<SmtpClient>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var smtpSettings = configuration.GetSection("Smtp");

    var smtpClient = new SmtpClient(smtpSettings["Host"])
    {
        Port = int.Parse(smtpSettings["Port"]),
        Credentials = new NetworkCredential(smtpSettings["Email"], smtpSettings["Password"]),
        EnableSsl = true
    };
    return smtpClient;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(с =>
{
    с.EnableAnnotations();

    с.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введіть токен у форматі: Bearer {токен}"
    });

    с.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

var app = builder.Build();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication(); // Включення аутентифікації
app.UseAuthorization();

// Запуск MqttService
using (var scope = app.Services.CreateScope())
{
    var mqttService = scope.ServiceProvider.GetRequiredService<MqttService>();
    await mqttService.StartAsync();
}

app.MapControllers();

app.UseCors(builder =>
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader());

app.Run();
