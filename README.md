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

## 🔍 API Overview

QontrolSystem exposes a RESTful JSON-based API to enable programmatic access to key platform features like user authentication, ticket management, and administrative user control.

### Authentication

●	The API uses JWT (JSON Web Tokens) for securing protected endpoints.


●	Clients must obtain a JWT by logging in (POST /api/account/login) and include it in the Authorization header as:
 Authorization: Bearer <token>


●	Role-based access control restricts endpoints to users with appropriate roles such as Employee or System Administrator.


### Data Format
●	The API communicates primarily via JSON for request and response bodies.

●	Endpoints that accept file uploads (e.g., ticket attachments) use multipart/form-data.


### Response Codes
●	Standard HTTP status codes indicate success (200 OK, 201 Created) or errors (400 Bad Request, 401 Unauthorized, 403 Forbidden, 404 Not Found).


### Usage
●	This API is intended primarily for internal use by the web application frontend and trusted clients.

●	Developers can test the API with tools like Postman, curl, or Swagger UI if available.


#### For detailed API documentation, view the full guide on OneDrive: 
https://dyndna-my.sharepoint.com/:w:/g/personal/zesuliwe_sinyamba_dynamicdna_co_za/ES9IHvEL0c5Aqb0bf6k-tEcB2IERAhsPc1L0KrmzUVd1SQ?e=fYHK0b

---

## 🧑‍💻 Coding Standards

To ensure consistency, readability, and maintainability across the codebase, all contributors must adhere to the project's coding standards.
#### View the Coding Standards Document:
https://1drv.ms/w/c/83408a743f4c04e4/EXwI69xqjFlDvEjcG5Baw64B2LehjLIfWxZ66DE1lf6eoQ?e=VkaqRt


## 👥 Contributors
- [Mnqobi Ntekere](https://github.com/MnqobiConquer)
- [Nhlakanipho Radebe](https://github.com/AlsonAfrica)
- [Ntshebetseng Moloi](https://github.com/NtshebetsengM)
- [Zesuliwe Sinyamba](https://github.com/Zesuliwe17)
