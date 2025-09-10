# 🏦 BankSystem

BankSystem, çok katmanlı mimari (Multi-Layered Architecture) prensipleriyle geliştirilmiş bir **bankacılık sistemi Web API** projesidir.  
Proje, kullanıcıların kayıt olabilmesini, giriş yapabilmesini, hesaplarını yönetebilmesini, güvenlik işlemleri (parola değiştirme, 2FA) gerçekleştirmesini ve tüm bu işlemlerin loglanmasını sağlar.

---

## 📂 Proje Yapısı

Proje 3 ana katmandan oluşur:

BankSystem
│
├── BankSystem.Data # Entity Framework Core, Entities, Repositories, Configurations
├── BankSystem.Business # İş mantığı, Service katmanı, Validations, DTOs
├── BankSystem.WebApi # ASP.NET Core Web API, Controllers, JWT Authentication

### 🔹 Katmanlar

- **Data Layer (BankSystem.Data)**  
  - `Entities`: Veritabanı tablolarını temsil eden entity sınıfları  
  - `Enums`: Kullanıcı tipleri, SecurityActionType gibi enum tanımları  
  - `Repositories`: Generic repository yapısı  
  - `Configurations`: Entity Framework Core Fluent API konfigürasyonları  
  - `UnitOfWork`: Transaction yönetimi  

- **Business Layer (BankSystem.Business)**  
  - `Operations`: İş mantığı servisleri (UserManager, SecurityManager, AccountManager, vb.)  
  - `Dtos`: Veri transfer objeleri  
  - `Types`: ServiceMessage gibi ortak dönüş tipleri  
  - `DataProtection`: Şifreleme/deşifreleme mekanizması  
  - `Sms`: 2FA için SMS gönderim servisi  

- **Web API Layer (BankSystem.WebApi)**  
  - `Controllers`: Kullanıcı, Güvenlik, Auth ve diğer işlemleri yöneten controllerlar  
  - `Program.cs`: Middleware, Service registration, JWT Authentication  

---

## 🛠 Kullanılan Teknolojiler

- **.NET 8.0 / ASP.NET Core Web API**
- **Entity Framework Core (Code First)**
- **SQL Server** (veritabanı)
- **JWT Authentication** (Kimlik doğrulama)
- **Dependency Injection**
- **Unit of Work & Repository Pattern**
- **DTO (Data Transfer Objects)**
- **2FA (Two Factor Authentication) via SMS**
- **Logging & Security Actions**

---

## 📡 API Endpointleri

### 🔹 Auth İşlemleri
- `POST /api/Auth/register` → Yeni kullanıcı kaydı  
- `POST /api/Auth/login` → Kullanıcı girişi (**2FA kontrolü dahil**)  
- `POST /api/Auth/verify-2fa` → 2FA kodunu doğrulama  
- `GET /api/Auth/{id}` → Kullanıcı bilgilerini getir (**Admin**)  
- `PUT /api/Auth/{id}` → Kullanıcı bilgilerini güncelle (**Admin**)  
- `DELETE /api/Auth/{id}` → Kullanıcı sil (**Admin**)  

---

### 🔹 User İşlemleri
(Servis mantığı `IUserService` içinde, AuthController üzerinden erişiliyor)

- Kullanıcı yönetimi **AuthController** içinde yapılmaktadır.  

---

### 🔹 Security İşlemleri
- `POST /api/Security/two-factor-auth` → Kullanıcı için 2FA ayarını aç/kapat  
- `POST /api/Security/change-password` → Şifre değiştirme  
- `GET /api/Security/logs` → Kullanıcıya ait güvenlik loglarını getir  

---

### 🔹 Account İşlemleri
- `GET /api/Accounts` → Tüm hesapları getir (**Admin**)  
- `POST /api/Accounts` → Yeni hesap oluştur (**Admin**)  
- `GET /api/Accounts/{id}/balance` → Hesap bakiyesini getir  
- `GET /api/Accounts/{id}/transactions` → Hesaba ait işlemleri getir  
- `POST /api/Accounts/{id}/transfer` → Para transferi yap  
- `POST /api/Accounts/{id}/set-default` → Varsayılan hesap ayarla  

---

### 🔹 Bill İşlemleri
- `POST /api/Bills/{id}` → Fatura öde  
- `POST /api/Bills` → Yeni fatura ekle (**Admin**)  
