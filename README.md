# Notes Management API

A secure RESTful API built with **.NET 9** and **SQL Server**, designed for managing user notes with authentication and CRUD operations.

## 🚀 Features
- User registration and login with **JWT authentication**
- Create, read, update, and delete notes
- One-to-many relationship: User ↔ Notes
- **Swagger UI** with JWT support for interactive testing
- **SOLID principles** and dependency injection for clean architecture
- **GitHub Actions CI pipeline** for automated build and test
- Ready for **Azure App Service deployment**

## 🛠️ Tech Stack
- **Backend**: ASP.NET Core Web API (.NET 9)
- **Database**: SQL Server, Entity Framework Core
- **Authentication**: JWT
- **Tools**: Swagger, AutoMapper, GitHub Actions, Azure
## 📂 Project Structure
NotesApp.Domain        → Entities (User, Note)
NotesApp.Application   → Services, DTOs, Interfaces
NotesApp.Infrastructure → EF Core, Repositories
NotesApp.Api           → Controllers, Startup, Swagger

# Developer Guide

## 🛠️ Prerequisites
- .NET 9 SDK
- SQL Server (local or Azure)
- Git

## ⚙️ Setup Instructions
1. Clone the repo:
   git clone https://github.com/yourusername/NotesApp.git
   cd NotesApp
2. Update appsettings.json with your SQL Server connection string.
3. Apply migrations:
   dotnet ef database update
4. Run the API:
   cd NotesApp.Api
   dotnet run
5. Open Swagger:
   eg: https://localhost:5001/swagger

🚀 Usage Flow
- Register a user → Login → Copy JWT → Authorize in Swagger → Create notes → Fetch notes

## 🔮 Future Improvements
- Role-based authorization
- Azure SQL integration
- More unit test coverage

