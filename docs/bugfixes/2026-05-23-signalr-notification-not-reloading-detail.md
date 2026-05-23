# Bug Report: Solicitante não recebe notificação em tempo real na tela de detalhes

**Data:** 2026-05-23  
**Severidade:** Alta — funcionalidade de tempo real não funcionava na tela principal de uso  
**Status:** Corrigido

---

## Descrição

Ao registrar um parecer como Especialista, o Solicitante deveria receber uma notificação em tempo real via SignalR e ver a teleconsultoria atualizada sem recarregar a página. O comportamento observado era que nada acontecia na tela de detalhes — o parecer só aparecia após F5.

---

## Causa Raiz

O `NotificationService` recebia o evento `NewOpinion` via SignalR e atualizava o signal `lastOpinionNotification` corretamente. O problema era que **`DetailComponent` nunca escutava esse signal**.

O `effect()` que reagia à notificação existia apenas no `DashboardComponent` (listagem). Quando o Solicitante estava na tela de detalhes, o `DashboardComponent` não estava ativo, e o `DetailComponent` não tinha nenhuma lógica para reagir ao evento.

**Fluxo com o bug:**
```
SignalR → NotificationService.lastOpinionNotification.set(payload)
  → DashboardComponent.effect() reage (mas não está na tela)
  → DetailComponent: nada acontece
```

**Fluxo esperado:**
```
SignalR → NotificationService.lastOpinionNotification.set(payload)
  → DetailComponent.effect() verifica se payload.teleconsultoriaId === this.id
  → Se sim: chama this.load() → dados atualizados sem reload
```

---

## Solução

Injetado `NotificationService` no `DetailComponent` com `effect()` que filtra notificações pelo ID da teleconsultoria atual:

```typescript
constructor(
  private route: ActivatedRoute,
  private tcService: TeleconsultoriaService,
  public auth: AuthService,
  notifications: NotificationService
) {
  effect(() => {
    const n = notifications.lastOpinionNotification();
    this.onNotification(n);
  });
}

onNotification(payload: { teleconsultoriaId: string; opinionId: string } | null) {
  if (payload && payload.teleconsultoriaId === this.id) {
    this.load();
  }
}
```

O filtro por `teleconsultoriaId` garante que o componente não recarrega ao receber notificações de outras teleconsultorias.

---

## Testes

Três casos cobertos via `onNotification()` diretamente (effect testado pelo comportamento, não pelo mecanismo — Angular 17 não expõe `TestBed.flushEffects()`):

```typescript
it('should reload tc data when onNotification is called with matching teleconsultoria id')
it('should NOT reload when onNotification is called with a different teleconsultoria id')
it('should NOT reload when onNotification is called with null')
```

---

## Nota sobre testabilidade de `effect()` em Angular 17

`TestBed.flushEffects()` foi introduzido no Angular 18. No Angular 17, testar effects via signal diretamente é instável (requer `fakeAsync` + múltiplos `detectChanges` + `ApplicationRef.tick()` sem garantia de ordem). A solução adotada foi extrair a lógica de reação para `onNotification()` — método público e facilmente testável — e deixar o `effect()` apenas como ponte entre o signal e o método.
