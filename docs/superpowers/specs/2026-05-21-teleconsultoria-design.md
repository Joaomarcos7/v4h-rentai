# Design: Módulo de Teleconsultoria — V4H ReNTAI

**Data:** 2026-05-21  
**Status:** Aprovado

---

## 1. Contexto

Módulo web para gerenciamento do ciclo completo de teleconsultorias no projeto ReNTAI/V4H. Profissionais da APS (Solicitantes) criam solicitações com documentação clínica; Especialistas respondem com pareceres. O sistema valida documentos via IA e notifica em tempo real.

---

## 2. Stack

| Camada | Tecnologia |
|--------|-----------|
| Backend | .NET 8, ASP.NET Core, EF Core 8 |
| Frontend | Angular 17 (standalone components) |
| Banco | PostgreSQL 16 |
| Real-time | SignalR |
| Auth | JWT (Bearer, exp 8h) |
| AI Validation | Mock com interface `IDocumentValidationService` |
| PDF Export | QuestPDF |
| Container | Docker Compose |
| Testes (backend) | xUnit + NSubstitute + WebApplicationFactory |

---

## 3. Arquitetura

### 3.1 Backend — Clean Architecture

```
V4H.Domain          → Entidades, enums, interfaces de domínio, domain events
V4H.Application     → Use cases (CQRS/MediatR), DTOs, ports (interfaces)
V4H.Infrastructure  → EF Core, PostgreSQL, SignalR hub, AI mock, file storage
V4H.API             → Controllers, SignalR hub endpoint, middleware, Swagger
```

**Dependência:** Domain ← Application ← Infrastructure → API. API depende de Application e Infrastructure (via DI).

### 3.2 Frontend — Angular 17

```
src/app/
├── auth/           → login, register (guards: AuthGuard, RoleGuard)
├── dashboard/      → lista, filtros, busca
├── teleconsultoria/ → form, detalhes, timeline, pareceres
└── shared/         → interceptors, services, models tipados do OpenAPI
```

---

## 4. Modelo de Dados

### Entidades

**User**
- `Id` (Guid, PK)
- `Name` (string)
- `Email` (string, unique)
- `PasswordHash` (string)
- `Role` (enum: Solicitante | Especialista)
- `CreatedAt` (DateTimeOffset)

**Teleconsultoria**
- `Id` (Guid, PK)
- `PatientName` (string)
- `BirthDate` (DateOnly)
- `Specialty` (enum: Cardiologia | CirurgiaRobotica | Odontologia | DoencasRaras | Oxigenoterapia)
- `DiagnosticHypothesis` (string)
- `ClinicalHistory` (string)
- `Status` (enum: Pendente | EmAndamento | Concluida | Cancelada)
- `RequesterId` (Guid, FK → User)
- `CreatedAt`, `UpdatedAt` (DateTimeOffset)

**Document**
- `Id` (Guid, PK)
- `TeleconsultoriaId` (Guid, FK)
- `FileName` (string)
- `StoredPath` (string)
- `ValidationScore` (decimal)
- `ValidationProvider` (string)
- `ValidationThreshold` (decimal)
- `ValidatedAt` (DateTimeOffset)
- `IsApproved` (bool)

**StatusHistory**
- `Id` (Guid, PK)
- `TeleconsultoriaId` (Guid, FK)
- `OldStatus` (enum)
- `NewStatus` (enum)
- `ChangedAt` (DateTimeOffset)
- `ChangedById` (Guid, FK → User)
- `Notes` (string?)

**Opinion**
- `Id` (Guid, PK)
- `TeleconsultoriaId` (Guid, FK)
- `SpecialistId` (Guid, FK → User)
- `Content` (string)
- `CreatedAt` (DateTimeOffset)

---

## 5. API Contracts (OpenAPI)

```
POST   /api/auth/register                         → 201 {userId}
POST   /api/auth/login                            → 200 {token, user}

GET    /api/teleconsultorias                      → 200 [{...}] (query: specialty, patient, status, dateFrom, dateTo)
POST   /api/teleconsultorias                      → 201 {id}     [Solicitante]
GET    /api/teleconsultorias/{id}                 → 200 {...}
PUT    /api/teleconsultorias/{id}/status          → 204           [Especialista]
POST   /api/teleconsultorias/{id}/documents       → 201 | 422 {score, message}  [Solicitante]
POST   /api/teleconsultorias/{id}/opinions        → 201 {id}     [Especialista]
GET    /api/teleconsultorias/{id}/export/pdf      → 200 application/pdf
```

**SignalR Hub:** `/hubs/notifications`  
Evento: `NewOpinion` → enviado ao grupo do `requesterId` quando parecer registrado.

---

## 6. Fluxos Críticos

### 6.1 Upload + Validação IA

1. Multipart POST recebe arquivo + campos do form
2. `UploadDocumentCommandHandler` chama `IDocumentValidationService.ValidateAsync(stream, mimeType)`
3. Mock retorna `{Score, Provider, Timestamp}`
4. `Score < AI__ValidationThreshold` → lança `DocumentValidationException` → 422 com score
5. Score OK → persiste `Document` com todos os campos de validação → 201

### 6.2 Parecer + Real-time

1. `RegisterOpinionCommandHandler` persiste `Opinion`
2. Atualiza `Teleconsultoria.Status → Concluida`
3. Insere `StatusHistory`
4. Chama `IHubContext<NotificationHub>.Clients.Group(requesterId).SendAsync("NewOpinion", payload)`
5. Angular recebe evento → toast + atualiza status sem reload

---

## 7. Auth

- Senha: BCrypt (cost factor 12)
- JWT: assimétrico HS256, secret via `JWT__Secret` env var, exp 8h
- `[Authorize]` + `[Authorize(Roles="Solicitante")]` nos endpoints relevantes
- Angular: `AuthInterceptor` injeta Bearer token; `RoleGuard` protege rotas

---

## 8. Serviço de IA (Mock)

```csharp
public interface IDocumentValidationService
{
    Task<ValidationResult> ValidateAsync(Stream file, string mimeType, CancellationToken ct = default);
}

public record ValidationResult(decimal Score, string Provider, DateTimeOffset Timestamp);
```

Mock: retorna score configurável via `AI__MockScore` ou baseado em mimeType (PDF → 0.9, imagem → 0.75, outros → 0.3).  
Threshold configurável: `AI__ValidationThreshold` (default: 0.6).  
Substituível via DI — trocar `MockDocumentValidationService` por implementação real sem alterar nada além do registro no DI.

---

## 9. Testes

- **Unit:** Application handlers testados isoladamente com NSubstitute mocks
- **Integration:** `WebApplicationFactory` com banco em memória (ou TestContainers PostgreSQL)
- **Angular:** Jasmine/Karma para AuthGuard, RoleGuard, interceptor
- Cobertura mínima: fluxos críticos (upload, validação IA, parecer, real-time)

---

## 10. ADRs Planejados

| # | Decisão |
|---|---------|
| ADR-001 | Clean Architecture como estrutura do backend |
| ADR-002 | CQRS via MediatR |
| ADR-003 | JWT para autenticação |
| ADR-004 | SignalR para notificações em tempo real |
| ADR-005 | Mock com interface real para serviço de IA |
| ADR-006 | QuestPDF para exportação PDF |

---

## 11. Limitações Conhecidas

- Arquivo salvo em disco local (não blob storage) — produção usaria S3/Azure Blob
- JWT sem refresh token — sessão expira em 8h
- Mock AI sem análise real de conteúdo — substituível via DI sem refatoração
- Sem rate limiting ou auditoria de acesso — necessário em produção
- Testes de integração com banco em memória — TestContainers para fidelidade real
