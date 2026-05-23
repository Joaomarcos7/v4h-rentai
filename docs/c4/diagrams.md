# C4 Diagrams — V4H ReNTAI Teleconsultoria

Diagramas usando notação C4 com sintaxe Mermaid (renderizável no GitHub).

---

## Nível 1 — Contexto do Sistema

```mermaid
C4Context
    title Sistema de Teleconsultoria — Contexto

    Person(solicitante, "Solicitante (APS)", "Profissional da Atenção Primária à Saúde que cria solicitações de teleconsultoria")
    Person(especialista, "Especialista", "Médico especialista que analisa casos e emite pareceres")

    System(v4h, "V4H ReNTAI", "Sistema web de gerenciamento do ciclo completo de teleconsultorias, incluindo validação de documentos via IA e notificações em tempo real")

    System_Ext(ai, "Serviço de Validação IA", "Valida documentos clínicos enviados (mock configurável, substituível via DI)")
    System_Ext(storage, "Armazenamento de Arquivos", "Armazena documentos enviados (local em dev, S3/Azure Blob em produção)")

    Rel(solicitante, v4h, "Cria teleconsultorias, faz upload de documentos", "HTTPS")
    Rel(especialista, v4h, "Visualiza casos, registra pareceres, atualiza status", "HTTPS")
    Rel(v4h, ai, "Valida documentos clínicos", "In-process (mock)")
    Rel(v4h, storage, "Persiste arquivos de documentos", "Filesystem / SDK")
```

---

## Nível 2 — Containers

```mermaid
C4Container
    title Sistema de Teleconsultoria — Containers

    Person(solicitante, "Solicitante (APS)")
    Person(especialista, "Especialista")

    System_Boundary(v4h, "V4H ReNTAI") {
        Container(frontend, "Frontend Angular", "Angular 17, TypeScript", "SPA — interface para gerenciamento de teleconsultorias, auth, upload de documentos e visualização de pareceres")

        Container(api, "Backend API", ".NET 8, ASP.NET Core", "REST API com Clean Architecture + CQRS. Processa comandos e queries, valida documentos, exporta PDFs")

        Container(hub, "SignalR Hub", "ASP.NET Core SignalR", "Endpoint WebSocket em /hubs/notifications. Envia evento NewOpinion ao grupo do solicitante quando parecer é registrado")

        ContainerDb(db, "PostgreSQL 16", "PostgreSQL", "Persiste usuários, teleconsultorias, documentos, histórico de status e pareceres")
    }

    System_Ext(ai, "Mock AI Service", "In-process — MockDocumentValidationService. Substituível via DI por serviço real")
    System_Ext(storage, "Armazenamento Local", "Disco local em /uploads. Em produção: S3 / Azure Blob")

    Rel(solicitante, frontend, "Usa", "HTTPS / Browser")
    Rel(especialista, frontend, "Usa", "HTTPS / Browser")

    Rel(frontend, api, "Chama endpoints REST", "HTTP/JSON, JWT Bearer")
    Rel(frontend, hub, "Conecta para notificações", "WebSocket (JWT via query string)")

    Rel(api, db, "Lê e grava dados", "TCP / EF Core + Npgsql")
    Rel(api, ai, "Valida documentos", "In-process")
    Rel(api, storage, "Salva arquivos", "Filesystem")
    Rel(hub, frontend, "Envia evento NewOpinion", "WebSocket")
```

---

## Fluxo: Upload + Validação IA

```mermaid
sequenceDiagram
    actor S as Solicitante
    participant FE as Angular Frontend
    participant API as ASP.NET Core API
    participant AI as MockDocumentValidationService
    participant FS as LocalFileStorageService
    participant DB as PostgreSQL

    S->>FE: Seleciona arquivo e clica "Enviar"
    FE->>API: POST /api/teleconsultorias/{id}/documents (multipart)
    API->>AI: ValidateAsync(stream, mimeType)
    AI-->>API: ValidationResult { Score, Provider, Timestamp }
    alt Score < Threshold (0.6)
        API-->>FE: 422 { error, score }
        FE-->>S: "Documento rejeitado. Score: 0.25"
    else Score >= Threshold
        API->>FS: SaveAsync(stream, fileName)
        FS-->>API: storedPath
        API->>DB: INSERT TeleconsultoriaDocument
        API-->>FE: 201 { id }
        FE-->>S: "Documento enviado com sucesso!"
    end
```

---

## Fluxo: Parecer + Notificação em Tempo Real

```mermaid
sequenceDiagram
    actor E as Especialista
    actor Sol as Solicitante
    participant FE_E as Frontend (Especialista)
    participant FE_S as Frontend (Solicitante)
    participant API as ASP.NET Core API
    participant DB as PostgreSQL
    participant HUB as SignalR Hub

    E->>FE_E: Preenche parecer e clica "Registrar"
    FE_E->>API: POST /api/teleconsultorias/{id}/opinions
    API->>DB: INSERT Opinion
    API->>DB: UPDATE Teleconsultoria.Status = Concluida
    API->>DB: INSERT StatusHistory
    API->>HUB: SendAsync("NewOpinion", payload) ao grupo requesterId
    HUB-->>FE_S: Evento "NewOpinion" via WebSocket
    FE_S-->>Sol: Toast "Novo parecer recebido!" + atualiza lista
    API-->>FE_E: 201 { id }
    FE_E-->>E: "Parecer registrado!"
```
