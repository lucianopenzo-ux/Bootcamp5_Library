using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

using TheLibrary.Controllers;

using TheLibrary.Seeding;

using TheLibrary_BusinessLayer.Services;
using TheLibrary_BusinessLayer.Services.Auth;

using TheLibrary_DataAccessLayer.Data;
using TheLibrary_DataAccessLayer.Models;
using TheLibrary_DataAccessLayer.Repositories;

var builder = WebApplication.CreateBuilder(args);

//var service = new BookService();
//var controller = new BookController(service);
//controller.AddService(service);
//controller.bookService = service;


//BookController -> BookService -> BookRepository
//PL -> BL -> DAL
//PL -> DAL
//Program

// -----------------------------------------------------------------------------
// 1. Base de Datos y Microsoft Identity
// -----------------------------------------------------------------------------
builder.Services.AddDbContext<TheLibraryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<TheLibraryDbContext>()
.AddDefaultTokenProviders();

// -----------------------------------------------------------------------------
// 2. Configuración del Middleware de Autenticación JWT Bearer
// -----------------------------------------------------------------------------
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]!;

builder.Services.AddAuthentication(options =>
{
    // Define JWT Bearer como el esquema por defecto para autenticación y desafío
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Solo para entorno local de desarrollo
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero // Elimina la tolerancia por defecto de 5 minutos
    };
});

// -----------------------------------------------------------------------------
// 3. Registro de Servicios de Aplicación
// -----------------------------------------------------------------------------
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Add services to the container.
//AddSingleton, AddScoped, AddTransient
builder.Services.AddScoped<IBookRepository, BookEFRepository>();
builder.Services.AddScoped<ILibraryRepository, LibraryEFRepository>();
builder.Services.AddScoped<ILibraryBranchRepository, LibraryBranchEFRepository>();
builder.Services.AddScoped<IMembershipRepository, MembershipEFRepository>();
builder.Services.AddScoped<ILoanRepository, LoanEFRepository>();

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ILibraryService, LibraryService>();
builder.Services.AddScoped<ILibraryBranchService, LibraryBranchService>();
builder.Services.AddScoped<IMembershipService, MembershipService>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// 3. Configurar Swagger Security
builder.Services.AddSwaggerGen(c => {
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
});

var app = builder.Build();

await app.UseInitialDataSeedAsync();

// -----------------------------------------------------------------------------
// 5. Pipeline HTTP: Orden Estricto de Ejecución
// -----------------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API");
    });
}

app.UseHttpsRedirection();

// ¡REGLA DE ORO DE ASP.NET CORE!:
// UseAuthentication debe ejecutarse obligatoriamente ANTES de UseAuthorization
app.UseAuthentication(); // 1. ¿Quién sos? Lee el header y valida el token
app.UseAuthorization();  // 2. ¿Tenés permiso? Evalúa [Authorize] y roles

app.MapControllers();

app.Run();
