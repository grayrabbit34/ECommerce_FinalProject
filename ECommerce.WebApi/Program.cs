using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ECommerce.Business.DataProtection;
using ECommerce.Business.Operations.Order;
using ECommerce.Business.Operations.Order.Dtos;
using ECommerce.Business.Operations.Product;
using ECommerce.Business.Operations.Setting;
using ECommerce.Business.Operations.User;
using ECommerce.Data.Context;
using ECommerce.Data.Repositories;
using ECommerce.Data.UnitOfWork;
using ECommerce.WebApi.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- Controller ve Swagger servisleri ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Swagger üzerinden JWT girip deneme yapabilmek için şema tanımı
    var jwtScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Insert ONLY your JWT Bearer Token on textbox below!",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    options.AddSecurityDefinition(jwtScheme.Reference.Id, jwtScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtScheme, Array.Empty<string>() }
    });
});

// --- Data Protection ---
// Parola şifreleme için anahtar dosyalarını projedeki bir klasöre yazar.
// Uygulama yeniden başlasa da aynı anahtarlarla çözebilmek için şart.
var keysDir = new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "App_Data", "Keys"));
builder.Services.AddDataProtection()
    .SetApplicationName("ECommercePlatform")
    .PersistKeysToFileSystem(keysDir);

//database bağlantısı için 3. ve son işlemler
// --- PostgreSQL DbContext ---
// Npgsql provider ile bağlanıyoruz (EF Core)
var cs = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ECommerceDbContext>(opt => opt.UseNpgsql(cs));

// --- JWT Authentication ---
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // .NET'in default claim eşlemelerini kapat
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, //eşleşemeleri kontrol ediyorum
            ValidateAudience = true,
            ValidateLifetime = true,    // süresi geçmiş token'ı reddet
            ValidateIssuerSigningKey = true,       // imza doğrulama
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!)
            ),
            NameClaimType = "Id",                  // User.Identity.Name yerine "Id" claim'i
            RoleClaimType = ClaimTypes.Role        // [Authorize(Roles="Admin")] için
        };
    });

// --- DI Kayıtları ---
// Generic Repository, UnitOfWork ve Business servisleri

//generic olduğu için "typeof" kullanımı yapıyoruz
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDataProtection,DataProtection>();
builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<IProductService, ProductManager>();
builder.Services.AddScoped<IOrderService, OrderManager>();
builder.Services.AddScoped<ISettingService, SettingManager>();

var app = builder.Build();

// --- HTTP Pipeline ---
// Swagger yalnızca Development'ta açılır (istersen prod'da da açabilirsin)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Sıra önemli: önce global hata, sonra maintenance ve logging
app.UseGlobalException();   // En başta dursun ki aşağıdaki middleware'lerden gelen hataları da yakalasın
app.UseMaintenanceMode();   // Bakım modunda istekleri kes
app.UseRequestLogging();    // Her isteği logla (en sonda da olabilir; tercih meselesi)

app.UseHttpsRedirection();

app.UseAuthentication();    // Kimlik doğrulama jwt için kullanıyoruz
app.UseAuthorization();     // Yetkilendirme

app.MapControllers();       // Controller endpointlerini map et

app.Run();


