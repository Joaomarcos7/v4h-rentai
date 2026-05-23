# Bug Report: Notificação em tempo real não recarrega tela do solicitante (causa raiz: NgZone)

**Data:** 2026-05-23  
**Severidade:** Alta — solicitante não vê novo parecer sem recarregar a página manualmente  
**Status:** Corrigido

---

## Descrição

Mesmo após adicionar `effect()` no `DetailComponent` para escutar `lastOpinionNotification`, a tela do solicitante não era atualizada automaticamente quando o especialista registrava um parecer. O sinal era atualizado no serviço, mas o `effect()` nunca disparava.

---

## Causa Raiz

O callback passado a `connection.on('NewOpinion', callback)` do SignalR é executado **fora da NgZone do Angular**. Isso significa que quando o sinal `lastOpinionNotification` era atualizado dentro do callback, o Angular não agendava um ciclo de detecção de mudanças — e portanto os `effect()`s que dependiam desse sinal nunca executavam.

Em outras palavras:

```
SignalR callback (fora da zone)
  → signal.set(payload)          ← Angular não "vê" essa mudança
  → effect() NÃO dispara
  → tela não atualiza
```

---

## Solução

Injetar `NgZone` no `NotificationService` e envolver a atualização do sinal em `zone.run()`. Também foi extraído o método `handleNewOpinion()` para facilitar testes isolados.

```typescript
import { Injectable, NgZone, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  readonly lastOpinionNotification = signal<NewOpinionPayload | null>(null);

  constructor(private auth: AuthService, private zone: NgZone) {}

  handleNewOpinion(payload: NewOpinionPayload) {
    this.zone.run(() => this.lastOpinionNotification.set(payload));
  }

  connect() {
    // ...
    this.connection.on('NewOpinion', (payload: NewOpinionPayload) => {
      this.handleNewOpinion(payload);  // ← agora dentro da zone
    });
  }
}
```

---

## Teste

```typescript
it('should update lastOpinionNotification inside Angular zone so effects trigger', fakeAsync(() => {
  const payload = { teleconsultoriaId: 'tc-1', opinionId: 'op-1' };
  let isInsideZone = false;

  const originalSet = service.lastOpinionNotification.set.bind(service.lastOpinionNotification);
  spyOn(service.lastOpinionNotification, 'set').and.callFake((val) => {
    isInsideZone = NgZone.isInAngularZone();
    originalSet(val);
  });

  // Simula callback SignalR disparando FORA da zone (como em produção)
  zone.runOutsideAngular(() => {
    (service as any).handleNewOpinion(payload);
  });

  tick();

  expect(isInsideZone).toBe(true);
  expect(service.lastOpinionNotification()).toEqual(payload);
}));
```

---

## Contexto adicional

Este bug tem dois layers:

1. **Layer 1** (fix anterior): `DetailComponent` precisava de um `effect()` para reagir a mudanças no sinal — adicionado em [signalr-notification-not-reloading-detail](./2026-05-23-signalr-notification-not-reloading-detail.md).

2. **Layer 2** (este fix): O próprio `set()` do sinal ocorria fora da zone, então o `effect()` nunca era acionado mesmo existindo.

Qualquer integração com WebSocket, SSE, ou bibliotecas que usam callbacks nativos (fora do Angular) deve sempre envolver atualizações de estado em `zone.run()`.
