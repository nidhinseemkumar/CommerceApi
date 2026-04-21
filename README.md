# CommerceApi 🛒

A robust, service-oriented E-Commerce Web API built with .NET 10, featuring Entity Framework Core, JWT Authentication, and Response Caching.

## 🚀 Features

- **Authentication**: Secure JWT-based Login and Registration.
- **Product Management**: Full CRUD operations with category association.
- **Cart System**: High-performance shopping cart management.
- **Order Processing**: Atomic order placement with stock validation.
- **Payments**: Integrated payment tracking and order status management.
- **Caching**: Optimized performance using `CachedService` patterns for high-traffic data.
- **Robust Error Handling**: Custom exception middleware for consistent API responses.

## 🛠 Tech Stack

- **Framework**: ASP.NET Core Web API (.NET 10)
- **Database**: SQL Server / Entity Framework Core
- **Security**: JWT Bearer Authentication
- **Patterns**: Service-Oriented Architecture, Repository/Service Pattern, Partial Classes for large services.

## 📁 Project Structure

```text
CommerceApi/
├── Controllers/      # API Endpoints
├── Services/         # Business Logic
│   └── Interfaces/   # Service Contracts
├── Models/           # Database Entities
├── DTOs/             # Data Transfer Objects
├── Data/             # DB Context & Migrations
├── Docs/             # Technical Documentation
└── Scripts/          # Dev & DB Scripts
```

## ⚙️ Getting Started

### Prerequisites
- .NET 10 SDK
- SQL Server (LocalDB or Express)

### Setup
1. Clone the repository.
2. Update the connection string in `appsettings.json` (or use the template from `appsettings.Example.json`).
3. Apply migrations:
   ```bash
   dotnet ef database update
   ```
4. Run the application:
   ```bash
   dotnet run
   ```

## 📖 Documentation

Detailed documentation is available in the [Docs/](Docs/) folder:
- [API Master Specification](Docs/API_Documentation.md)
- [Caching Strategy Guide](Docs/CACHING_GUIDE.md)
- [Architecture & DFD Report](Docs/CommerceAPI_DFD_Report.md)

## 🤝 Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct and the process for submitting pull requests.

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
