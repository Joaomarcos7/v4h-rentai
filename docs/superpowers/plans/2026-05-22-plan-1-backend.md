# Teleconsultoria — Plan 1: Backend (.NET 8)

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build the complete .NET 8 backend — Clean Architecture, CQRS, auth, AI document validation mock, SignalR real-time notifications, PDF export, and integration tests.

**Architecture:** Four projects (V4H.Domain → V4H.Application → V4H.Infrastructure → V4H.API) following Clean Architecture dependency rules. CQRS via MediatR; repository pattern over EF Core 8 + PostgreSQL 16; JWT HS256 auth; SignalR hub for real-time; QuestPDF for export.

**Tech Stack:** .NET 8, ASP.NET Core, EF Core 8, PostgreSQL 16, MediatR, SignalR, QuestPDF, BCrypt.Net-Next, xUnit, NSubstitute, WebApplicationFactory, Docker Compose

---

## File Map

```
backend/
├── src/
│   ├── V4H.Domain/
│   │   ├── V4H.Domain.csproj
│   │   ├── Entities/User.cs
│   │   ├── Entities/Teleconsultoria.cs
│   │   ├── Entities/TeleconsultoriaDocument.cs
│   │   ├── Entities/StatusHistory.cs
│   │   ├── Entities/Opinion.cs
│   │   ├── Enums/UserRole.cs
│   │   ├── Enums/Specialty.cs
│   │   ├── Enums/TeleconsultoriaStatus.cs
│   │   └── Interfaces/Repositories/
│   │       ├── IUserRepository.cs
│   │       ├── ITeleconsultoriaRepository.cs
│   │       └── IOpinionRepository.cs
│   ├── V4H.Application/
│   │   ├── V4H.Application.csproj
│   │   ├── Common/Exceptions/
│   │   │   ├── NotFoundException.cs
│   │   │   ├── UnauthorizedException.cs
│   │   │   └── DocumentValidationException.cs
│   │   ├── Common/Interfaces/
│   │   │   ├── IPasswordHasher.cs
│   │   │   ├── IJwtService.cs
│   │   │   ├── IDocumentValidationService.cs
│   │   │   ├── IFileStorageService.cs
│   │   │   ├── INotificationService.cs
│   │   │   └── IPdfExportService.cs
│   │   ├── Auth/
│   │   │   ├── Commands/RegisterCommand.cs
│   │   │   ├── Commands/RegisterCommandHandler.cs
│   │   │   ├── Commands/LoginCommand.cs
│   │   │   ├── Commands/LoginCommandHandler.cs
│   │   │   └── DTOs/AuthResultDto.cs
│   │   └── Teleconsultorias/
│   │       ├── Commands/CreateTeleconsultoriaCommand.cs
│   │       ├── Commands/CreateTeleconsultoriaCommandHandler.cs
│   │       ├── Commands/UpdateStatusCommand.cs
│   │       ├── Commands/UpdateStatusCommandHandler.cs
│   │       ├── Commands/UploadDocumentCommand.cs
│   │       ├── Commands/UploadDocumentCommandHandler.cs
│   │       ├── Commands/RegisterOpinionCommand.cs
│   │       ├── Commands/RegisterOpinionCommandHandler.cs
│   │       ├── Queries/ListTeleconsultoriasQuery.cs
│   │       ├── Queries/ListTeleconsultoriasQueryHandler.cs
│   │       ├── Queries/GetTeleconsultoriaDetailQuery.cs
│   │       ├── Queries/GetTeleconsultoriaDetailQueryHandler.cs
│   │       ├── Queries/ExportPdfQuery.cs
│   │       ├── Queries/ExportPdfQueryHandler.cs
│   │       └── DTOs/
│   │           ├── TeleconsultoriaListItemDto.cs
│   │           ├── TeleconsultoriaDetailDto.cs
│   │           ├── DocumentDto.cs
│   │           └── OpinionDto.cs
│   ├── V4H.Infrastructure/
│   │   ├── V4H.Infrastructure.csproj
│   │   ├── Persistence/
│   │   │   ├── AppDbContext.cs
│   │   │   ├── Configurations/UserConfiguration.cs
│   │   │   ├── Configurations/TeleconsultoriaConfiguration.cs
│   │   │   ├── Configurations/DocumentConfiguration.cs
│   │   │   ├── Configurations/StatusHistoryConfiguration.cs
│   │   │   ├── Configurations/OpinionConfiguration.cs
│   │   │   └── Repositories/
│   │   │       ├── UserRepository.cs
│   │   │       ├── TeleconsultoriaRepository.cs
│   │   │       └── OpinionRepository.cs
│   │   ├── Services/
│   │   │   ├── BcryptPasswordHasher.cs
│   │   │   ├── JwtService.cs
│   │   │   ├── MockDocumentValidationService.cs
│   │   │   ├── LocalFileStorageService.cs
│   │   │   ├── QuestPdfExportService.cs
│   │   │   └── SignalRNotificationService.cs
│   │   ├── Hubs/NotificationHub.cs
│   │   └── DependencyInjection.cs
│   └── V4H.API/
│       ├── V4H.API.csproj
│       ├── Program.cs
│       ├── Middleware/ExceptionHandlingMiddleware.cs
│       └── Controllers/
│           ├── AuthController.cs
│           └── TeleconsultoriasController.cs
└── tests/
    ├── V4H.Application.Tests/
    │   ├── V4H.Application.Tests.csproj
    │   ├── Auth/RegisterCommandHandlerTests.cs
    │   ├── Auth/LoginCommandHandlerTests.cs
    │   ├── Teleconsultorias/CreateTeleconsultoriaCommandHandlerTests.cs
    │   ├── Teleconsultorias/UploadDocumentCommandHandlerTests.cs
    │   └── Teleconsultorias/RegisterOpinionCommandHandlerTests.cs
    └── V4H.API.Tests/
        ├── V4H.API.Tests.csproj
        └── Integration/TeleconsultoriasEndpointTests.cs
```

---

### Task 1: Project Scaffold + NuGet Packages

**Files:**
- Modify: `backend/src/V4H.Domain/V4H.Domain.csproj`
- Modify: `backend/src/V4H.Application/V4H.Application.csproj`
- Modify: `backend/src/V4H.Infrastructure/V4H.Infrastructure.csproj`
- Modify: `backend/src/V4H.API/V4H.API.csproj`
- Create: `backend/tests/V4H.Application.Tests/V4H.Application.Tests.csproj`
- Create: `backend/tests/V4H.API.Tests/V4H.API.Tests.csproj`
- Create: `docker-compose.yml` (repo root)
- Create: `.env.example` (repo root)

- [ ] **Step 1: Initialize git repo**

```bash
cd C:\Users\joaom\Dev\v4h-rentai
git init
git add .gitignore
git commit -m "chore: initial commit with gitignore"
```

- [ ] **Step 2: Add docker-compose.yml**

Create `docker-compose.yml` at repo root:

```yaml
version: "3.9"
services:
  db:
    image: postgres:16-alpine
    environment:
      POSTGRES_USER: ${POSTGRES_USER}
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
      POSTGRES_DB: ${POSTGRES_DB}
    ports:
      - "5432:5432"
    volumes:
      - pgdata:/var/lib/postgresql/data

  api:
    build:
      context: ./backend
      dockerfile: src/V4H.API/Dockerfile
    environment:
      ConnectionStrings__Default: "Host=db;Database=${POSTGRES_DB};Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD}"
      JWT__Secret: ${JWT_SECRET}
      AI__ValidationThreshold: "0.6"
    ports:
      - "5000:8080"
    depends_on:
      - db

volumes:
  pgdata:
```

- [ ] **Step 3: Add .env.example and .gitignore entries**

Create `.env.example`:
```
POSTGRES_USER=v4h
POSTGRES_PASSWORD=v4h_secret
POSTGRES_DB=v4h_rentai
JWT_SECRET=change-me-super-secret-at-least-32-chars
```

Ensure `.gitignore` contains:
```
.env
backend/uploads/
```

- [ ] **Step 4: Create backend solution and projects**

```bash
cd backend
dotnet new sln -n V4H
dotnet new classlib -n V4H.Domain -o src/V4H.Domain --framework net8.0
dotnet new classlib -n V4H.Application -o src/V4H.Application --framework net8.0
dotnet new classlib -n V4H.Infrastructure -o src/V4H.Infrastructure --framework net8.0
dotnet new webapi -n V4H.API -o src/V4H.API --framework net8.0 --no-openapi
dotnet new xunit -n V4H.Application.Tests -o tests/V4H.Application.Tests --framework net8.0
dotnet new xunit -n V4H.API.Tests -o tests/V4H.API.Tests --framework net8.0
dotnet sln add src/V4H.Domain src/V4H.Application src/V4H.Infrastructure src/V4H.API tests/V4H.Application.Tests tests/V4H.API.Tests
```

- [ ] **Step 5: Add project references**

```bash
cd backend
dotnet add src/V4H.Application/V4H.Application.csproj reference src/V4H.Domain/V4H.Domain.csproj
dotnet add src/V4H.Infrastructure/V4H.Infrastructure.csproj reference src/V4H.Application/V4H.Application.csproj
dotnet add src/V4H.API/V4H.API.csproj reference src/V4H.Application/V4H.Application.csproj
dotnet add src/V4H.API/V4H.API.csproj reference src/V4H.Infrastructure/V4H.Infrastructure.csproj
dotnet add tests/V4H.Application.Tests/V4H.Application.Tests.csproj reference src/V4H.Application/V4H.Application.csproj
dotnet add tests/V4H.Application.Tests/V4H.Application.Tests.csproj reference src/V4H.Domain/V4H.Domain.csproj
dotnet add tests/V4H.API.Tests/V4H.API.Tests.csproj reference src/V4H.API/V4H.API.csproj
```

- [ ] **Step 6: Add NuGet packages**

```bash
cd backend
# Application
dotnet add src/V4H.Application/V4H.Application.csproj package MediatR --version 12.4.1

# Infrastructure
dotnet add src/V4H.Infrastructure/V4H.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Design --version 8.0.11
dotnet add src/V4H.Infrastructure/V4H.Infrastructure.csproj package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.11
dotnet add src/V4H.Infrastructure/V4H.Infrastructure.csproj package BCrypt.Net-Next --version 4.0.3
dotnet add src/V4H.Infrastructure/V4H.Infrastructure.csproj package Microsoft.AspNetCore.SignalR --version 1.1.0
dotnet add src/V4H.Infrastructure/V4H.Infrastructure.csproj package QuestPDF --version 2024.10.3
dotnet add src/V4H.Infrastructure/V4H.Infrastructure.csproj package MediatR --version 12.4.1

# API
dotnet add src/V4H.API/V4H.API.csproj package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.11
dotnet add src/V4H.API/V4H.API.csproj package Swashbuckle.AspNetCore --version 6.9.0
dotnet add src/V4H.API/V4H.API.csproj package MediatR --version 12.4.1

# Test projects
dotnet add tests/V4H.Application.Tests/V4H.Application.Tests.csproj package NSubstitute --version 5.3.0
dotnet add tests/V4H.Application.Tests/V4H.Application.Tests.csproj package FluentAssertions --version 6.12.2
dotnet add tests/V4H.API.Tests/V4H.API.Tests.csproj package Microsoft.AspNetCore.Mvc.Testing --version 8.0.11
dotnet add tests/V4H.API.Tests/V4H.API.Tests.csproj package Microsoft.EntityFrameworkCore.InMemory --version 8.0.11
dotnet add tests/V4H.API.Tests/V4H.API.Tests.csproj package NSubstitute --version 5.3.0
```

- [ ] **Step 7: Delete generated placeholder files**

```bash
cd backend
Remove-Item src/V4H.Domain/Class1.cs -ErrorAction SilentlyContinue
Remove-Item src/V4H.Application/Class1.cs -ErrorAction SilentlyContinue
Remove-Item src/V4H.Infrastructure/Class1.cs -ErrorAction SilentlyContinue
Remove-Item src/V4H.API/WeatherForecast.cs -ErrorAction SilentlyContinue
```

Also delete the WeatherForecast endpoint from `src/V4H.API/Program.cs` (replace entire file — done in Task 9).

- [ ] **Step 8: Verify build**

```bash
cd backend
dotnet build
```
Expected: Build succeeded, 0 errors.

- [ ] **Step 9: Commit**

```bash
git add backend/ docker-compose.yml .env.example
git commit -m "chore: backend solution scaffold with NuGet packages"
```

---

### Task 2: Domain Layer — Enums + Entities

**Files:**
- Create: `backend/src/V4H.Domain/Enums/UserRole.cs`
- Create: `backend/src/V4H.Domain/Enums/Specialty.cs`
- Create: `backend/src/V4H.Domain/Enums/TeleconsultoriaStatus.cs`
- Create: `backend/src/V4H.Domain/Entities/User.cs`
- Create: `backend/src/V4H.Domain/Entities/Teleconsultoria.cs`
- Create: `backend/src/V4H.Domain/Entities/TeleconsultoriaDocument.cs`
- Create: `backend/src/V4H.Domain/Entities/StatusHistory.cs`
- Create: `backend/src/V4H.Domain/Entities/Opinion.cs`

- [ ] **Step 1: Create enums**

`backend/src/V4H.Domain/Enums/UserRole.cs`:
```csharp
namespace V4H.Domain.Enums;

public enum UserRole
{
    Solicitante = 1,
    Especialista = 2
}
```

`backend/src/V4H.Domain/Enums/Specialty.cs`:
```csharp
namespace V4H.Domain.Enums;

public enum Specialty
{
    Cardiologia = 1,
    CirurgiaRobotica = 2,
    Odontologia = 3,
    DoencasRaras = 4,
    Oxigenoterapia = 5
}
```

`backend/src/V4H.Domain/Enums/TeleconsultoriaStatus.cs`:
```csharp
namespace V4H.Domain.Enums;

public enum TeleconsultoriaStatus
{
    Pendente = 1,
    EmAndamento = 2,
    Concluida = 3,
    Cancelada = 4
}
```

- [ ] **Step 2: Create User entity**

`backend/src/V4H.Domain/Entities/User.cs`:
```csharp
using V4H.Domain.Enums;

namespace V4H.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public UserRole Role { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private User() { }

    public static User Create(string name, string email, string passwordHash, UserRole role)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            PasswordHash = passwordHash,
            Role = role,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
```

- [ ] **Step 3: Create Teleconsultoria entity**

`backend/src/V4H.Domain/Entities/Teleconsultoria.cs`:
```csharp
using V4H.Domain.Enums;

namespace V4H.Domain.Entities;

public class Teleconsultoria
{
    public Guid Id { get; private set; }
    public string PatientName { get; private set; } = default!;
    public DateOnly BirthDate { get; private set; }
    public Specialty Specialty { get; private set; }
    public string DiagnosticHypothesis { get; private set; } = default!;
    public string ClinicalHistory { get; private set; } = default!;
    public TeleconsultoriaStatus Status { get; private set; }
    public Guid RequesterId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public User Requester { get; private set; } = default!;
    public ICollection<TeleconsultoriaDocument> Documents { get; private set; } = new List<TeleconsultoriaDocument>();
    public ICollection<StatusHistory> StatusHistories { get; private set; } = new List<StatusHistory>();
    public ICollection<Opinion> Opinions { get; private set; } = new List<Opinion>();

    private Teleconsultoria() { }

    public static Teleconsultoria Create(
        string patientName,
        DateOnly birthDate,
        Specialty specialty,
        string diagnosticHypothesis,
        string clinicalHistory,
        Guid requesterId)
    {
        return new Teleconsultoria
        {
            Id = Guid.NewGuid(),
            PatientName = patientName,
            BirthDate = birthDate,
            Specialty = specialty,
            DiagnosticHypothesis = diagnosticHypothesis,
            ClinicalHistory = clinicalHistory,
            Status = TeleconsultoriaStatus.Pendente,
            RequesterId = requesterId,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    public void UpdateStatus(TeleconsultoriaStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
```

- [ ] **Step 4: Create remaining entities**

`backend/src/V4H.Domain/Entities/TeleconsultoriaDocument.cs`:
```csharp
namespace V4H.Domain.Entities;

public class TeleconsultoriaDocument
{
    public Guid Id { get; private set; }
    public Guid TeleconsultoriaId { get; private set; }
    public string FileName { get; private set; } = default!;
    public string StoredPath { get; private set; } = default!;
    public decimal ValidationScore { get; private set; }
    public string ValidationProvider { get; private set; } = default!;
    public decimal ValidationThreshold { get; private set; }
    public DateTimeOffset ValidatedAt { get; private set; }
    public bool IsApproved { get; private set; }

    public Teleconsultoria Teleconsultoria { get; private set; } = default!;

    private TeleconsultoriaDocument() { }

    public static TeleconsultoriaDocument Create(
        Guid teleconsultoriaId,
        string fileName,
        string storedPath,
        decimal score,
        string provider,
        decimal threshold,
        DateTimeOffset validatedAt)
    {
        return new TeleconsultoriaDocument
        {
            Id = Guid.NewGuid(),
            TeleconsultoriaId = teleconsultoriaId,
            FileName = fileName,
            StoredPath = storedPath,
            ValidationScore = score,
            ValidationProvider = provider,
            ValidationThreshold = threshold,
            ValidatedAt = validatedAt,
            IsApproved = score >= threshold
        };
    }
}
```

`backend/src/V4H.Domain/Entities/StatusHistory.cs`:
```csharp
using V4H.Domain.Enums;

namespace V4H.Domain.Entities;

public class StatusHistory
{
    public Guid Id { get; private set; }
    public Guid TeleconsultoriaId { get; private set; }
    public TeleconsultoriaStatus OldStatus { get; private set; }
    public TeleconsultoriaStatus NewStatus { get; private set; }
    public DateTimeOffset ChangedAt { get; private set; }
    public Guid ChangedById { get; private set; }
    public string? Notes { get; private set; }

    public Teleconsultoria Teleconsultoria { get; private set; } = default!;
    public User ChangedBy { get; private set; } = default!;

    private StatusHistory() { }

    public static StatusHistory Create(
        Guid teleconsultoriaId,
        TeleconsultoriaStatus oldStatus,
        TeleconsultoriaStatus newStatus,
        Guid changedById,
        string? notes = null)
    {
        return new StatusHistory
        {
            Id = Guid.NewGuid(),
            TeleconsultoriaId = teleconsultoriaId,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            ChangedAt = DateTimeOffset.UtcNow,
            ChangedById = changedById,
            Notes = notes
        };
    }
}
```

`backend/src/V4H.Domain/Entities/Opinion.cs`:
```csharp
namespace V4H.Domain.Entities;

public class Opinion
{
    public Guid Id { get; private set; }
    public Guid TeleconsultoriaId { get; private set; }
    public Guid SpecialistId { get; private set; }
    public string Content { get; private set; } = default!;
    public DateTimeOffset CreatedAt { get; private set; }

    public Teleconsultoria Teleconsultoria { get; private set; } = default!;
    public User Specialist { get; private set; } = default!;

    private Opinion() { }

    public static Opinion Create(Guid teleconsultoriaId, Guid specialistId, string content)
    {
        return new Opinion
        {
            Id = Guid.NewGuid(),
            TeleconsultoriaId = teleconsultoriaId,
            SpecialistId = specialistId,
            Content = content,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
```

- [ ] **Step 5: Create repository interfaces**

`backend/src/V4H.Domain/Interfaces/Repositories/IUserRepository.cs`:
```csharp
using V4H.Domain.Entities;

namespace V4H.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
```

`backend/src/V4H.Domain/Interfaces/Repositories/ITeleconsultoriaRepository.cs`:
```csharp
using V4H.Domain.Entities;
using V4H.Domain.Enums;

namespace V4H.Domain.Interfaces.Repositories;

public interface ITeleconsultoriaRepository
{
    Task<List<Teleconsultoria>> ListAsync(
        Specialty? specialty,
        string? patient,
        TeleconsultoriaStatus? status,
        DateTimeOffset? dateFrom,
        DateTimeOffset? dateTo,
        CancellationToken ct = default);

    Task<Teleconsultoria?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Teleconsultoria teleconsultoria, CancellationToken ct = default);
    Task AddDocumentAsync(TeleconsultoriaDocument document, CancellationToken ct = default);
    Task AddStatusHistoryAsync(StatusHistory history, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
```

`backend/src/V4H.Domain/Interfaces/Repositories/IOpinionRepository.cs`:
```csharp
using V4H.Domain.Entities;

namespace V4H.Domain.Interfaces.Repositories;

public interface IOpinionRepository
{
    Task AddAsync(Opinion opinion, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
```

- [ ] **Step 6: Build to verify**

```bash
cd backend
dotnet build src/V4H.Domain
```
Expected: Build succeeded, 0 errors.

- [ ] **Step 7: Commit**

```bash
git add backend/src/V4H.Domain/
git commit -m "feat(domain): entities, enums, repository interfaces"
```

---

### Task 3: Application Layer — Interfaces + Exceptions + Auth Commands

**Files:**
- Create: `backend/src/V4H.Application/Common/Exceptions/NotFoundException.cs`
- Create: `backend/src/V4H.Application/Common/Exceptions/UnauthorizedException.cs`
- Create: `backend/src/V4H.Application/Common/Exceptions/DocumentValidationException.cs`
- Create: `backend/src/V4H.Application/Common/Interfaces/IPasswordHasher.cs`
- Create: `backend/src/V4H.Application/Common/Interfaces/IJwtService.cs`
- Create: `backend/src/V4H.Application/Common/Interfaces/IDocumentValidationService.cs`
- Create: `backend/src/V4H.Application/Common/Interfaces/IFileStorageService.cs`
- Create: `backend/src/V4H.Application/Common/Interfaces/INotificationService.cs`
- Create: `backend/src/V4H.Application/Common/Interfaces/IPdfExportService.cs`
- Create: `backend/src/V4H.Application/Auth/Commands/RegisterCommand.cs`
- Create: `backend/src/V4H.Application/Auth/Commands/RegisterCommandHandler.cs`
- Create: `backend/src/V4H.Application/Auth/Commands/LoginCommand.cs`
- Create: `backend/src/V4H.Application/Auth/Commands/LoginCommandHandler.cs`
- Create: `backend/src/V4H.Application/Auth/DTOs/AuthResultDto.cs`
- Test: `backend/tests/V4H.Application.Tests/Auth/RegisterCommandHandlerTests.cs`
- Test: `backend/tests/V4H.Application.Tests/Auth/LoginCommandHandlerTests.cs`

- [ ] **Step 1: Create exceptions**

`backend/src/V4H.Application/Common/Exceptions/NotFoundException.cs`:
```csharp
namespace V4H.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found.") { }
}
```

`backend/src/V4H.Application/Common/Exceptions/UnauthorizedException.cs`:
```csharp
namespace V4H.Application.Common.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message = "Access denied.") : base(message) { }
}
```

`backend/src/V4H.Application/Common/Exceptions/DocumentValidationException.cs`:
```csharp
namespace V4H.Application.Common.Exceptions;

public class DocumentValidationException : Exception
{
    public decimal Score { get; }

    public DocumentValidationException(decimal score)
        : base($"Document validation failed. Score: {score:F2}")
    {
        Score = score;
    }
}
```

- [ ] **Step 2: Create application interfaces**

`backend/src/V4H.Application/Common/Interfaces/IPasswordHasher.cs`:
```csharp
namespace V4H.Application.Common.Interfaces;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
```

`backend/src/V4H.Application/Common/Interfaces/IJwtService.cs`:
```csharp
namespace V4H.Application.Common.Interfaces;

public interface IJwtService
{
    string Generate(Guid userId, string email, string role);
}
```

`backend/src/V4H.Application/Common/Interfaces/IDocumentValidationService.cs`:
```csharp
namespace V4H.Application.Common.Interfaces;

public record ValidationResult(decimal Score, string Provider, DateTimeOffset Timestamp);

public interface IDocumentValidationService
{
    Task<ValidationResult> ValidateAsync(Stream file, string mimeType, CancellationToken ct = default);
}
```

`backend/src/V4H.Application/Common/Interfaces/IFileStorageService.cs`:
```csharp
namespace V4H.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveAsync(Stream content, string fileName, CancellationToken ct = default);
}
```

`backend/src/V4H.Application/Common/Interfaces/INotificationService.cs`:
```csharp
namespace V4H.Application.Common.Interfaces;

public interface INotificationService
{
    Task SendNewOpinionAsync(Guid requesterId, Guid teleconsultoriaId, Guid opinionId, CancellationToken ct = default);
}
```

`backend/src/V4H.Application/Common/Interfaces/IPdfExportService.cs`:
```csharp
using V4H.Domain.Entities;

namespace V4H.Application.Common.Interfaces;

public interface IPdfExportService
{
    byte[] Export(Teleconsultoria teleconsultoria);
}
```

- [ ] **Step 3: Write failing tests for RegisterCommandHandler**

`backend/tests/V4H.Application.Tests/Auth/RegisterCommandHandlerTests.cs`:
```csharp
using FluentAssertions;
using NSubstitute;
using V4H.Application.Auth.Commands;
using V4H.Application.Common.Interfaces;
using V4H.Domain.Enums;
using V4H.Domain.Interfaces.Repositories;
using Xunit;

namespace V4H.Application.Tests.Auth;

public class RegisterCommandHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();

    private RegisterCommandHandler CreateHandler() => new(_users, _hasher);

    [Fact]
    public async Task Handle_NewUser_ReturnsGuid()
    {
        _users.GetByEmailAsync("test@test.com").Returns((V4H.Domain.Entities.User?)null);
        _hasher.Hash("pass123").Returns("hashed");

        var cmd = new RegisterCommand("Joao", "test@test.com", "pass123", UserRole.Solicitante);
        var result = await CreateHandler().Handle(cmd, default);

        result.Should().NotBeEmpty();
        await _users.Received(1).AddAsync(Arg.Any<V4H.Domain.Entities.User>());
        await _users.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsInvalidOperation()
    {
        var existing = V4H.Domain.Entities.User.Create("X", "test@test.com", "h", UserRole.Solicitante);
        _users.GetByEmailAsync("test@test.com").Returns(existing);

        var cmd = new RegisterCommand("Y", "test@test.com", "pass", UserRole.Solicitante);
        var act = () => CreateHandler().Handle(cmd, default);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
```

- [ ] **Step 4: Run test — verify FAIL**

```bash
cd backend
dotnet test tests/V4H.Application.Tests --filter "Auth" --no-build 2>&1 | tail -5
```
Expected: error — `RegisterCommandHandler` not found.

- [ ] **Step 5: Implement auth commands**

`backend/src/V4H.Application/Auth/DTOs/AuthResultDto.cs`:
```csharp
namespace V4H.Application.Auth.DTOs;

public record AuthResultDto(string Token, Guid UserId, string Name, string Email, string Role);
```

`backend/src/V4H.Application/Auth/Commands/RegisterCommand.cs`:
```csharp
using MediatR;
using V4H.Domain.Enums;

namespace V4H.Application.Auth.Commands;

public record RegisterCommand(string Name, string Email, string Password, UserRole Role) : IRequest<Guid>;
```

`backend/src/V4H.Application/Auth/Commands/RegisterCommandHandler.cs`:
```csharp
using MediatR;
using V4H.Application.Common.Interfaces;
using V4H.Domain.Entities;
using V4H.Domain.Interfaces.Repositories;

namespace V4H.Application.Auth.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Guid>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;

    public RegisterCommandHandler(IUserRepository users, IPasswordHasher hasher)
    {
        _users = users;
        _hasher = hasher;
    }

    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existing = await _users.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException($"Email '{request.Email}' already registered.");

        var hash = _hasher.Hash(request.Password);
        var user = User.Create(request.Name, request.Email, hash, request.Role);

        await _users.AddAsync(user, cancellationToken);
        await _users.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
```

`backend/src/V4H.Application/Auth/Commands/LoginCommand.cs`:
```csharp
using MediatR;
using V4H.Application.Auth.DTOs;

namespace V4H.Application.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<AuthResultDto>;
```

`backend/src/V4H.Application/Auth/Commands/LoginCommandHandler.cs`:
```csharp
using MediatR;
using V4H.Application.Auth.DTOs;
using V4H.Application.Common.Exceptions;
using V4H.Application.Common.Interfaces;
using V4H.Domain.Interfaces.Repositories;

namespace V4H.Application.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResultDto>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtService _jwt;

    public LoginCommandHandler(IUserRepository users, IPasswordHasher hasher, IJwtService jwt)
    {
        _users = users;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<AuthResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByEmailAsync(request.Email, cancellationToken)
            ?? throw new UnauthorizedException("Invalid credentials.");

        if (!_hasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid credentials.");

        var token = _jwt.Generate(user.Id, user.Email, user.Role.ToString());
        return new AuthResultDto(token, user.Id, user.Name, user.Email, user.Role.ToString());
    }
}
```

- [ ] **Step 6: Write LoginCommandHandler test**

`backend/tests/V4H.Application.Tests/Auth/LoginCommandHandlerTests.cs`:
```csharp
using FluentAssertions;
using NSubstitute;
using V4H.Application.Auth.Commands;
using V4H.Application.Common.Exceptions;
using V4H.Application.Common.Interfaces;
using V4H.Domain.Enums;
using V4H.Domain.Interfaces.Repositories;
using Xunit;

namespace V4H.Application.Tests.Auth;

public class LoginCommandHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtService _jwt = Substitute.For<IJwtService>();

    private LoginCommandHandler CreateHandler() => new(_users, _hasher, _jwt);

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsToken()
    {
        var user = V4H.Domain.Entities.User.Create("Joao", "j@j.com", "hash", UserRole.Solicitante);
        _users.GetByEmailAsync("j@j.com").Returns(user);
        _hasher.Verify("pass", "hash").Returns(true);
        _jwt.Generate(user.Id, "j@j.com", "Solicitante").Returns("tok");

        var result = await CreateHandler().Handle(new LoginCommand("j@j.com", "pass"), default);

        result.Token.Should().Be("tok");
        result.Role.Should().Be("Solicitante");
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsUnauthorized()
    {
        var user = V4H.Domain.Entities.User.Create("X", "x@x.com", "hash", UserRole.Solicitante);
        _users.GetByEmailAsync("x@x.com").Returns(user);
        _hasher.Verify("wrong", "hash").Returns(false);

        var act = () => CreateHandler().Handle(new LoginCommand("x@x.com", "wrong"), default);
        await act.Should().ThrowAsync<UnauthorizedException>();
    }
}
```

- [ ] **Step 7: Run tests — verify PASS**

```bash
cd backend
dotnet test tests/V4H.Application.Tests --filter "Auth" -v minimal
```
Expected: 4 passed.

- [ ] **Step 8: Commit**

```bash
git add backend/src/V4H.Application/ backend/tests/V4H.Application.Tests/Auth/
git commit -m "feat(application): auth commands with tests"
```

---

### Task 4: Application Layer — Teleconsultoria Commands + Queries

**Files:**
- Create: all files in `backend/src/V4H.Application/Teleconsultorias/`
- Test: `backend/tests/V4H.Application.Tests/Teleconsultorias/`

- [ ] **Step 1: Create DTOs**

`backend/src/V4H.Application/Teleconsultorias/DTOs/DocumentDto.cs`:
```csharp
namespace V4H.Application.Teleconsultorias.DTOs;

public record DocumentDto(
    Guid Id,
    string FileName,
    decimal ValidationScore,
    bool IsApproved,
    DateTimeOffset ValidatedAt);
```

`backend/src/V4H.Application/Teleconsultorias/DTOs/OpinionDto.cs`:
```csharp
namespace V4H.Application.Teleconsultorias.DTOs;

public record OpinionDto(
    Guid Id,
    string SpecialistName,
    string Content,
    DateTimeOffset CreatedAt);
```

`backend/src/V4H.Application/Teleconsultorias/DTOs/TeleconsultoriaListItemDto.cs`:
```csharp
namespace V4H.Application.Teleconsultorias.DTOs;

public record TeleconsultoriaListItemDto(
    Guid Id,
    string PatientName,
    string Specialty,
    string Status,
    string RequesterName,
    DateTimeOffset CreatedAt);
```

`backend/src/V4H.Application/Teleconsultorias/DTOs/TeleconsultoriaDetailDto.cs`:
```csharp
namespace V4H.Application.Teleconsultorias.DTOs;

public record TeleconsultoriaDetailDto(
    Guid Id,
    string PatientName,
    DateOnly BirthDate,
    string Specialty,
    string DiagnosticHypothesis,
    string ClinicalHistory,
    string Status,
    string RequesterName,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    List<DocumentDto> Documents,
    List<OpinionDto> Opinions);
```

- [ ] **Step 2: Create CreateTeleconsultoria command**

`backend/src/V4H.Application/Teleconsultorias/Commands/CreateTeleconsultoriaCommand.cs`:
```csharp
using MediatR;
using V4H.Domain.Enums;

namespace V4H.Application.Teleconsultorias.Commands;

public record CreateTeleconsultoriaCommand(
    string PatientName,
    DateOnly BirthDate,
    Specialty Specialty,
    string DiagnosticHypothesis,
    string ClinicalHistory,
    Guid RequesterId) : IRequest<Guid>;
```

`backend/src/V4H.Application/Teleconsultorias/Commands/CreateTeleconsultoriaCommandHandler.cs`:
```csharp
using MediatR;
using V4H.Domain.Entities;
using V4H.Domain.Interfaces.Repositories;

namespace V4H.Application.Teleconsultorias.Commands;

public class CreateTeleconsultoriaCommandHandler : IRequestHandler<CreateTeleconsultoriaCommand, Guid>
{
    private readonly ITeleconsultoriaRepository _repo;

    public CreateTeleconsultoriaCommandHandler(ITeleconsultoriaRepository repo) => _repo = repo;

    public async Task<Guid> Handle(CreateTeleconsultoriaCommand request, CancellationToken cancellationToken)
    {
        var tc = Teleconsultoria.Create(
            request.PatientName,
            request.BirthDate,
            request.Specialty,
            request.DiagnosticHypothesis,
            request.ClinicalHistory,
            request.RequesterId);

        await _repo.AddAsync(tc, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);

        return tc.Id;
    }
}
```

- [ ] **Step 3: Create UpdateStatus command**

`backend/src/V4H.Application/Teleconsultorias/Commands/UpdateStatusCommand.cs`:
```csharp
using MediatR;
using V4H.Domain.Enums;

namespace V4H.Application.Teleconsultorias.Commands;

public record UpdateStatusCommand(
    Guid TeleconsultoriaId,
    TeleconsultoriaStatus NewStatus,
    Guid ChangedById,
    string? Notes) : IRequest;
```

`backend/src/V4H.Application/Teleconsultorias/Commands/UpdateStatusCommandHandler.cs`:
```csharp
using MediatR;
using V4H.Application.Common.Exceptions;
using V4H.Domain.Entities;
using V4H.Domain.Interfaces.Repositories;

namespace V4H.Application.Teleconsultorias.Commands;

public class UpdateStatusCommandHandler : IRequestHandler<UpdateStatusCommand>
{
    private readonly ITeleconsultoriaRepository _repo;

    public UpdateStatusCommandHandler(ITeleconsultoriaRepository repo) => _repo = repo;

    public async Task Handle(UpdateStatusCommand request, CancellationToken cancellationToken)
    {
        var tc = await _repo.GetByIdWithDetailsAsync(request.TeleconsultoriaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Teleconsultoria), request.TeleconsultoriaId);

        var history = StatusHistory.Create(
            tc.Id, tc.Status, request.NewStatus, request.ChangedById, request.Notes);

        tc.UpdateStatus(request.NewStatus);

        await _repo.AddStatusHistoryAsync(history, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
    }
}
```

- [ ] **Step 4: Create UploadDocument command**

`backend/src/V4H.Application/Teleconsultorias/Commands/UploadDocumentCommand.cs`:
```csharp
using MediatR;

namespace V4H.Application.Teleconsultorias.Commands;

public record UploadDocumentCommand(
    Guid TeleconsultoriaId,
    Stream FileStream,
    string FileName,
    string MimeType,
    Guid RequesterId) : IRequest<Guid>;
```

`backend/src/V4H.Application/Teleconsultorias/Commands/UploadDocumentCommandHandler.cs`:
```csharp
using MediatR;
using V4H.Application.Common.Exceptions;
using V4H.Application.Common.Interfaces;
using V4H.Domain.Entities;
using V4H.Domain.Interfaces.Repositories;

namespace V4H.Application.Teleconsultorias.Commands;

public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, Guid>
{
    private readonly ITeleconsultoriaRepository _repo;
    private readonly IDocumentValidationService _validator;
    private readonly IFileStorageService _storage;
    private readonly IConfiguration _config;

    public UploadDocumentCommandHandler(
        ITeleconsultoriaRepository repo,
        IDocumentValidationService validator,
        IFileStorageService storage,
        IConfiguration config)
    {
        _repo = repo;
        _validator = validator;
        _storage = storage;
        _config = config;
    }

    public async Task<Guid> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        var tc = await _repo.GetByIdWithDetailsAsync(request.TeleconsultoriaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Teleconsultoria), request.TeleconsultoriaId);

        if (tc.RequesterId != request.RequesterId)
            throw new UnauthorizedException("Only the requester can upload documents.");

        // Copy stream so we can use it for both validation and storage
        using var ms = new MemoryStream();
        await request.FileStream.CopyToAsync(ms, cancellationToken);
        var bytes = ms.ToArray();

        var threshold = _config.GetValue<decimal>("AI:ValidationThreshold", 0.6m);
        var validationResult = await _validator.ValidateAsync(
            new MemoryStream(bytes), request.MimeType, cancellationToken);

        if (validationResult.Score < threshold)
            throw new DocumentValidationException(validationResult.Score);

        var storedPath = await _storage.SaveAsync(
            new MemoryStream(bytes), request.FileName, cancellationToken);

        var doc = TeleconsultoriaDocument.Create(
            tc.Id,
            request.FileName,
            storedPath,
            validationResult.Score,
            validationResult.Provider,
            threshold,
            validationResult.Timestamp);

        await _repo.AddDocumentAsync(doc, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);

        return doc.Id;
    }
}
```

Note: `IConfiguration` is from `Microsoft.Extensions.Configuration`. Add to `V4H.Application.csproj`:
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="8.0.0" />
</ItemGroup>
```

- [ ] **Step 5: Create RegisterOpinion command**

`backend/src/V4H.Application/Teleconsultorias/Commands/RegisterOpinionCommand.cs`:
```csharp
using MediatR;

namespace V4H.Application.Teleconsultorias.Commands;

public record RegisterOpinionCommand(
    Guid TeleconsultoriaId,
    Guid SpecialistId,
    string Content) : IRequest<Guid>;
```

`backend/src/V4H.Application/Teleconsultorias/Commands/RegisterOpinionCommandHandler.cs`:
```csharp
using MediatR;
using V4H.Application.Common.Exceptions;
using V4H.Application.Common.Interfaces;
using V4H.Domain.Entities;
using V4H.Domain.Enums;
using V4H.Domain.Interfaces.Repositories;

namespace V4H.Application.Teleconsultorias.Commands;

public class RegisterOpinionCommandHandler : IRequestHandler<RegisterOpinionCommand, Guid>
{
    private readonly ITeleconsultoriaRepository _repo;
    private readonly IOpinionRepository _opinions;
    private readonly INotificationService _notifications;

    public RegisterOpinionCommandHandler(
        ITeleconsultoriaRepository repo,
        IOpinionRepository opinions,
        INotificationService notifications)
    {
        _repo = repo;
        _opinions = opinions;
        _notifications = notifications;
    }

    public async Task<Guid> Handle(RegisterOpinionCommand request, CancellationToken cancellationToken)
    {
        var tc = await _repo.GetByIdWithDetailsAsync(request.TeleconsultoriaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Teleconsultoria), request.TeleconsultoriaId);

        var opinion = Opinion.Create(tc.Id, request.SpecialistId, request.Content);
        await _opinions.AddAsync(opinion, cancellationToken);

        var oldStatus = tc.Status;
        tc.UpdateStatus(TeleconsultoriaStatus.Concluida);
        var history = StatusHistory.Create(tc.Id, oldStatus, TeleconsultoriaStatus.Concluida, request.SpecialistId);
        await _repo.AddStatusHistoryAsync(history, cancellationToken);

        await _repo.SaveChangesAsync(cancellationToken);
        await _opinions.SaveChangesAsync(cancellationToken);

        await _notifications.SendNewOpinionAsync(tc.RequesterId, tc.Id, opinion.Id, cancellationToken);

        return opinion.Id;
    }
}
```

- [ ] **Step 6: Create queries**

`backend/src/V4H.Application/Teleconsultorias/Queries/ListTeleconsultoriasQuery.cs`:
```csharp
using MediatR;
using V4H.Application.Teleconsultorias.DTOs;
using V4H.Domain.Enums;

namespace V4H.Application.Teleconsultorias.Queries;

public record ListTeleconsultoriasQuery(
    Specialty? Specialty,
    string? Patient,
    TeleconsultoriaStatus? Status,
    DateTimeOffset? DateFrom,
    DateTimeOffset? DateTo) : IRequest<List<TeleconsultoriaListItemDto>>;
```

`backend/src/V4H.Application/Teleconsultorias/Queries/ListTeleconsultoriasQueryHandler.cs`:
```csharp
using MediatR;
using V4H.Application.Teleconsultorias.DTOs;
using V4H.Domain.Interfaces.Repositories;

namespace V4H.Application.Teleconsultorias.Queries;

public class ListTeleconsultoriasQueryHandler
    : IRequestHandler<ListTeleconsultoriasQuery, List<TeleconsultoriaListItemDto>>
{
    private readonly ITeleconsultoriaRepository _repo;

    public ListTeleconsultoriasQueryHandler(ITeleconsultoriaRepository repo) => _repo = repo;

    public async Task<List<TeleconsultoriaListItemDto>> Handle(
        ListTeleconsultoriasQuery request, CancellationToken cancellationToken)
    {
        var list = await _repo.ListAsync(
            request.Specialty, request.Patient, request.Status,
            request.DateFrom, request.DateTo, cancellationToken);

        return list.Select(tc => new TeleconsultoriaListItemDto(
            tc.Id,
            tc.PatientName,
            tc.Specialty.ToString(),
            tc.Status.ToString(),
            tc.Requester?.Name ?? "",
            tc.CreatedAt)).ToList();
    }
}
```

`backend/src/V4H.Application/Teleconsultorias/Queries/GetTeleconsultoriaDetailQuery.cs`:
```csharp
using MediatR;
using V4H.Application.Teleconsultorias.DTOs;

namespace V4H.Application.Teleconsultorias.Queries;

public record GetTeleconsultoriaDetailQuery(Guid Id) : IRequest<TeleconsultoriaDetailDto>;
```

`backend/src/V4H.Application/Teleconsultorias/Queries/GetTeleconsultoriaDetailQueryHandler.cs`:
```csharp
using MediatR;
using V4H.Application.Common.Exceptions;
using V4H.Application.Teleconsultorias.DTOs;
using V4H.Domain.Entities;
using V4H.Domain.Interfaces.Repositories;

namespace V4H.Application.Teleconsultorias.Queries;

public class GetTeleconsultoriaDetailQueryHandler
    : IRequestHandler<GetTeleconsultoriaDetailQuery, TeleconsultoriaDetailDto>
{
    private readonly ITeleconsultoriaRepository _repo;

    public GetTeleconsultoriaDetailQueryHandler(ITeleconsultoriaRepository repo) => _repo = repo;

    public async Task<TeleconsultoriaDetailDto> Handle(
        GetTeleconsultoriaDetailQuery request, CancellationToken cancellationToken)
    {
        var tc = await _repo.GetByIdWithDetailsAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Teleconsultoria), request.Id);

        return new TeleconsultoriaDetailDto(
            tc.Id,
            tc.PatientName,
            tc.BirthDate,
            tc.Specialty.ToString(),
            tc.DiagnosticHypothesis,
            tc.ClinicalHistory,
            tc.Status.ToString(),
            tc.Requester?.Name ?? "",
            tc.CreatedAt,
            tc.UpdatedAt,
            tc.Documents.Select(d => new DocumentDto(
                d.Id, d.FileName, d.ValidationScore, d.IsApproved, d.ValidatedAt)).ToList(),
            tc.Opinions.Select(o => new OpinionDto(
                o.Id, o.Specialist?.Name ?? "", o.Content, o.CreatedAt)).ToList());
    }
}
```

`backend/src/V4H.Application/Teleconsultorias/Queries/ExportPdfQuery.cs`:
```csharp
using MediatR;

namespace V4H.Application.Teleconsultorias.Queries;

public record ExportPdfQuery(Guid Id) : IRequest<byte[]>;
```

`backend/src/V4H.Application/Teleconsultorias/Queries/ExportPdfQueryHandler.cs`:
```csharp
using MediatR;
using V4H.Application.Common.Exceptions;
using V4H.Application.Common.Interfaces;
using V4H.Domain.Entities;
using V4H.Domain.Interfaces.Repositories;

namespace V4H.Application.Teleconsultorias.Queries;

public class ExportPdfQueryHandler : IRequestHandler<ExportPdfQuery, byte[]>
{
    private readonly ITeleconsultoriaRepository _repo;
    private readonly IPdfExportService _pdf;

    public ExportPdfQueryHandler(ITeleconsultoriaRepository repo, IPdfExportService pdf)
    {
        _repo = repo;
        _pdf = pdf;
    }

    public async Task<byte[]> Handle(ExportPdfQuery request, CancellationToken cancellationToken)
    {
        var tc = await _repo.GetByIdWithDetailsAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Teleconsultoria), request.Id);

        return _pdf.Export(tc);
    }
}
```

- [ ] **Step 7: Write unit tests for command handlers**

`backend/tests/V4H.Application.Tests/Teleconsultorias/CreateTeleconsultoriaCommandHandlerTests.cs`:
```csharp
using FluentAssertions;
using NSubstitute;
using V4H.Application.Teleconsultorias.Commands;
using V4H.Domain.Enums;
using V4H.Domain.Interfaces.Repositories;
using Xunit;

namespace V4H.Application.Tests.Teleconsultorias;

public class CreateTeleconsultoriaCommandHandlerTests
{
    private readonly ITeleconsultoriaRepository _repo = Substitute.For<ITeleconsultoriaRepository>();

    [Fact]
    public async Task Handle_ValidCommand_ReturnsNewId()
    {
        var cmd = new CreateTeleconsultoriaCommand(
            "Paciente X", new DateOnly(1990, 1, 1),
            Specialty.Cardiologia, "Hipotese", "Historia", Guid.NewGuid());

        var handler = new CreateTeleconsultoriaCommandHandler(_repo);
        var id = await handler.Handle(cmd, default);

        id.Should().NotBeEmpty();
        await _repo.Received(1).AddAsync(Arg.Any<V4H.Domain.Entities.Teleconsultoria>());
        await _repo.Received(1).SaveChangesAsync();
    }
}
```

`backend/tests/V4H.Application.Tests/Teleconsultorias/UploadDocumentCommandHandlerTests.cs`:
```csharp
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using V4H.Application.Common.Exceptions;
using V4H.Application.Common.Interfaces;
using V4H.Application.Teleconsultorias.Commands;
using V4H.Domain.Entities;
using V4H.Domain.Enums;
using V4H.Domain.Interfaces.Repositories;
using Xunit;

namespace V4H.Application.Tests.Teleconsultorias;

public class UploadDocumentCommandHandlerTests
{
    private readonly ITeleconsultoriaRepository _repo = Substitute.For<ITeleconsultoriaRepository>();
    private readonly IDocumentValidationService _validator = Substitute.For<IDocumentValidationService>();
    private readonly IFileStorageService _storage = Substitute.For<IFileStorageService>();

    private IConfiguration BuildConfig(decimal threshold = 0.6m)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                ["AI:ValidationThreshold"] = threshold.ToString()
            }).Build();
    }

    private UploadDocumentCommandHandler CreateHandler(decimal threshold = 0.6m)
        => new(_repo, _validator, _storage, BuildConfig(threshold));

    private static Teleconsultoria MakeTc(Guid requesterId)
    {
        var tc = Teleconsultoria.Create("P", new DateOnly(2000,1,1),
            Specialty.Cardiologia, "H", "C", requesterId);
        return tc;
    }

    [Fact]
    public async Task Handle_ScoreAboveThreshold_ReturnsDocumentId()
    {
        var requesterId = Guid.NewGuid();
        var tc = MakeTc(requesterId);
        _repo.GetByIdWithDetailsAsync(tc.Id).Returns(tc);
        _validator.ValidateAsync(Arg.Any<Stream>(), "application/pdf")
            .Returns(new ValidationResult(0.92m, "mock", DateTimeOffset.UtcNow));
        _storage.SaveAsync(Arg.Any<Stream>(), "test.pdf").Returns("/uploads/test.pdf");

        var cmd = new UploadDocumentCommand(tc.Id, new MemoryStream([1, 2, 3]), "test.pdf", "application/pdf", requesterId);
        var docId = await CreateHandler().Handle(cmd, default);

        docId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_ScoreBelowThreshold_ThrowsDocumentValidationException()
    {
        var requesterId = Guid.NewGuid();
        var tc = MakeTc(requesterId);
        _repo.GetByIdWithDetailsAsync(tc.Id).Returns(tc);
        _validator.ValidateAsync(Arg.Any<Stream>(), "text/plain")
            .Returns(new ValidationResult(0.25m, "mock", DateTimeOffset.UtcNow));

        var cmd = new UploadDocumentCommand(tc.Id, new MemoryStream([1]), "x.txt", "text/plain", requesterId);
        var act = () => CreateHandler().Handle(cmd, default);

        await act.Should().ThrowAsync<DocumentValidationException>();
    }

    [Fact]
    public async Task Handle_WrongRequester_ThrowsUnauthorized()
    {
        var tc = MakeTc(Guid.NewGuid());
        _repo.GetByIdWithDetailsAsync(tc.Id).Returns(tc);

        var cmd = new UploadDocumentCommand(tc.Id, new MemoryStream([1]), "x.pdf", "application/pdf", Guid.NewGuid());
        var act = () => CreateHandler().Handle(cmd, default);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }
}
```

`backend/tests/V4H.Application.Tests/Teleconsultorias/RegisterOpinionCommandHandlerTests.cs`:
```csharp
using FluentAssertions;
using NSubstitute;
using V4H.Application.Common.Interfaces;
using V4H.Application.Teleconsultorias.Commands;
using V4H.Domain.Entities;
using V4H.Domain.Enums;
using V4H.Domain.Interfaces.Repositories;
using Xunit;

namespace V4H.Application.Tests.Teleconsultorias;

public class RegisterOpinionCommandHandlerTests
{
    private readonly ITeleconsultoriaRepository _repo = Substitute.For<ITeleconsultoriaRepository>();
    private readonly IOpinionRepository _opinions = Substitute.For<IOpinionRepository>();
    private readonly INotificationService _notifications = Substitute.For<INotificationService>();

    private RegisterOpinionCommandHandler CreateHandler()
        => new(_repo, _opinions, _notifications);

    [Fact]
    public async Task Handle_ValidOpinion_UpdatesStatusAndNotifies()
    {
        var requesterId = Guid.NewGuid();
        var tc = Teleconsultoria.Create("P", new DateOnly(2000,1,1),
            Specialty.Cardiologia, "H", "C", requesterId);
        _repo.GetByIdWithDetailsAsync(tc.Id).Returns(tc);

        var specialistId = Guid.NewGuid();
        var cmd = new RegisterOpinionCommand(tc.Id, specialistId, "Parecer detalhado");
        var opinionId = await CreateHandler().Handle(cmd, default);

        opinionId.Should().NotBeEmpty();
        tc.Status.Should().Be(TeleconsultoriaStatus.Concluida);
        await _notifications.Received(1).SendNewOpinionAsync(requesterId, tc.Id, opinionId, default);
    }
}
```

- [ ] **Step 8: Run tests — verify PASS**

```bash
cd backend
dotnet test tests/V4H.Application.Tests -v minimal
```
Expected: All tests passed.

- [ ] **Step 9: Commit**

```bash
git add backend/src/V4H.Application/ backend/tests/V4H.Application.Tests/
git commit -m "feat(application): teleconsultoria CQRS handlers with unit tests"
```

---

### Task 5: Infrastructure — Persistence (EF Core + Repositories)

**Files:**
- Create: `backend/src/V4H.Infrastructure/Persistence/AppDbContext.cs`
- Create: `backend/src/V4H.Infrastructure/Persistence/Configurations/*.cs`
- Create: `backend/src/V4H.Infrastructure/Persistence/Repositories/*.cs`

- [ ] **Step 1: Create AppDbContext**

`backend/src/V4H.Infrastructure/Persistence/AppDbContext.cs`:
```csharp
using Microsoft.EntityFrameworkCore;
using V4H.Domain.Entities;

namespace V4H.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Teleconsultoria> Teleconsultorias => Set<Teleconsultoria>();
    public DbSet<TeleconsultoriaDocument> Documents => Set<TeleconsultoriaDocument>();
    public DbSet<StatusHistory> StatusHistories => Set<StatusHistory>();
    public DbSet<Opinion> Opinions => Set<Opinion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
```

- [ ] **Step 2: Create EF configurations**

`backend/src/V4H.Infrastructure/Persistence/Configurations/UserConfiguration.cs`:
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using V4H.Domain.Entities;

namespace V4H.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.Name).HasMaxLength(200).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(320).IsRequired();
        builder.Property(u => u.PasswordHash).IsRequired();
    }
}
```

`backend/src/V4H.Infrastructure/Persistence/Configurations/TeleconsultoriaConfiguration.cs`:
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using V4H.Domain.Entities;

namespace V4H.Infrastructure.Persistence.Configurations;

public class TeleconsultoriaConfiguration : IEntityTypeConfiguration<Teleconsultoria>
{
    public void Configure(EntityTypeBuilder<Teleconsultoria> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.PatientName).HasMaxLength(300).IsRequired();
        builder.Property(t => t.DiagnosticHypothesis).HasMaxLength(2000).IsRequired();
        builder.Property(t => t.ClinicalHistory).HasMaxLength(5000).IsRequired();
        builder.HasOne(t => t.Requester).WithMany()
            .HasForeignKey(t => t.RequesterId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(t => t.Documents).WithOne(d => d.Teleconsultoria)
            .HasForeignKey(d => d.TeleconsultoriaId);
        builder.HasMany(t => t.Opinions).WithOne(o => o.Teleconsultoria)
            .HasForeignKey(o => o.TeleconsultoriaId);
        builder.HasMany(t => t.StatusHistories).WithOne(s => s.Teleconsultoria)
            .HasForeignKey(s => s.TeleconsultoriaId);
    }
}
```

`backend/src/V4H.Infrastructure/Persistence/Configurations/DocumentConfiguration.cs`:
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using V4H.Domain.Entities;

namespace V4H.Infrastructure.Persistence.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<TeleconsultoriaDocument>
{
    public void Configure(EntityTypeBuilder<TeleconsultoriaDocument> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.FileName).HasMaxLength(500).IsRequired();
        builder.Property(d => d.StoredPath).HasMaxLength(1000).IsRequired();
        builder.Property(d => d.ValidationScore).HasPrecision(5, 4);
        builder.Property(d => d.ValidationThreshold).HasPrecision(5, 4);
        builder.Property(d => d.ValidationProvider).HasMaxLength(100);
    }
}
```

`backend/src/V4H.Infrastructure/Persistence/Configurations/StatusHistoryConfiguration.cs`:
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using V4H.Domain.Entities;

namespace V4H.Infrastructure.Persistence.Configurations;

public class StatusHistoryConfiguration : IEntityTypeConfiguration<StatusHistory>
{
    public void Configure(EntityTypeBuilder<StatusHistory> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Notes).HasMaxLength(1000);
        builder.HasOne(s => s.ChangedBy).WithMany()
            .HasForeignKey(s => s.ChangedById).OnDelete(DeleteBehavior.Restrict);
    }
}
```

`backend/src/V4H.Infrastructure/Persistence/Configurations/OpinionConfiguration.cs`:
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using V4H.Domain.Entities;

namespace V4H.Infrastructure.Persistence.Configurations;

public class OpinionConfiguration : IEntityTypeConfiguration<Opinion>
{
    public void Configure(EntityTypeBuilder<Opinion> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Content).HasMaxLength(10000).IsRequired();
        builder.HasOne(o => o.Specialist).WithMany()
            .HasForeignKey(o => o.SpecialistId).OnDelete(DeleteBehavior.Restrict);
    }
}
```

- [ ] **Step 3: Create repositories**

`backend/src/V4H.Infrastructure/Persistence/Repositories/UserRepository.cs`:
```csharp
using Microsoft.EntityFrameworkCore;
using V4H.Domain.Entities;
using V4H.Domain.Interfaces.Repositories;

namespace V4H.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) => _db = db;

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Users.FindAsync([id], ct).AsTask();

    public async Task AddAsync(User user, CancellationToken ct = default)
        => await _db.Users.AddAsync(user, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
```

`backend/src/V4H.Infrastructure/Persistence/Repositories/TeleconsultoriaRepository.cs`:
```csharp
using Microsoft.EntityFrameworkCore;
using V4H.Domain.Entities;
using V4H.Domain.Enums;
using V4H.Domain.Interfaces.Repositories;

namespace V4H.Infrastructure.Persistence.Repositories;

public class TeleconsultoriaRepository : ITeleconsultoriaRepository
{
    private readonly AppDbContext _db;

    public TeleconsultoriaRepository(AppDbContext db) => _db = db;

    public async Task<List<Teleconsultoria>> ListAsync(
        Specialty? specialty, string? patient, TeleconsultoriaStatus? status,
        DateTimeOffset? dateFrom, DateTimeOffset? dateTo, CancellationToken ct = default)
    {
        var query = _db.Teleconsultorias.Include(t => t.Requester).AsQueryable();

        if (specialty.HasValue) query = query.Where(t => t.Specialty == specialty.Value);
        if (!string.IsNullOrWhiteSpace(patient))
            query = query.Where(t => t.PatientName.Contains(patient));
        if (status.HasValue) query = query.Where(t => t.Status == status.Value);
        if (dateFrom.HasValue) query = query.Where(t => t.CreatedAt >= dateFrom.Value);
        if (dateTo.HasValue) query = query.Where(t => t.CreatedAt <= dateTo.Value);

        return await query.OrderByDescending(t => t.CreatedAt).ToListAsync(ct);
    }

    public Task<Teleconsultoria?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
        => _db.Teleconsultorias
            .Include(t => t.Requester)
            .Include(t => t.Documents)
            .Include(t => t.Opinions).ThenInclude(o => o.Specialist)
            .Include(t => t.StatusHistories)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task AddAsync(Teleconsultoria tc, CancellationToken ct = default)
        => await _db.Teleconsultorias.AddAsync(tc, ct);

    public async Task AddDocumentAsync(TeleconsultoriaDocument doc, CancellationToken ct = default)
        => await _db.Documents.AddAsync(doc, ct);

    public async Task AddStatusHistoryAsync(StatusHistory history, CancellationToken ct = default)
        => await _db.StatusHistories.AddAsync(history, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
```

`backend/src/V4H.Infrastructure/Persistence/Repositories/OpinionRepository.cs`:
```csharp
using V4H.Domain.Entities;
using V4H.Domain.Interfaces.Repositories;

namespace V4H.Infrastructure.Persistence.Repositories;

public class OpinionRepository : IOpinionRepository
{
    private readonly AppDbContext _db;

    public OpinionRepository(AppDbContext db) => _db = db;

    public async Task AddAsync(Opinion opinion, CancellationToken ct = default)
        => await _db.Opinions.AddAsync(opinion, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
```

- [ ] **Step 4: Build to verify**

```bash
cd backend
dotnet build src/V4H.Infrastructure
```
Expected: Build succeeded, 0 errors.

- [ ] **Step 5: Commit**

```bash
git add backend/src/V4H.Infrastructure/Persistence/
git commit -m "feat(infrastructure): EF Core AppDbContext, configurations, repositories"
```

---

### Task 6: Infrastructure — Services

**Files:**
- Create: `backend/src/V4H.Infrastructure/Services/BcryptPasswordHasher.cs`
- Create: `backend/src/V4H.Infrastructure/Services/JwtService.cs`
- Create: `backend/src/V4H.Infrastructure/Services/MockDocumentValidationService.cs`
- Create: `backend/src/V4H.Infrastructure/Services/LocalFileStorageService.cs`
- Create: `backend/src/V4H.Infrastructure/Services/QuestPdfExportService.cs`
- Create: `backend/src/V4H.Infrastructure/Services/SignalRNotificationService.cs`
- Create: `backend/src/V4H.Infrastructure/Hubs/NotificationHub.cs`
- Create: `backend/src/V4H.Infrastructure/DependencyInjection.cs`

- [ ] **Step 1: Create password hasher and JWT service**

`backend/src/V4H.Infrastructure/Services/BcryptPasswordHasher.cs`:
```csharp
using V4H.Application.Common.Interfaces;

namespace V4H.Infrastructure.Services;

public class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, 12);

    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}
```

`backend/src/V4H.Infrastructure/Services/JwtService.cs`:
```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using V4H.Application.Common.Interfaces;

namespace V4H.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly string _secret;

    public JwtService(IConfiguration config)
    {
        _secret = config["JWT:Secret"] ?? throw new InvalidOperationException("JWT:Secret not configured.");
    }

    public string Generate(Guid userId, string email, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

Note: Add to `V4H.Infrastructure.csproj`:
```xml
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.0.2" />
<PackageReference Include="Microsoft.IdentityModel.Tokens" Version="8.0.2" />
```

- [ ] **Step 2: Create mock AI validation service**

`backend/src/V4H.Infrastructure/Services/MockDocumentValidationService.cs`:
```csharp
using Microsoft.Extensions.Configuration;
using V4H.Application.Common.Interfaces;

namespace V4H.Infrastructure.Services;

public class MockDocumentValidationService : IDocumentValidationService
{
    private readonly decimal? _fixedScore;

    public MockDocumentValidationService(IConfiguration config)
    {
        var val = config["AI:MockScore"];
        _fixedScore = val is not null ? decimal.Parse(val) : null;
    }

    public Task<ValidationResult> ValidateAsync(Stream file, string mimeType, CancellationToken ct = default)
    {
        var score = _fixedScore ?? mimeType switch
        {
            "application/pdf" => 0.92m,
            "image/jpeg" or "image/png" => 0.78m,
            _ => 0.25m
        };

        return Task.FromResult(new ValidationResult(score, "MockValidator/1.0", DateTimeOffset.UtcNow));
    }
}
```

- [ ] **Step 3: Create file storage service**

`backend/src/V4H.Infrastructure/Services/LocalFileStorageService.cs`:
```csharp
using V4H.Application.Common.Interfaces;

namespace V4H.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public LocalFileStorageService(IConfiguration config)
    {
        _basePath = config["Storage:BasePath"] ?? "uploads";
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> SaveAsync(Stream content, string fileName, CancellationToken ct = default)
    {
        var unique = $"{Guid.NewGuid()}_{fileName}";
        var path = Path.Combine(_basePath, unique);
        await using var fs = File.Create(path);
        await content.CopyToAsync(fs, ct);
        return path;
    }
}
```

Add `using Microsoft.Extensions.Configuration;` — already present via implicit usings if added.

- [ ] **Step 4: Create QuestPDF export service**

`backend/src/V4H.Infrastructure/Services/QuestPdfExportService.cs`:
```csharp
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using V4H.Application.Common.Interfaces;
using V4H.Domain.Entities;

namespace V4H.Infrastructure.Services;

public class QuestPdfExportService : IPdfExportService
{
    public QuestPdfExportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] Export(Teleconsultoria tc)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Text($"Teleconsultoria — {tc.PatientName}")
                    .SemiBold().FontSize(16).FontColor(Colors.Blue.Medium);

                page.Content().Column(col =>
                {
                    col.Item().Text($"Especialidade: {tc.Specialty}");
                    col.Item().Text($"Status: {tc.Status}");
                    col.Item().Text($"Data de Nascimento: {tc.BirthDate:dd/MM/yyyy}");
                    col.Item().Text($"Hipótese Diagnóstica: {tc.DiagnosticHypothesis}");
                    col.Item().Text($"Histórico Clínico: {tc.ClinicalHistory}");

                    if (tc.Opinions.Any())
                    {
                        col.Item().PaddingTop(10).Text("Pareceres:").SemiBold();
                        foreach (var op in tc.Opinions)
                        {
                            col.Item().Text($"• {op.Specialist?.Name ?? "Especialista"} ({op.CreatedAt:dd/MM/yyyy HH:mm}): {op.Content}");
                        }
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("V4H ReNTAI — Gerado em ");
                    x.Span(DateTimeOffset.UtcNow.ToString("dd/MM/yyyy HH:mm"));
                });
            });
        }).GeneratePdf();
    }
}
```

- [ ] **Step 5: Create SignalR hub and notification service**

`backend/src/V4H.Infrastructure/Hubs/NotificationHub.cs`:
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace V4H.Infrastructure.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (userId is not null)
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);

        await base.OnConnectedAsync();
    }
}
```

`backend/src/V4H.Infrastructure/Services/SignalRNotificationService.cs`:
```csharp
using Microsoft.AspNetCore.SignalR;
using V4H.Application.Common.Interfaces;
using V4H.Infrastructure.Hubs;

namespace V4H.Infrastructure.Services;

public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hub;

    public SignalRNotificationService(IHubContext<NotificationHub> hub) => _hub = hub;

    public Task SendNewOpinionAsync(Guid requesterId, Guid teleconsultoriaId, Guid opinionId, CancellationToken ct = default)
        => _hub.Clients.Group(requesterId.ToString())
            .SendAsync("NewOpinion", new { teleconsultoriaId, opinionId }, ct);
}
```

- [ ] **Step 6: Create DependencyInjection extension**

`backend/src/V4H.Infrastructure/DependencyInjection.cs`:
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using V4H.Application.Common.Interfaces;
using V4H.Domain.Interfaces.Repositories;
using V4H.Infrastructure.Hubs;
using V4H.Infrastructure.Persistence;
using V4H.Infrastructure.Persistence.Repositories;
using V4H.Infrastructure.Services;

namespace V4H.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("Default")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITeleconsultoriaRepository, TeleconsultoriaRepository>();
        services.AddScoped<IOpinionRepository, OpinionRepository>();

        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IDocumentValidationService, MockDocumentValidationService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<IPdfExportService, QuestPdfExportService>();
        services.AddScoped<INotificationService, SignalRNotificationService>();

        services.AddSignalR();

        return services;
    }
}
```

- [ ] **Step 7: Build to verify**

```bash
cd backend
dotnet build src/V4H.Infrastructure
```
Expected: Build succeeded, 0 errors.

- [ ] **Step 8: Commit**

```bash
git add backend/src/V4H.Infrastructure/
git commit -m "feat(infrastructure): services, SignalR hub, DI registration"
```

---

### Task 7: API — Program.cs + Middleware + Controllers

**Files:**
- Modify: `backend/src/V4H.API/Program.cs`
- Create: `backend/src/V4H.API/Middleware/ExceptionHandlingMiddleware.cs`
- Create: `backend/src/V4H.API/Controllers/AuthController.cs`
- Create: `backend/src/V4H.API/Controllers/TeleconsultoriasController.cs`
- Create: `backend/src/V4H.API/Dockerfile`

- [ ] **Step 1: Create exception handling middleware**

`backend/src/V4H.API/Middleware/ExceptionHandlingMiddleware.cs`:
```csharp
using System.Text.Json;
using V4H.Application.Common.Exceptions;

namespace V4H.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, extra) = exception switch
        {
            NotFoundException nfe => (StatusCodes.Status404NotFound, nfe.Message, (object?)null),
            UnauthorizedException ue => (StatusCodes.Status403Forbidden, ue.Message, null),
            InvalidOperationException ioe => (StatusCodes.Status409Conflict, ioe.Message, null),
            DocumentValidationException dve => (StatusCodes.Status422UnprocessableEntity, dve.Message,
                new { score = dve.Score }),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.", null)
        };

        context.Response.StatusCode = statusCode;

        var body = extra is not null
            ? JsonSerializer.Serialize(new { error = message, extra })
            : JsonSerializer.Serialize(new { error = message });

        return context.Response.WriteAsync(body);
    }
}
```

- [ ] **Step 2: Write Program.cs**

`backend/src/V4H.API/Program.cs`:
```csharp
using System.Security.Claims;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using V4H.API.Middleware;
using V4H.Infrastructure;
using V4H.Infrastructure.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(V4H.Application.Auth.Commands.RegisterCommand).Assembly);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "V4H ReNTAI API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference {
                Type = ReferenceType.SecurityScheme, Id = "Bearer" }},
            Array.Empty<string>()
        }
    });
});

var jwtSecret = builder.Configuration["JWT:Secret"]
    ?? throw new InvalidOperationException("JWT:Secret not configured.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role
        };
        opts.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                var accessToken = ctx.Request.Query["access_token"];
                var path = ctx.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    ctx.Token = accessToken;
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(opts =>
{
    opts.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader().AllowAnyMethod().AllowCredentials());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

// Auto-migrate on startup (dev convenience)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<V4H.Infrastructure.Persistence.AppDbContext>();
    db.Database.Migrate();
}

app.Run();

public partial class Program { }
```

- [ ] **Step 3: Create AuthController**

`backend/src/V4H.API/Controllers/AuthController.cs`:
```csharp
using MediatR;
using Microsoft.AspNetCore.Mvc;
using V4H.Application.Auth.Commands;
using V4H.Domain.Enums;

namespace V4H.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var id = await _mediator.Send(new RegisterCommand(req.Name, req.Email, req.Password, req.Role));
        return CreatedAtAction(nameof(Register), new { userId = id });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var result = await _mediator.Send(new LoginCommand(req.Email, req.Password));
        return Ok(result);
    }
}

public record RegisterRequest(string Name, string Email, string Password, UserRole Role);
public record LoginRequest(string Email, string Password);
```

- [ ] **Step 4: Create TeleconsultoriasController**

`backend/src/V4H.API/Controllers/TeleconsultoriasController.cs`:
```csharp
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using V4H.Application.Teleconsultorias.Commands;
using V4H.Application.Teleconsultorias.Queries;
using V4H.Domain.Enums;

namespace V4H.API.Controllers;

[ApiController]
[Route("api/teleconsultorias")]
[Authorize]
public class TeleconsultoriasController : ControllerBase
{
    private readonly IMediator _mediator;

    public TeleconsultoriasController(IMediator mediator) => _mediator = mediator;

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new InvalidOperationException("User ID not in token."));

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] Specialty? specialty,
        [FromQuery] string? patient,
        [FromQuery] TeleconsultoriaStatus? status,
        [FromQuery] DateTimeOffset? dateFrom,
        [FromQuery] DateTimeOffset? dateTo)
    {
        var result = await _mediator.Send(
            new ListTeleconsultoriasQuery(specialty, patient, status, dateFrom, dateTo));
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Solicitante")]
    public async Task<IActionResult> Create([FromBody] CreateTeleconsultoriaRequest req)
    {
        var id = await _mediator.Send(new CreateTeleconsultoriaCommand(
            req.PatientName, req.BirthDate, req.Specialty,
            req.DiagnosticHypothesis, req.ClinicalHistory, CurrentUserId));
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetTeleconsultoriaDetailQuery(id));
        return Ok(result);
    }

    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = "Especialista")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest req)
    {
        await _mediator.Send(new UpdateStatusCommand(id, req.Status, CurrentUserId, req.Notes));
        return NoContent();
    }

    [HttpPost("{id:guid}/documents")]
    [Authorize(Roles = "Solicitante")]
    public async Task<IActionResult> UploadDocument(Guid id, IFormFile file)
    {
        var docId = await _mediator.Send(new UploadDocumentCommand(
            id, file.OpenReadStream(), file.FileName, file.ContentType, CurrentUserId));
        return CreatedAtAction(nameof(GetById), new { id }, new { id = docId });
    }

    [HttpPost("{id:guid}/opinions")]
    [Authorize(Roles = "Especialista")]
    public async Task<IActionResult> RegisterOpinion(Guid id, [FromBody] RegisterOpinionRequest req)
    {
        var opinionId = await _mediator.Send(
            new RegisterOpinionCommand(id, CurrentUserId, req.Content));
        return CreatedAtAction(nameof(GetById), new { id }, new { id = opinionId });
    }

    [HttpGet("{id:guid}/export/pdf")]
    public async Task<IActionResult> ExportPdf(Guid id)
    {
        var bytes = await _mediator.Send(new ExportPdfQuery(id));
        return File(bytes, "application/pdf", $"teleconsultoria-{id}.pdf");
    }
}

public record CreateTeleconsultoriaRequest(
    string PatientName,
    DateOnly BirthDate,
    Specialty Specialty,
    string DiagnosticHypothesis,
    string ClinicalHistory);

public record UpdateStatusRequest(TeleconsultoriaStatus Status, string? Notes);
public record RegisterOpinionRequest(string Content);
```

- [ ] **Step 5: Fix ClaimTypes.NameIdentifier — update CurrentUserId**

In the controller, update `CurrentUserId` to parse the `sub` claim correctly (JWT uses `sub` for `NameIdentifier`):
```csharp
private Guid CurrentUserId
{
    get
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue("sub");
        return Guid.Parse(sub ?? throw new InvalidOperationException("Missing user ID claim."));
    }
}
```

- [ ] **Step 6: Create Dockerfile**

`backend/src/V4H.API/Dockerfile`:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/V4H.API/V4H.API.csproj", "src/V4H.API/"]
COPY ["src/V4H.Application/V4H.Application.csproj", "src/V4H.Application/"]
COPY ["src/V4H.Infrastructure/V4H.Infrastructure.csproj", "src/V4H.Infrastructure/"]
COPY ["src/V4H.Domain/V4H.Domain.csproj", "src/V4H.Domain/"]
RUN dotnet restore "src/V4H.API/V4H.API.csproj"
COPY . .
WORKDIR "/src/src/V4H.API"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "V4H.API.dll"]
```

- [ ] **Step 7: Add EF migration**

```bash
cd backend
dotnet add src/V4H.Infrastructure/V4H.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Design --version 8.0.11
dotnet ef migrations add InitialCreate --project src/V4H.Infrastructure --startup-project src/V4H.API --output-dir Persistence/Migrations
```
Expected: Migration file created in `V4H.Infrastructure/Persistence/Migrations/`.

- [ ] **Step 8: Build all**

```bash
cd backend
dotnet build
```
Expected: Build succeeded, 0 errors.

- [ ] **Step 9: Commit**

```bash
git add backend/src/V4H.API/ backend/src/V4H.Infrastructure/Persistence/Migrations/
git commit -m "feat(api): Program.cs, middleware, controllers, Dockerfile, EF migration"
```

---

### Task 8: Integration Tests

**Files:**
- Create: `backend/tests/V4H.API.Tests/Integration/TeleconsultoriasEndpointTests.cs`
- Create: `backend/tests/V4H.API.Tests/CustomWebApplicationFactory.cs`

- [ ] **Step 1: Create CustomWebApplicationFactory**

`backend/tests/V4H.API.Tests/CustomWebApplicationFactory.cs`:
```csharp
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using V4H.Infrastructure.Persistence;

namespace V4H.API.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor is not null) services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(opts =>
                opts.UseInMemoryDatabase("TestDb_" + Guid.NewGuid()));

            services.AddSingleton(
                Microsoft.Extensions.Configuration.ConfigurationExtensions
                    .GetSection(
                        new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                            .AddInMemoryCollection(new Dictionary<string, string?> {
                                ["JWT:Secret"] = "test-secret-at-least-32-characters-long!!",
                                ["AI:ValidationThreshold"] = "0.6",
                                ["Storage:BasePath"] = Path.GetTempPath()
                            }).Build(), "").GetChildren()
                        .ToDictionary(c => c.Key, c => c.Value));
        });

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?> {
                ["JWT:Secret"] = "test-secret-at-least-32-characters-long!!",
                ["AI:ValidationThreshold"] = "0.6",
                ["Storage:BasePath"] = Path.GetTempPath()
            });
        });
    }
}
```

- [ ] **Step 2: Write integration test**

`backend/tests/V4H.API.Tests/Integration/TeleconsultoriasEndpointTests.cs`:
```csharp
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using V4H.Domain.Enums;
using V4H.Infrastructure.Persistence;
using Xunit;

namespace V4H.API.Tests.Integration;

public class TeleconsultoriasEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TeleconsultoriasEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> RegisterAndLoginAsync(string email, UserRole role)
    {
        await _client.PostAsJsonAsync("/api/auth/register", new
        {
            name = "Test User",
            email,
            password = "Pass123!",
            role = (int)role
        });

        var loginResp = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email,
            password = "Pass123!"
        });

        loginResp.EnsureSuccessStatusCode();
        var json = await loginResp.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("token").GetString()!;
    }

    [Fact]
    public async Task CreateTeleconsultoria_AsSolicitante_Returns201()
    {
        var token = await RegisterAndLoginAsync("sol@test.com", UserRole.Solicitante);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var resp = await _client.PostAsJsonAsync("/api/teleconsultorias", new
        {
            patientName = "Paciente Teste",
            birthDate = "1990-01-01",
            specialty = 1,
            diagnosticHypothesis = "Hipótese",
            clinicalHistory = "Histórico clínico detalhado"
        });

        Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
    }

    [Fact]
    public async Task CreateTeleconsultoria_AsEspecialista_Returns403()
    {
        var token = await RegisterAndLoginAsync("esp@test.com", UserRole.Especialista);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var resp = await _client.PostAsJsonAsync("/api/teleconsultorias", new
        {
            patientName = "P",
            birthDate = "1990-01-01",
            specialty = 1,
            diagnosticHypothesis = "H",
            clinicalHistory = "C"
        });

        Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
    }

    [Fact]
    public async Task ListTeleconsultorias_Authenticated_Returns200()
    {
        var token = await RegisterAndLoginAsync("list@test.com", UserRole.Especialista);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var resp = await _client.GetAsync("/api/teleconsultorias");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task GetTeleconsultoria_NotFound_Returns404()
    {
        var token = await RegisterAndLoginAsync("notfound@test.com", UserRole.Especialista);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var resp = await _client.GetAsync($"/api/teleconsultorias/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }
}
```

- [ ] **Step 3: Run integration tests**

```bash
cd backend
dotnet test tests/V4H.API.Tests -v minimal
```
Expected: All integration tests pass.

- [ ] **Step 4: Run all tests**

```bash
cd backend
dotnet test -v minimal
```
Expected: All tests pass.

- [ ] **Step 5: Commit**

```bash
git add backend/tests/V4H.API.Tests/
git commit -m "test(api): integration tests for auth and teleconsultorias endpoints"
```

---

### Task 9: Smoke Test with Docker Compose

- [ ] **Step 1: Copy .env.example to .env**

```bash
cd C:\Users\joaom\Dev\v4h-rentai
Copy-Item .env.example .env
```

Edit `.env` — set a real JWT secret (at least 32 chars):
```
JWT_SECRET=v4h-rentai-super-secret-jwt-key-2026
```

- [ ] **Step 2: Start services**

```bash
docker compose up --build -d
```
Expected: db and api containers start.

- [ ] **Step 3: Test endpoints**

```bash
# Register
curl -s -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"name":"Joao","email":"joao@test.com","password":"Pass123!","role":1}' | jq .

# Login
curl -s -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"joao@test.com","password":"Pass123!"}' | jq .token
```
Expected: 201 on register, token string on login.

- [ ] **Step 4: Check Swagger**

Open browser: `http://localhost:5000/swagger`
Expected: Swagger UI with all endpoints documented.

- [ ] **Step 5: Commit final**

```bash
git add .
git commit -m "chore: backend complete — all layers, tests, Docker"
```
