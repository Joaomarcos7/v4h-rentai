# V4H ReNTAI — Módulo de Teleconsultoria

Sistema web para gerenciamento do ciclo completo de teleconsultorias no projeto ReNTAI/V4H da UFPB LAVID. Desenvolvido como desafio técnico para processo seletivo.

## Funcionalidades

- **Autenticação** — Registro e login com dois perfis: Solicitante (APS) e Especialista
- **Gestão de Teleconsultorias** — CRUD completo com filtros por especialidade, paciente, status e data
- **Upload de Documentos com IA** — Validação automática via `IDocumentValidationService` (mock configurável, substituível via DI)
- **Pareceres** — Especialistas registram pareceres clínicos; status atualizado automaticamente para Concluída
- **Notificações em Tempo Real** — SignalR notifica o Solicitante quando um parecer é registrado
- **Exportação PDF** — Download do relatório completo via QuestPDF

## Stack

| Camada | Tecnologia |
|--------|-----------|
| Backend | .NET 8, ASP.NET Core, EF Core 8 |
| Frontend | Angular 17 (standalone components) |
| Banco | PostgreSQL 16 |
| Real-time | SignalR |
| Auth | JWT HS256, BCrypt cost-12 |
| AI (Mock) | `IDocumentValidationService` — PDF 0.92, JPEG/PNG 0.78, outros 0.25 |
| PDF Export | QuestPDF |
| Testes | xUnit, NSubstitute, WebApplicationFactory |
| Container | Docker Compose |

## Arquitetura

Clean Architecture com 4 camadas no backend:

```
V4H.Domain → V4H.Application → V4H.Infrastructure → V4H.API
```

Ver [ADRs](docs/adr/) e [diagramas C4](docs/c4/diagrams.md) para decisões arquiteturais detalhadas.

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [Node.js 20+](https://nodejs.org/) e npm
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

## Rodando Localmente

### 1. Configurar variáveis de ambiente

```bash
cp .env.example .env
# Edite .env e defina JWT_SECRET com ao menos 32 caracteres
```

### 2. Subir banco de dados

```bash
docker compose up db -d
```

### 3. Rodar o backend

```bash
cd backend
dotnet run --project src/V4H.API
```

A API estará em `http://localhost:5000`. Swagger disponível em `http://localhost:5000/swagger`.

As migrations são aplicadas automaticamente na inicialização.

### 4. Rodar o frontend

```bash
cd frontend
npm install
npx ng serve
```

O frontend estará em `http://localhost:4200`.

### 5. Subir tudo com Docker Compose

```bash
docker compose up --build
```

## Rodando Testes

### Backend

```bash
cd backend
dotnet test
```

Testes unitários (handlers Application) e integração (endpoints API com banco in-memory).

## Variáveis de Ambiente

| Variável | Descrição | Padrão |
|----------|-----------|--------|
| `ConnectionStrings__Default` | Connection string PostgreSQL | — |
| `JWT__Secret` | Secret para assinar JWT (≥32 chars) | — |
| `AI__ValidationThreshold` | Score mínimo para aprovação de documento | `0.6` |
| `AI__MockScore` | Se definido, sobrepõe o score calculado por mimeType | — |
| `Storage__BasePath` | Diretório local para upload de arquivos | `uploads` |

## Endpoints da API

```
POST   /api/auth/register
POST   /api/auth/login

GET    /api/teleconsultorias                      (query: specialty, patient, status, dateFrom, dateTo)
POST   /api/teleconsultorias                      [Solicitante]
GET    /api/teleconsultorias/{id}
PUT    /api/teleconsultorias/{id}/status          [Especialista]
POST   /api/teleconsultorias/{id}/documents       [Solicitante]
POST   /api/teleconsultorias/{id}/opinions        [Especialista]
GET    /api/teleconsultorias/{id}/export/pdf

WS     /hubs/notifications                        (evento: NewOpinion)
```

## Documentação

- [ADR-001: Clean Architecture](docs/adr/ADR-001-clean-architecture.md)
- [ADR-002: CQRS via MediatR](docs/adr/ADR-002-cqrs-mediatr.md)
- [ADR-003: JWT Authentication](docs/adr/ADR-003-jwt-authentication.md)
- [ADR-004: SignalR Real-time](docs/adr/ADR-004-signalr-realtime.md)
- [ADR-005: Mock AI Service](docs/adr/ADR-005-mock-ai-service.md)
- [ADR-006: QuestPDF](docs/adr/ADR-006-questpdf.md)
- [Diagramas C4 (Mermaid)](docs/c4/diagrams.md)
- [Design Spec](docs/superpowers/specs/2026-05-21-teleconsultoria-design.md)

## Uso de Ferramentas de IA

**Declaração obrigatória conforme requisito do processo seletivo:**

Este projeto foi desenvolvido com assistência ativa do **Claude Code** (Anthropic, modelo `claude-sonnet-4-6`) como ferramenta de desenvolvimento.

### Como a IA foi utilizada

| Fase | Uso |
|------|-----|
| **Brainstorming** | Levantamento de requisitos, escolha de stack (.NET 8 + Angular 17), definição de arquitetura (Clean Architecture + CQRS), decisão sobre mock AI service |
| **Design** | Produção do design spec com modelo de dados, contratos de API, fluxos de dados, fluxo de auth |
| **Planejamento** | Geração dos planos de implementação segregados (backend, frontend, docs) com tasks, steps e código completo |
| **Implementação** | Scaffolding de todos os arquivos de código (entidades, handlers, repositórios, serviços, controllers, componentes Angular) com verificação passo a passo |
| **Testes** | Escrita de testes unitários (NSubstitute) e integração (WebApplicationFactory), identificação de casos de borda (score abaixo do threshold, requester errado) |
| **Documentação** | Redação dos ADRs, diagramas C4 em Mermaid, README |

### Postura durante o uso

- Todas as decisões arquiteturais foram tomadas em colaboração, com explicação dos trade-offs
- O código gerado foi revisado e validado em cada passo antes de avançar
- A IA foi usada como par de programação sênior, não como caixa-preta
- O processo seguiu práticas de spec-driven development, contract-first e TDD

### Ferramentas utilizadas

- **Claude Code** (claude.ai/code) — ferramenta principal de desenvolvimento
- **Superpowers Plugin** — skills de brainstorming, writing-plans, subagent-driven-development para estruturar o fluxo de trabalho com IA

## Limitações Conhecidas

- Arquivos salvos em disco local — em produção usaria S3/Azure Blob
- JWT sem refresh token — sessão expira em 8h
- Mock AI sem análise real de conteúdo — substituível via DI sem refatoração
- Sem rate limiting — necessário em produção
- SignalR sem Redis backplane — requerido para múltiplas instâncias em produção

## Licença

Projeto desenvolvido para fins avaliativos — LAVID/UFPB ReNTAI, 2026.
