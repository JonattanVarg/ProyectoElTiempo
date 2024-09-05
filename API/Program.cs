#region Namespaces
using System.Text;
using API.Configurations.Swagger;
using API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog.Sinks.SQLite;
using Serilog;
using Serilog.Events;
using API.Data.Logging;
using API.Models.Logging;
using API.Models.Identity;
using API.Repositories.Interfaces;
using API.Repositories;
using API.Services.Interfaces;
using API.Services;
using API.Mappings;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using API.Configurations.InitialUsers;
#endregion

var builder = WebApplication.CreateBuilder(args);

// *** Configurar a Usuarios iniciales (se encuentra uno por cada rol)
builder.Services.Configure<InitialUsersConfig>(builder.Configuration.GetSection("InitialUsers"));
builder.Services.AddSingleton(provider => provider.GetRequiredService<IOptions<InitialUsersConfig>>().Value);

// Uso de Sqlite para facilitar evaluación de la prueba por su inicio rápido y sencillo
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<LogsDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("LogsConnection")));

var connectionString = builder.Configuration.GetConnectionString("LogsConnection")!;
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() 
    .WriteTo.Sink(new CustomSQLiteSink(connectionString))
    .CreateLogger();

builder.Host.UseSerilog();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

// *** Servicio de Identificación - ASP.NET CORE IDENTITY 8.0.0
builder.Services.AddIdentity<AppUser,IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Se requiere que el campo Email sea único y otras solicitudes para password
builder.Services.Configure<IdentityOptions>(options =>
{
    // Configuración de la contraseña
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Configuración del bloqueo de cuenta por intentos fallidos
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // Duración del bloqueo
    options.Lockout.MaxFailedAccessAttempts = 4; // Número máximo de intentos fallidos antes de bloquear la cuenta
    options.Lockout.AllowedForNewUsers = true; // Permitir el bloqueo para nuevos usuarios

    // Configuración de usuario
    options.User.RequireUniqueEmail = true;
});


// *** Servicio de Autenticación - ASP. NET CORE AUTHENTICATION 8.0.0
//JwtBearerDefaults.AuthenticationScheme, lo que significa que el servidor espera tokens JWT en el encabezado Authorization.

var JWTSettings = builder.Configuration.GetSection("JWTSetting");

builder.Services.AddAuthentication(opt => {
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt => {
    opt.SaveToken = true;
    opt.RequireHttpsMetadata = true;
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = JWTSettings["ValidAudience"],
        ValidIssuer = JWTSettings["ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTSettings.GetSection("securityKey").Value!))
    };
});

// ***  Configurar logging
// Agrego para eventos importantes como para limitar intentos fallidos de inicio de sesión
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSerilog();
});

// Configurar AutoMapper
builder.Services.AddAutoMapper(typeof(JobMappingsProfile));

// *** Agregar Repositorios y Servicios
builder.Services.AddScoped<IJobOfferRepository, JobOfferRepository>();
builder.Services.AddScoped<IJobOfferService, JobOfferService>(); 

builder.Services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
builder.Services.AddScoped<IJobApplicationService, JobApplicationService>();


builder.Services.AddControllers();

// *** Servicios Swagger UI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c=>{

    // Configura la información básica del documento Swagger
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

    // Configura cómo Swagger UI debe presentar el esquema de autenticación a la API y permite a los usuarios ingresar el token JWT en la interfaz de Swagger.
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
        Description=@"JWT Authorization Example : 'Bearer eyeyeyeyeyye'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    // Documenta qué esquemas de seguridad son necesarios para acceder a diferentes recursos de la API, proporcionando contexto en la documentación sobre los requisitos de seguridad.
    c.AddSecurityRequirement(new OpenApiSecurityRequirement(){
        {
        new OpenApiSecurityScheme{
            Reference = new OpenApiReference{
                Type = ReferenceType.SecurityScheme,
                Id="Bearer"
            },
            Scheme="Bearer",
            Name="Bearer",
            In=ParameterLocation.Header,
        },
        new List<string>()
        }
    });

     // Habilita las anotaciones XML para la documentación
    var xmlFile = "API.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.EnableAnnotations();
    c.SchemaFilter<LoginDtoSwaggerSchemaFilter>();
    c.SchemaFilter<RegisterDtoSwaggerSchemaFilter>();
});


// ---------------------------------------------------------------------------------------------------------------------------------

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Agrega el middleware de prueba para logging
app.Use(async (context, next) =>
{
    var logger = Log.ForContext<Program>(); // Usa Serilog.ILogger

    // Agregar información contextual
    var logContext = logger.ForContext("RequestPath", context.Request.Path)
                           .ForContext("HttpMethod", context.Request.Method)
                           .ForContext("IpAddress", context.Connection.RemoteIpAddress?.ToString());

    // Aquí puedes usar logContext para registrar eventos
    logContext.Information("Handling request");

    // Pasar al siguiente middleware
    await next();
});

app.UseHttpsRedirection();

app.UseCors(options => {
    options.AllowAnyHeader();
    options.AllowAnyMethod();
    options.AllowAnyOrigin();
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Aquí iniciado la base de datos y los datos (está ya creada la migración inicial)
// Se puede usar la APP de forma automatica con el usuario Admin
using (var scope = app.Services.CreateScope())
{
    // Se ejecutan las migraciones iniciales previamente registradas en Migrations como ".. Initial"
    // Es automatico, no se deben registrar las migraciones manualmente
    var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await appDbContext.Database.MigrateAsync();

    var logsDbContext = scope.ServiceProvider.GetRequiredService<LogsDbContext>();
    await logsDbContext.Database.MigrateAsync();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var initialUsersConfig = scope.ServiceProvider.GetRequiredService<InitialUsersConfig>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    // Se crea el usuario Admin en base de datos para que haya un usuario desde el inicio de la App
    // que pueda registrar reclutadores, que a su vez, puedan registrar candidatos
    await AppDbInitializer.Initialize(userManager, roleManager, initialUsersConfig, logger);
}

app.Run();
