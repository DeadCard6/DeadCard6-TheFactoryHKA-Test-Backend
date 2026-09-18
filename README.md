# EvaluacionTecnicaTheFactoryHKA - Backend API

This repository contains the backend implementation for the technical evaluation of **The Factory HKA**. 
The application provides a RESTful API to manage sales (Invoices), Clients, Products, and Categories, including JWT-based authentication.

## 🏗️ Architecture & Design Patterns

The solution is built using **Onion Architecture** and principles of **Domain-Driven Design (DDD)**:

- **Rich Domain Model:** Business logic (such as stock deduction, invoice total calculations, and status validation) is encapsulated directly inside the domain entities (`Invoice`, `Product`). 
- **DTOs (Data Transfer Objects):** Used to isolate the domain layer from the API layer. The application layer maps entities to strong-typed `Response` and `Request` DTOs.
- **Repository Pattern & Unit of Work:** Abstracts data access, providing a clean contract for the Application services.
- **SQL Sequences:** Avoids race conditions during concurrent invoice creation by generating atomic invoice numbers at the database level.

### Project Structure
- `EvaluacionTecnicaTheFactoryHKA.Domain`: Core entities, interfaces, and domain exceptions.
- `EvaluacionTecnicaTheFactoryHKA.Application`: Application services, DTOs, and business orchestration.
- `EvaluacionTecnicaTheFactoryHKA.Infrastructure`: Entity Framework Core implementation, DbContext, Migrations, and Repositories.
- `EvaluacionTecnicaTheFactoryHKA.Authentication`: JWT Token generation and validation.
- `EvaluacionTecnicaTheFactoryHKA`: The ASP.NET Core Web API presentation layer.
- `EvaluacionTecnicaTheFactoryHKA.Tests`: xUnit project containing Domain and Application unit tests.

## 🚀 Technologies Used

- **.NET 8**
- **Entity Framework Core**
- **SQL Server**
- **JWT Authentication**
- **xUnit, Moq, FluentAssertions** (for Unit Testing)
- **Swagger / OpenAPI** (for API documentation)

## 🛠️ Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- SQL Server (LocalDB or a full instance)

### Setup Instructions

1. **Clone the repository:**
   ```bash
   git clone https://github.com/DeadCard6/DeadCard6-TheFactoryHKA-Test-Backend.git
   cd EvaluacionTecnicaTheFactoryHKA
   ```

2. **Configure the Database Connection:**
   Update the `DefaultConnection` string in `appsettings.json` located in the main API project if necessary. By default, it's set to use SQL Server.

3. **Apply Database Migrations:**
   Ensure your database is created, seeded, and updated to the latest schema:
   ```bash
   dotnet ef database update --project EvaluacionTecnicaTheFactoryHKA.Infrastructure/EvaluacionTecnicaTheFactoryHKA.Infrastructure.csproj --startup-project EvaluacionTecnicaTheFactoryHKA/EvaluacionTecnicaTheFactoryHKA.csproj
   ```
   *Note: Seed data is automatically applied during migration, populating initial Categories, Products, and a default Client.*

4. **Run the Application:**
   ```bash
   dotnet run --project EvaluacionTecnicaTheFactoryHKA/EvaluacionTecnicaTheFactoryHKA.csproj
   ```
   The API will start. You can explore and test the endpoints via Swagger UI at `https://localhost:<port>/swagger`.

## 🧪 Running Tests

The solution includes a test project validating the Domain logic and Application services.

To run all unit tests:
```bash
dotnet test
```

## 🔐 Authentication (JWT)

Most endpoints are secured via JWT (`[Authorize]`). 
To test the API, you must first create a user or login via the `/api/Auth/login` endpoint to obtain a Token, and then pass it in the `Authorization` header as a Bearer token.
