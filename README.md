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

**Declaração obrigatória conforme requisito do processo seletivo.**

Este projeto foi desenvolvido com assistência ativa do **Claude Code** (Anthropic, modelo `claude-sonnet-4-6`) como ferramenta de desenvolvimento ao longo de todo o ciclo — do brainstorming ao bugfix pós-entrega.

### Contexto passado à IA no início do projeto

Antes de qualquer linha de código, o contexto completo do projeto foi apresentado à IA:

- **Natureza do projeto:** desafio técnico de processo seletivo para o LAVID/UFPB. Não iria para produção. O foco era demonstrar domínio técnico e aderência aos requisitos propostos — não pensar em escalabilidade, resiliência ou manutenibilidade como se trataria em um sistema real com ciclo de vida longo.
- **Prazo:** curto (fim de semana). A escolha de stack precisava equilibrar familiaridade do desenvolvedor com robustez técnica para atender os requisitos.
- **Stack escolhida com justificativa:** .NET 8 + Angular 17. O desenvolvedor tem maior experiência com essas tecnologias, o que reduzia o risco de não entregar dentro do prazo. Além disso, ambas são stacks modernas, robustas e reconhecidamente escaláveis — atendendo ao requisito técnico mesmo que escalabilidade real não fosse o foco do desafio.
- **Requisitos declarados:** Clean Architecture, CQRS, TDD, ADRs, diagramas C4, SignalR, validação via IA (mock com interface real), exportação PDF, autenticação JWT.

Com esse contexto estabelecido, a colaboração com a IA foi estruturada para maximizar a qualidade dentro do tempo disponível — priorizando implementar todos os requisitos corretamente, com testes e documentação, em vez de otimizar para cenários que o desafio não exigia.

### Fluxo de trabalho com a IA

O desenvolvimento seguiu um fluxo estruturado via **Superpowers Plugin** para Claude Code, com skills especializadas em cada fase:

**1. Brainstorming (`superpowers:brainstorming`)**
Definição colaborativa de: modelo de dados, contratos de API, fluxos críticos (upload + IA, parecer + real-time), escolha de bibliotecas (QuestPDF, NSubstitute, MediatR), estratégia de mock para o serviço de IA.

**2. Planejamento (`superpowers:writing-plans`)**
Geração de três planos de implementação segregados — backend, frontend e documentação — cada um com tasks granulares, steps com código completo, comandos exatos e saída esperada. Nenhum step com "TBD" ou "implementar depois".

**3. Implementação (`superpowers:subagent-driven-development`)**
Execução task a task via subagentes isolados: cada task recebia um subagente fresco (sem contexto contaminado das anteriores), seguido de revisão de conformidade com o spec e depois revisão de qualidade de código antes de marcar como concluída.

**4. TDD em cada task (`superpowers:test-driven-development`)**
Todos os handlers de Application foram cobertos com testes unitários escritos antes da implementação (RED → GREEN → REFACTOR). Testes de integração via `WebApplicationFactory` cobrindo os endpoints críticos. Bugfixes descobertos pós-entrega seguiram o mesmo ciclo — teste que reproduz o bug primeiro, depois o fix.

**5. Bugfixes documentados**
Cada bug encontrado foi corrigido com TDD e documentado em [`docs/bugfixes/`](docs/bugfixes/):
- Download de PDF retornando 401 — `<a href>` bypassava o interceptor Angular
- Cadastro retornando 400 — `[value]` em select coagia número para string
- Atualização de status retornando 500 — mesma causa + dropdown não refletia status atual

### Como a IA foi utilizada por fase

| Fase | Uso |
|------|-----|
| **Brainstorming** | Levantamento de requisitos, escolha de stack, definição de arquitetura, modelo de dados, contratos de API |
| **Design Spec** | Documento completo com fluxos de dados, fluxo de auth, ADRs planejados, diagramas C4 |
| **Planejamento** | Três planos segregados com tasks, steps, código e comandos completos — zero placeholders |
| **Implementação** | Todos os arquivos de código gerados com revisão spec + qualidade após cada task |
| **Testes** | Testes unitários e integração escritos via TDD; bugfixes sempre com teste reproduzindo o bug primeiro |
| **Documentação** | ADRs, diagramas C4 em Mermaid, README, bug reports |

### Postura durante o uso

- Contexto do projeto passado explicitamente antes de qualquer decisão técnica
- Todas as decisões arquiteturais tomadas em colaboração com explicação de trade-offs
- IA usada como par de programação sênior — não como caixa-preta
- Código revisado e validado em cada passo antes de avançar
- Processo seguiu spec-driven development, contract-first e TDD de forma rigorosa

### O que foi descartado durante o planejamento

Durante a fase de brainstorming, algumas alternativas foram consideradas e explicitamente descartadas:

- **Next.js no frontend:** cogitado por ser uma stack moderna e popular, mas descartado em favor do Angular 17. O desenvolvedor tem mais experiência com Angular, e dado o prazo apertado, a curva de aprendizado de Next.js representava risco real de não entrega. Angular standalone components + signals atendeu todos os requisitos sem overhead.
- **Integração de IA real:** o spec original mencionava validação de documentos via IA. A decisão foi implementar um mock robusto com interface real (`IDocumentValidationService`), substituível via DI sem refatoração. Uma integração real (ex.: OpenAI, AWS Textract) adicionaria complexidade e dependência externa que não agregaria valor avaliativo no contexto de um desafio técnico.

Esses descartes foram documentados nos ADRs e no design spec, com justificativa explícita dos trade-offs.

### Avaliação do que funcionou e do que não funcionou

#### O que funcionou bem

**Separar o desenvolvimento em etapas de refinamento progressivo foi o principal fator de sucesso.** O fluxo seguiu uma sequência deliberada e sem pular etapas:

1. Contextualizar a IA com a natureza do projeto, critérios de aceite, prazo e perfil do desenvolvedor
2. Refinar a arquitetura necessária para lidar com os requisitos (Clean Architecture, CQRS, SignalR, etc.)
3. Escolher a stack com base na familiaridade do desenvolvedor e nos requisitos técnicos
4. Definir planos e tasks dimensionados para o prazo disponível
5. Somente então partir para a implementação

Essa sequência foi determinante para a assertividade do modelo. Ao chegar na fase de implementação com contexto bem definido, arquitetura decidida e plano detalhado, o modelo conseguiu cumprir a maior parte da implementação de forma completa, robusta e testada — sem alucinar APIs inexistentes, sem contradizer o spec e sem over-engineering além do que os requisitos pediam.

#### O que não funcionou inicialmente

No início do projeto, tentamos segregar os planos em poucos blocos grandes. O resultado foi um contexto muito denso por task, o que dificultava a interpretação do modelo e aumentava o risco de desvio do spec.

A partir do momento que dividimos em **três planos independentes** (backend, frontend, documentação) e cada plano em **tasks granulares com subtasks** (steps de 2-5 minutos cada), a execução ficou visivelmente mais rápida, assertiva e fácil de revisar. Tasks menores = menos ambiguidade = menos chance de erro.

**Lição:** granularidade do plano impacta diretamente a qualidade da implementação. Resistir à tentação de agrupar demais.

### Bugfixes pós-entrega — testes funcionais E2E

Após a implementação inicial, foram realizados testes funcionais end-to-end navegando pela aplicação real. Três bugs foram encontrados e corrigidos seguindo TDD (teste reproduzindo o bug primeiro, fix depois):

| Bug | Causa | Documento |
|-----|-------|-----------|
| Download de PDF retornando 401 | `<a [href]>` não passa pelo `HttpClient`, bypassando o `AuthInterceptor` Angular | [ver](docs/bugfixes/2026-05-22-pdf-download-401.md) |
| Cadastro retornando 400 | `[value]="1"` em `<select>` coage número para string; backend rejeita string em enum | [ver](docs/bugfixes/2026-05-23-register-role-string-400.md) |
| Atualização de status retornando 500 | Mesma causa do cadastro + dropdown não inicializava com status atual da teleconsultoria | [ver](docs/bugfixes/2026-05-23-update-status-string-500.md) |

O comportamento adotado em cada bug foi: reproduzir com teste automatizado (RED), corrigir (GREEN), e documentar em `docs/bugfixes/` com causa raiz, solução e lição aprendida. Isso garante histórico real e rastreável do que foi feito e por que — não apenas o código final, mas o caminho percorrido.

### Ferramentas utilizadas

- **Claude Code** (`claude-sonnet-4-6`) — ferramenta principal
- **Superpowers Plugin** — skills: `brainstorming`, `writing-plans`, `subagent-driven-development`, `test-driven-development`, `finishing-a-development-branch`

## Limitações Conhecidas

- Arquivos salvos em disco local — em produção usaria S3/Azure Blob
- JWT sem refresh token — sessão expira em 8h
- Mock AI sem análise real de conteúdo — substituível via DI sem refatoração
- Sem rate limiting — necessário em produção
- SignalR sem Redis backplane — requerido para múltiplas instâncias em produção

## Licença

Projeto desenvolvido para fins avaliativos — LAVID/UFPB ReNTAI, 2026.
