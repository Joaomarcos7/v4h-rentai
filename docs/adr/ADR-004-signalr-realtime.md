# ADR-004: SignalR para notificações em tempo real

**Data:** 2026-05-21  
**Status:** Aceito

## Contexto

Quando um Especialista registra um parecer, o Solicitante que criou a teleconsultoria deve ser notificado imediatamente, sem necessidade de recarregar a página.

## Decisão

Usar SignalR (ASP.NET Core) para notificações push:

- **Hub:** `NotificationHub` em `/hubs/notifications`
- **Grupos:** cada usuário entra no grupo identificado pelo seu `userId` ao conectar
- **Evento:** `NewOpinion` — enviado ao grupo do `requesterId` com `{ teleconsultoriaId, opinionId }`
- **Cliente:** `@microsoft/signalr` no Angular; `NotificationService` gerencia conexão e expõe `lastOpinionNotification` como Signal do Angular
- **Reconexão automática:** `withAutomaticReconnect()` no builder do cliente

## Consequências

**Positivo:**
- Nativo no ecossistema ASP.NET Core — sem broker externo (Redis, RabbitMQ)
- Fallback automático (WebSocket → Server-Sent Events → Long Polling)
- Angular Signals integram naturalmente com o padrão reativo do frontend

**Negativo:**
- Sem persistência de notificações perdidas (usuário desconectado não recupera eventos)
- Em produção com múltiplas instâncias, requereria Redis backplane

## Alternativas Consideradas

- **Polling:** simples de implementar, mas desperdiça banda e introduz latência
- **Server-Sent Events (SSE):** unidirecional; SignalR oferece bidirecionalidade e fallbacks
- **WebSockets puros:** requer implementação manual de reconexão e protocolo
