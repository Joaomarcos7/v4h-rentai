# Bug Report: NG0600 — signal write inside effect() bloqueia notificação em tempo real

**Data:** 2026-05-23  
**Severidade:** Alta — notificação em tempo real nunca recarrega a página (erro silencioso em produção)  
**Status:** Corrigido

---

## Descrição

Mesmo com NgZone e `connect()` corretos, a tela do solicitante não recarregava ao receber notificação de novo parecer. O erro `NG0600` era lançado silenciosamente no runtime, abortando o `effect()` antes que `load()` fosse chamado.

## Erro

```
RuntimeError: NG0600: Writing to signals is not allowed in a `computed` or an `effect` by default.
Use `allowSignalWrites` in the `CreateEffectOptions` to enable this inside effects.
    at _DetailComponent.load (detail.component.ts:54)
    at _DetailComponent.onNotification (detail.component.ts:39)
    at EffectHandle.effectFn (detail.component.ts:33)
```

## Causa Raiz

O `effect()` em `DetailComponent` chamava `onNotification()` → `load()` → `this.loading.set(true)` e `this.tc.set(data)`. Em Angular 17, escrever em signals dentro de `effect()` é proibido por padrão e lança NG0600.

```typescript
effect(() => {
  const n = notifications.lastOpinionNotification();
  this.onNotification(n); // → load() → signal.set() → NG0600
});
```

## Solução

```typescript
effect(() => {
  const n = notifications.lastOpinionNotification();
  this.onNotification(n);
}, { allowSignalWrites: true }); // ← habilita escrita de signals dentro do effect
```

## Teste

```typescript
it('should reload when notification signal fires with matching id (effect path)', fakeAsync(() => {
  const { fixture } = createAndLoad();
  const appRef = TestBed.inject(ApplicationRef);

  notificationService.handleNewOpinion({ teleconsultoriaId: TC_ID, opinionId: 'op-99' });
  tick();
  appRef.tick();
  fixture.detectChanges();

  const reloadReq = http.expectOne(`${environment.apiUrl}/teleconsultorias/${TC_ID}`);
  reloadReq.flush(mockTc);
}));
```

O teste reproduz o NG0600 via caminho real do `effect()` (signal → effect → load), não chamando `onNotification()` diretamente.

## Histórico completo das 4 causas raiz da notificação

| # | Problema | Fix |
|---|----------|-----|
| 1 | `DetailComponent` sem `effect()` para reagir ao sinal | Adicionado `effect()` |
| 2 | `connect()` nunca chamado após refresh | `AppComponent.ngOnInit()` |
| 3 | Signal atualizado fora da NgZone | `zone.run()` em `handleNewOpinion()` |
| 4 | `effect()` lançava NG0600 ao escrever signals | `allowSignalWrites: true` — este fix |
