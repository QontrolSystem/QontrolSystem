# QontrolSystem - IT Ticketing Web Application

## Overview

QontrolSystem is a ticketing platform built with **ASP.NET Core MVC** to streamline IT support operations at **Omnitak**. It replaces informal channels with a structured web-based system to improve request tracking, reduce downtime, and support company scalability.

---

## 📦 Features

- ✅ User authentication (Login, Register)
- ✅ Role-based user management (CRUD)
- ✅ Admin and user dashboards
- ✅ Structured ticketing system (in development)
- ✅ Responsive Razor view-based UI
- ✅ Soft delete for accountability

---

## 🛠 Technologies Used

| Tech | Purpose |
|------|---------|
| ASP.NET Core MVC 9.0 | Main web framework |
| Entity Framework Core 9.0.6 | ORM for SQL Server |
| ASP.NET Identity | User authentication and role management |
| HTML5, CSS3 | Front-end markup and styling |
| Visual Studio 2022 | IDE for development |
| GitHub | Version control & collaboration |
| SQL Server | Database backend |

---

## 🧱 Architecture

The project follows the **Model-View-Controller (MVC)** architecture:

- **Models**: Represent domain objects (e.g., User, Ticket)
- **Views**: Built with Razor `.cshtml` for dynamic HTML rendering
- **Controllers**: Handle requests and manage view-model interactions
- **Data**: `AppDbContext` manages data operations with EF Core

---

## 🗃 Database Design

The database is modeled around key entities:
- `Users`, `Tickets`, `Roles`, `Department`, `Comments`, `Feedback`, and `Audit Logs`

---

## 🚀 Getting Started

### Prerequisites
- .NET SDK 9.0
- Visual Studio 2022 (with ASP.NET workload)
- SQL Server + SQL Server Management Studio
- Git

### Setup Instructions

```bash
gitclone https://github.com/QontrolSystem/QontrolSystem.git
```
Alternatively, download the ZIP and extract manually.
---
### Database Configuration

1. Open SSMS and run the SQL scripts to create the required database and tables.

2. Update the appsettings.json connection string as needed.

3. Apply migrations:
```bash
powershell

Update-Database
```
---
### Run the App
1. Open the .sln file in Visual Studio.

2. Build the solution to restore packages.

3. Press F5 or click Start to launch locally.
---
## 👥 Contributors
- [Mnqobi Ntekere](https://github.com/MnqobiConquer)
- [Nhlakanipho Radebe](https://github.com/AlsonAfrica)
- [Ntshebetseng Moloi](https://github.com/NtshebetsengM)
- [Zesuliwe Sinyamba](https://github.com/Zesuliwe17)
