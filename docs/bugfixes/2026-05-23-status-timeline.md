# Feature: Linha do tempo de status com histórico de atualizações

**Data:** 2026-05-23  
**Tipo:** Feature  
**Status:** Implementado

---

## O que foi implementado

Card "Histórico de Status" na tela de detalhes da teleconsultoria, exibindo cada transição de status em ordem cronológica com:
- Data/hora e responsável pela alteração
- Status anterior → novo status (badges coloridos)
- Notas opcionais (quando preenchidas no `updateStatus`)

## Arquitetura

O domínio já possuía `StatusHistory` com `OldStatus`, `NewStatus`, `ChangedAt`, `ChangedById`, `Notes`. O repository já incluía `StatusHistories` via EF Include. Faltava apenas expor esses dados na API.

### Backend

| Arquivo | Mudança |
|---------|---------|
| `StatusHistoryDto.cs` | Novo DTO com campos necessários |
| `TeleconsultoriaDetailDto.cs` | Adicionado `List<StatusHistoryDto> StatusHistories` |
| `GetTeleconsultoriaDetailQueryHandler.cs` | Mapeamento ordenado por `ChangedAt` |
| `TeleconsultoriaRepository.cs` | `ThenInclude(h => h.ChangedBy)` para resolver `ChangedByName` |

### Frontend

| Arquivo | Mudança |
|---------|---------|
| `teleconsultoria.model.ts` | `StatusHistoryItem` interface |
| `detail.component.html` | Card com timeline e badges de status |
| `styles.css` | `.timeline`, `.timeline-item`, `.timeline-dot`, `.timeline-content` |
