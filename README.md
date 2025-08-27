ECommerce Multi-Layered Web API — README
Çok katmanlı (.NET 6) bir Online Alışveriş Platformu örneği.
Amaç: Ödev gereksinimlerini birebir karşılayan, Repository + UnitOfWork, EF Core Code-First (PostgreSQL), JWT ile kimlik doğrulama, Data Protection ile parola koruma, Middleware’ler (Log, Maintenance, Global Exception), Action Filter (saat kısıtı), Model Validation ve DI içeren bir API.
🧱 Proje Yapısı
ECommerceMultiLayeredApiProject/
├─ ECommerce.Data/                # Veri erişim katmanı (EF Core)
│  ├─ Context/
│  │  └─ ECommerceDbContext.cs
│  ├─ Entities/
│  │  ├─ BaseEntity.cs
│  │  ├─ UserEntity.cs
│  │  ├─ ProductEntity.cs
│  │  ├─ OrderEntity.cs
│  │  ├─ OrderProductEntity.cs
│  │  └─ SettingEntity.cs
│  ├─ Enums/
│  │  └─ Role.cs
│  ├─ Repositories/
│  │  ├─ IRepository.cs
│  │  └─ Repository.cs
│  └─ UnitOfWork/
│     ├─ IUnitOfWork.cs
│     └─ UnitOfWork.cs
│
├─ ECommerce.Business/            # İş kuralları katmanı
│  ├─ DataProtection/
│  │  ├─ IDataProtection.cs
│  │  └─ DataProtection.cs
│  ├─ Types/
│  │  └─ ServiceMessage.cs
│  └─ Operations/
│     ├─ User/ (IUserService, UserManager, Dtos)
│     ├─ Product/ (IProductService, ProductManager, Dtos)
│     ├─ Order/ (IOrderService, OrderManager, Dtos)
│     └─ Setting/ (ISettingService, SettingManager)
│
└─ ECommerce.WebApi/              # Sunum katmanı (API)
   ├─ Controllers/ (Auth, Products, Orders, Settings)
   ├─ Filters/ (TimeControlFilter.cs)
   ├─ Middlewares/
   │  ├─ GlobalExceptionMiddleware(+Extensions).cs
   │  ├─ MaintenanceMiddleware(+Extensions).cs
   │  └─ RequestLoggingMiddleware(+Extensions).cs
   ├─ Jwt/ (JwtClaimNames, JwtDto, JwtHelper)
   ├─ Models/ (RegisterRequest, LoginRequest, LoginResponse)
   ├─ appsettings.json
   └─ Program.cs
Bağımlılıklar:
ECommerce.WebApi → ECommerce.Business & ECommerce.Data
ECommerce.Business → ECommerce.Data
🔧 Önkoşullar
.NET 6 SDK
PostgreSQL (ör: localhost:5432, kullanıcı: postgres, şifre: 1234)
(İsteğe bağlı) Visual Studio 2022 for Mac veya Rider/VSCode
EF CLI:
dotnet tool install --global dotnet-ef
📦 NuGet Paketleri (özet)
Data: Microsoft.EntityFrameworkCore, Npgsql.EntityFrameworkCore.PostgreSQL
Business: Microsoft.AspNetCore.DataProtection
WebApi: Microsoft.EntityFrameworkCore.Design, Npgsql.EntityFrameworkCore.PostgreSQL, Microsoft.AspNetCore.Authentication.JwtBearer, Microsoft.AspNetCore.DataProtection, Swashbuckle.AspNetCore
VS’de: Project → Manage NuGet Packages.
CLI’de: dotnet add <csproj> package <name> --version 6.*
⚙️ Konfigürasyon
ECommerce.WebApi/appsettings.json:
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ECommerceDb;Username=postgres;Password=1234"
  },
  "Jwt": {
    "SecretKey": "SÜPER-UZUN-BİR-GİZLİ-ANAHTAR-EN-AZ-32-CHAR",
    "Issuer": "ECommerceIssuer",
    "Audience": "ECommerceAudience",
    "ExpireMinutes": "60"
  },
  "Logging": { "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" } },
  "AllowedHosts": "*"
}
Data Protection anahtarları Program.cs’de App_Data/Keys altına yazılır (dosya oluşturma izni gerekli).
🗄️ Veritabanı (Code-First)
Çözüm kökünden veya ECommerce.WebApi klasöründen:
# İlk migration (Data projesi için, WebApi startup)
dotnet ef migrations add InitialCreate -s ECommerce.WebApi -p ECommerce.Data

# DB oluştur / güncelle
dotnet ef database update -s ECommerce.WebApi -p ECommerce.Data
Seed: SettingEntity (Id=1, MaintenanceMode=false) başlangıçta eklenir.
▶️ Çalıştırma
dotnet run --project ECommerce.WebApi
Swagger UI: https://localhost:xxxxx/swagger
🔐 Kimlik Doğrulama (JWT) & Parola Koruma
Login sonrası Bearer <token> üretir (HS256).
ClaimTypes.Role ekli olduğu için [Authorize(Roles="Admin")] çalışır.
Parola: ASP.NET Core Data Protection ile Protect/Unprotect (ödev şartı).
Not: Üretimde geri döndürülemez hash (PBKDF2/BCrypt) tercih edilir; bu proje ödev gereksinimini karşılar.
🧩 Middleware & Filter’lar
GlobalExceptionMiddleware: Beklenmeyen hataları yakalar → tek tip JSON (500).
MaintenanceMiddleware: DB’de Settings tablosu MaintenanceMode=true ise, login/register/state hariç tüm endpoint’ler 503 döner.
RequestLoggingMiddleware: Metot/Path/UserId (varsa) loglar.
TimeControlFilter: Belirli saatler dışında (varsayılan 15:00-23:59) çağrıları 403 ile kapatır (örn. ürün PUT).
🧠 Mimari ve Tasarım Kararları
Katmanlı Mimari: API (sunum) ↔ Business (iş) ↔ Data (veri).
Repository + UnitOfWork: Veri erişimi soyutlanır; çoklu repository işlemleri tek SaveChanges ile persist edilir.
DTO Kullanımı: Dış dünyaya entity sızdırılmaz; input/output modelleri ayrıdır.
UTC Tarihler: CreatedDate/OrderDate UtcNow; PostgreSQL timestamptz ile uyumlu.
Validation: Request modellerinde [Required], [EmailAddress], [MinLength] vb.
🔌 API Uçları (özet)
Auth
POST /api/auth/register – Kayıt (anonim)
POST /api/auth/login – Giriş (token döner)
GET /api/auth/me – Token’daki claim’ler
Products
GET /api/products – Ürün listesi (anonim)
GET /api/products/paged?page=1&pageSize=10 – Sayfalı liste
GET /api/products/{id} – Ürün detayı
POST /api/products – Admin (ürün ekleme)
PUT /api/products/{id} – Admin + TimeControl (tam güncelle)
PATCH /api/products/{id}/price?price=123 – Admin
PATCH /api/products/{id}/stock?stock=10 – Admin
DELETE /api/products/{id} – Admin
Orders (Authorize)
POST /api/orders – Sipariş oluştur ({ "items":[{ "productId":1,"quantity":2 }] })
GET /api/orders/my – Kullanıcının siparişleri
GET /api/orders/{id} – Kendi siparişi; Admin herkesinkini görür
DELETE /api/orders/{id} – Sahibi veya Admin
Settings
PATCH /api/settings/maintenance-toggle – Admin (bakım modunu aç/kapat)
GET /api/settings/maintenance-state – Anonim (durum)
🧪 Hızlı Deneme Adımları (Swagger ile)
Register → POST /api/auth/register
Login → POST /api/auth/login → token’ı kopyala
Authorize (Swagger butonu) → Bearer <token> gir
Admin işlemleri için kullanıcı rolünü Admin yap:
-- PostgreSQL örneği:
UPDATE "Users" SET "Role" = 1 WHERE "Email" = 'seninmailin@example.com';
Ürün ekle / listele / güncelle / sil
Sipariş oluştur → POST /api/orders
Bakım modunu aç → PATCH /api/settings/maintenance-toggle
Sonra ürün GET/POST deneyip 503 görebilirsin (login/register/state hariç)
🩹 Sorun Giderme
“Unable to create an object of type '…DbContext'” (migration)
-s (startup) ve -p (migrations project) parametrelerini doğru ver:
dotnet ef migrations add InitialCreate -s ECommerce.WebApi -p ECommerce.Data
dotnet ef database update -s ECommerce.WebApi -p ECommerce.Data
Data Protection tipi bulunamadı (Business derlenmiyor)
ECommerce.Business projesine Microsoft.AspNetCore.DataProtection paketini ekle.
401/403
401 → token yok/yanlış; 403 → rol yetmezliği veya TimeControlFilter saat dışında.
503
Bakım modu açık. GET /api/settings/maintenance-state ile teyit et; gerekirse toggle.
DateTime/UTC
Tarihler UtcNow kullanır; PostgreSQL timestamptz ile uyumludur.
🛡️ Güvenlik Notları
Parola Data Protection ile şifrelenir ve geri açılabilir (ödev gereği).
Üretimde hash (tek yön) tercih edin.
Jwt:SecretKey en az 32 karakter ve gizli tutulmalı.
📌 Özelleştirme İpuçları
Saat kısıtı: TimeControlFilter(StartTime="09:00", EndTime="18:00")
Bakım modu beyaz liste: MaintenanceMiddleware içindeki Allowed setine ekle.
Sayfalama varsayılanı: GetPagedProductsAsync’ta pageSize ayarla.
