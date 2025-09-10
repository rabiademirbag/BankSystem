# 🏦 BankSystem

BankSystem, çok katmanlı mimari (Multi-Layered Architecture) prensipleriyle geliştirilmiş bir **bankacılık sistemi Web API** projesidir.  
Proje, kullanıcıların kayıt olabilmesini, giriş yapabilmesini, hesaplarını yönetebilmesini, güvenlik işlemleri (parola değiştirme, 2FA) gerçekleştirmesini ve tüm bu işlemlerin loglanmasını sağlar.

---

## 🏗 Mimari Yapı
Proje **3 katmandan** oluşmaktadır:

### 1️⃣ Data Katmanı
- **Entities**: `UserEntity`, `SecurityEntity`, `AccountEntity`  
- **Enums**: `SecurityActionType` (Login, Logout, PasswordChange, TwoFactorEnabled vb.)  
- **Repositories**: Generic `IRepository<T>` ve `Repository<T>` implementasyonu  
- **UnitOfWork**: Transaction yönetimi

### 2️⃣ Business Katmanı
- **Service Layer**: User, Auth, Security, Account işlemlerini kapsar  
- **DTOs**: API ile veri alışverişini kolaylaştırmak için kullanılır  
- **Data Protection**: Şifrelerin güvenli saklanması için `IDataProtection` kullanılmıştır  

### 3️⃣ WebApi Katmanı
- **Controllers**:  
  - `AuthController`  
  - `UserController`  
  - `SecurityController`  
  - `AccountController`  
- **Middleware**:  
  - JWT Authentication  
- **Dependency Injection**: Tüm servisler `Program.cs` üzerinden eklenmiştir  

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
