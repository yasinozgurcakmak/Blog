# 📝 Blog Platform (.NET 9 & N-Tier Architecture)

[![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C# 13](https://img.shields.io/badge/C%23-13.0-239120?style=for-the-badge&logo=csharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-blue?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/apps/aspnet/mvc)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-9.0-purple?style=for-the-badge&logo=nuget)](https://learn.microsoft.com/en-us/ef/core/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16%2B-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg?style=for-the-badge)](LICENSE)

A production-ready, full-stack blogging and publishing platform engineered with **.NET 9 (C# 13)** and **N-Tier (Layered) Architecture**. Designed with enterprise best practices, clean code principles, robust security standards, and high-performance backend patterns with **PostgreSQL** database support.

---

## 📑 Table of Contents

- [Key Features](#-key-features)
- [Architecture & Design Patterns](#-architecture--design-patterns)
- [Project Structure](#-project-structure)
- [Technology Stack](#-technology-stack)
- [Security & Performance Engineering](#-security--performance-engineering)
- [Getting Started](#-getting-started)
- [Configuration](#-configuration)

---

## 🚀 Key Features

### 📰 Content & Editorial Management
- **Rich-Text Publishing:** WYSIWYG editor integration with sanitized HTML output.
- **Automated Base64 Image Extractor:** Automatically parses inline base64 images from rich-text content, converts and optimizes them onto the filesystem, replacing them with clean URLs to keep database storage lightweight and queries fast.
- **SEO-Friendly URL Slugs:** Automatic slug generation tailored for special character transliteration (including Turkish language accents).
- **Dynamic Reading Time Estimation:** Calculates and displays estimated read time for articles dynamically based on word metrics.
- **Cookie-Based View Counter:** Tracks unique article views using a 24-hour cookie-based deduplication strategy to prevent spam.
- **Categorization:** Categorized browsing, article filtering, and search functionality.

### 💬 Threaded & Nested Comment Engine
- **Hierarchical Discussion Tree:** Recursive tree building algorithm in `CommentManager` that organizes comments and nested child replies to arbitrary depths.
- **Targeted Replies:** Allows users to reply directly to any comment with automatic `@mention` parent attribution.
- **Soft-Delete Tree Preservation:** Deleted comments that have active replies display a placeholder state to preserve discussion integrity without breaking conversation branches.
- **Anchor Navigation:** Redirects users directly to newly posted comments with CSS-targeted smooth focus.

### 👏 Engagement & Social Dynamics
- **Interactive Article "Clap" (Like) System:** Medium-inspired applause counter with AJAX requests for instant UI feedback without full page reloads.
- **Reading List & Bookmarks:** Users can bookmark articles to save them for later and manage their reading list under `/kaydedilenler`.

### 🛡️ Authentication, Authorization & Identity
- **ASP.NET Core Identity Integration:** Custom `AppUser` and `AppRole` implementation with integer primary keys backed by PostgreSQL (`Npgsql.EntityFrameworkCore.PostgreSQL`).
- **Role-Based Access Control (RBAC):** Distinct permission hierarchies (`Admin`, `Editor`, `Member`).
- **Secure Password Reset Workflow:** Cryptographically signed token-based password reset via transactional emails using MailKit and SMTP.
- **Anti-Spam Rate Limiting:** `IMemoryCache`-backed cooldown mechanism (60-second throttling) preventing password reset email flooding.

### ⚙️ Back-Office Administration (`/blgpnl`)
- **Dedicated Admin Area:** Fully isolated administrative dashboard.
- **Article & Category Management:** Complete CRUD operations with draft/published state toggling.
- **User & Role Administration:** Search, inspect, edit roles, and manage users.

---

## 🏛 Architecture & Design Patterns

The solution strictly adheres to **Separation of Concerns (SoC)** by utilizing a decoupled 5-tier architecture:

```
┌─────────────────────────────────────────────────────────────┐
│                       Blog (MVC)                            │  Presentation Layer
│     Controllers, Razor Views, ViewModels, Custom Filters    │  Areas (Admin Panel: blgpnl)
└──────────────────────────────┬──────────────────────────────┘
                               │
┌──────────────────────────────▼──────────────────────────────┐
│                      BusinessLayer                          │  Business Logic Layer
│   Managers, Services, Validation (FluentValidation), DI     │  Business Rules & Tree Builders
└──────────────────────────────┬──────────────────────────────┘
                               │
┌──────────────────────────────▼──────────────────────────────┐
│                     DataAccessLayer                         │  Data Access Layer
│   EF Core DbContext, Repositories (Generic & Specific),     │  Npgsql PostgreSQL Migrations
└──────────────────────────────┬──────────────────────────────┘
                               │
        ┌──────────────────────┴──────────────────────┐
        ▼                                             ▼
┌──────────────────────────────┐       ┌──────────────────────────────┐
│         EntityLayer          │       │           DtoLayer           │
│   Domain Entities & Models   │       │   Data Transfer Objects      │
└──────────────────────────────┘       └──────────────────────────────┘
```

- **Repository Pattern:** Generic repository base extended by domain-specific data access repositories (`EfBlogDal`, `EfCommentDal`, etc.).
- **Dependency Injection (IoC):** Native DI container utilizing clean extension methods (`ContainerDependencies`).
- **Data Transfer Objects (DTO) & AutoMapper:** Completely decouples persistence entities from view/client contracts.
- **FluentValidation:** Business and model validation isolated cleanly from presentation and database logic.
- **Modern C# 13 Constructs:** Primary constructors, pattern matching (`is not int userId`), and nullable reference types enabled across all projects.

---

## 📂 Project Structure

```bash
├── Blog/                   # Presentation Layer (ASP.NET Core MVC & Admin Area)
│   ├── Areas/blgpnl/       # Administrative Back-Office (Controllers, Views)
│   ├── Controllers/        # Public Client Controllers (Blog, Comment, Account, etc.)
│   ├── Helper/             # Slug, Image, HtmlSanitizer & ReadingTime helpers
│   ├── Services/           # Email & Notification Services
│   ├── Views/              # Razor Pages & Layouts
│   └── wwwroot/            # Static assets (CSS, JS, Vendors, Uploads)
│
├── BusinessLayer/          # Core Business Logic
│   ├── Abstract/           # Service Interfaces (IBlogService, ICommentService, etc.)
│   ├── Concrete/           # Service Implementations & Tree Algorithms
│   └── Container/          # DI Registration Extensions
│
├── DataAccessLayer/        # Database & ORM
│   ├── Abstract/           # Repository Interfaces (IBlogDal, ICommentDal, etc.)
│   ├── Concrete/           # EF Core Context & PostgreSQL Configurations
│   ├── EntityFramework/    # Concrete EF Core DAL Implementations
│   ├── Migrations/         # PostgreSQL Database Migrations
│   └── Repository/         # Generic Repository Base
│
├── DtoLayer/               # Data Transfer Objects
│   └── Dtos/               # BlogDtos, CommentDtos, UserDtos, CategoryDtos
│
└── EntityLayer/            # Core Domain Entities
    └── Concrete/           # Blog, Category, Comment, Likes, AppUser, AppRole
```

---

## 🛠 Technology Stack

### Backend
- **Framework:** .NET 9.0 (C# 13)
- **Web Engine:** ASP.NET Core MVC
- **Database & ORM:** PostgreSQL, Entity Framework Core 9.0 (`Npgsql.EntityFrameworkCore.PostgreSQL`)
- **Authentication:** ASP.NET Core Identity & Cookie Authentication
- **Mapping:** AutoMapper 16.x
- **Validation:** FluentValidation 11.x
- **Email Delivery:** MailKit 4.x & MimeKit
- **Media Processing:** SixLabors.ImageSharp 4.x
- **Security & Sanitization:** Ganss.Xss.HtmlSanitizer

### Frontend
- **Templating:** ASP.NET Core Razor Pages / View Components
- **Styling:** Bootstrap 5, Custom CSS
- **Interactions:** Modern Vanilla JavaScript, jQuery, AJAX

---

## 🔒 Security & Performance Engineering

- **Cross-Site Scripting (XSS) Mitigation:** All HTML content rendered from users is parsed and cleaned using `HtmlSanitizer`.
- **Cross-Site Request Forgery (CSRF):** Enforced across state-changing actions via `[ValidateAntiForgeryToken]`.
- **SQL Injection Prevention:** 100% parameterized queries via Entity Framework Core LINQ expressions.
- **Hardened Cookies:**
  - `HttpOnly = true` (Inaccessible to client-side scripts)
  - `SameSiteMode.Strict` (Protects against CSRF attacks)
  - `CookieSecurePolicy.Always` (Enforced HTTPS transmission)
- **Email Flooding Defense:** In-memory caching stores timestamp records per IP/user to restrict repeated password reset attempts within 60 seconds.
- **Image Optimization:** User-submitted avatars and blog banners are processed and resized asynchronously through `SixLabors.ImageSharp` before persisting to disk.

---

## ⚡ Getting Started

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL (v15+)](https://www.postgresql.org/download/) (Local installation, Cloud instance, or Docker container)
- [Visual Studio 2022 (v17.12+)](https://visualstudio.microsoft.com/), [VS Code](https://code.visualstudio.com/), or [JetBrains Rider](https://www.jetbrains.com/rider/)

### Installation

1. **Clone the repository:**
   ```bash
   git clone https://github.com/yasinozgurcakmak/Blog.git
   cd Blog
   ```

2. **Configure Database Connection & SMTP:**
   Open `Blog/appsettings.json` and configure your PostgreSQL connection string and SMTP mail credentials:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=BlogDb;Username=postgres;Password=your_password;"
     },
     "Smtp": {
       "Host": "smtp.gmail.com",
       "Port": 587,
       "UserName": "your-email@example.com",
       "Password": "your-app-password"
     }
   }
   ```

3. **Apply Database Migrations:**
   Run migrations to generate the PostgreSQL schema automatically:
   ```bash
   dotnet ef database update --project DataAccessLayer --startup-project Blog
   ```

4. **Run the Application:**
   ```bash
   dotnet run --project Blog
   ```
   Navigate to `https://localhost:7145` (or the URL output in your terminal).

---

## 📄 License

This project is licensed under the [MIT License](LICENSE). Feel free to use it for educational and portfolio purposes.
