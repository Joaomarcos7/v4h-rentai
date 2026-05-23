# Feature: Badge de notificação no Dashboard para novo parecer

**Data:** 2026-05-23  
**Tipo:** Feature  
**Status:** Implementado

---

## Comportamento

Quando o Solicitante está navegando no Dashboard e um especialista registra novo parecer:

1. **Banner verde** aparece no topo da página com link direto para a teleconsultoria: "🔔 Novo parecer recebido! Ver detalhes →"
2. **Badge "Novo parecer"** aparece na linha da tabela correspondente
3. Ambos são fecháveis via ✕ / `dismissDashboardAlert()`

## Implementação

`pendingOpinionTcId = signal<string | null>(null)` rastreia qual teleconsultoria recebeu a notificação.

```typescript
effect(() => {
  const notification = notifications.lastOpinionNotification();
  if (notification) {
    this.pendingOpinionTcId.set(notification.teleconsultoriaId);
    this.showToast('Novo parecer recebido!');
    this.loadList();
  }
}, { allowSignalWrites: true }); // allowSignalWrites necessário (NG0600)
```

Template renderiza banner com `[routerLink]` para a teleconsultoria específica e badge na linha da tabela.

## Fix colateral

O `effect()` pré-existente no Dashboard não tinha `allowSignalWrites: true`, causando NG0600 silencioso. Corrigido junto com esta feature.
