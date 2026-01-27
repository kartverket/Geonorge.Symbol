# Geonorge.Symbol

A web-based cartographic symbol management and registration system for Kartverket (the Norwegian Mapping Authority). Part of the [Geonorge](https://www.geonorge.no/) national geospatial data portal.

## Overview

Geonorge.Symbol enables organizations to upload, manage, and distribute cartographic symbols used in mapping applications. It provides both a web interface and REST API for symbol discovery and distribution.

### Key Features

- **Symbol Management** - Create, edit, and delete symbols with metadata and thumbnails
- **Package Organization** - Group symbols into themed collections with access control
- **Multi-Format Support** - Store symbols in various formats (PNG, SVG, GIF, EPS, TIFF, AI, PDF)
- **Image Processing** - Automatic format conversion, resizing, and raster-to-vector conversion
- **REST API** - Programmatic access for symbol discovery and retrieval
- **Authentication** - OpenID Connect integration with Norwegian government authentication
- **Authorization** - Role-based access control with organization-specific permissions

## Technology Stack

| Component | Technology |
|-----------|------------|
| Backend | ASP.NET MVC 5, .NET Framework 4.7.2 |
| API | ASP.NET Web API |
| Database | SQL Server with Entity Framework 6 |
| Image Processing | Magick.NET, ImageTracerNet |
| Authentication | OWIN, OpenID Connect, Geonorge.AuthLib |
| DI Container | Autofac |
| Frontend | Razor Views, jQuery, Bootstrap |
| Logging | log4net |

## Project Structure

```
Geonorge.Symbol/
├── Geonorge.Symbol/           # Main ASP.NET MVC project
│   ├── Controllers/           # MVC and API controllers
│   ├── Models/                # Entity Framework models
│   ├── Services/              # Business logic layer
│   ├── Views/                 # Razor templates
│   ├── Migrations/            # EF Code-First migrations
│   └── App_Start/             # Configuration classes
├── Geonorge.Symbol.Tests/     # Unit tests
└── ImageTracerNet/            # Custom raster-to-vector library
```

## Getting Started

### Prerequisites

- Visual Studio 2019 or later
- .NET Framework 4.7.2
- SQL Server (LocalDB or Express)
- Node.js and npm (for frontend dependencies)

### Configuration

1. Copy `Geonorge.Symbol/settings.config.example` to `settings.config` and configure:
   - Database connection string
   - Authentication settings
   - External service URLs

2. Update `Web.config` connection string if needed:
   ```xml
   <connectionStrings>
     <add name="SymbolDbContext"
          connectionString="Server=.\SQLEXPRESS;Database=kartverket_symbol;Integrated Security=True;"
          providerName="System.Data.SqlClient" />
   </connectionStrings>
   ```

### Running the Application

1. Open `Geonorge.Symbol.sln` in Visual Studio
2. Restore NuGet packages
3. Run Entity Framework migrations: `Update-Database`
4. Press F5 to run with IIS Express

## API Endpoints

| Endpoint | Description |
|----------|-------------|
| `GET /api/symbols` | List all symbols with optional filtering |
| `GET /api/symbol/{uuid}` | Get symbol details by UUID |

## Data Models

- **Symbol** - Represents a symbol with name, description, owner organization, and theme
- **SymbolPackage** - Groups symbols into collections with official status and folder structure
- **SymbolFile** - Individual symbol graphics with format, color, size, and type properties