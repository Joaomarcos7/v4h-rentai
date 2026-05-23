# Bug Report: Notificação SignalR não funciona após refresh de página

**Data:** 2026-05-23  
**Severidade:** Alta — notificação em tempo real nunca funciona sem re-login  
**Status:** Corrigido

---

## Descrição

Mesmo com os fixes de NgZone e `effect()` aplicados, notificações de parecer em tempo real não chegavam ao solicitante. O hub SignalR só conectava imediatamente após o login — qualquer refresh de página ou navegação direta deixava a conexão inexistente.

---

## Causa Raiz

`NotificationService.connect()` era chamado exclusivamente dentro do `LoginComponent.submit()`, no callback de sucesso do login:

```typescript
this.auth.login(...).subscribe({
  next: () => {
    this.notifications.connect(); // ← único lugar
    this.router.navigate(['/dashboard']);
  }
});
```

Fluxos que não passam pelo submit de login (refresh, abertura direta de URL com sessão ativa) nunca chamavam `connect()`, portanto o WebSocket nunca era estabelecido e nenhum evento SignalR chegava ao cliente.

---

## Solução

Chamar `connect()` no `AppComponent.ngOnInit()`, verificando se o usuário já tem sessão ativa (token no localStorage):

```typescript
@Component({ selector: 'app-root', ... })
export class AppComponent implements OnInit {
  constructor(private auth: AuthService, private notifications: NotificationService) {}

  ngOnInit() {
    if (this.auth.isLoggedIn()) {
      this.notifications.connect();
    }
  }
}
```

`connect()` já é idempotente (`if (this.connection) return`), então a chamada dupla no fluxo login → AppComponent não causa problemas.

---

## Histórico do bug (3 layers)

Este foi o terceiro fix necessário para a funcionalidade de notificação funcionar ponta a ponta:

| Layer | Problema | Fix |
|-------|----------|-----|
| 1 | `DetailComponent` não reagia ao sinal | Adicionado `effect()` em [signalr-notification-not-reloading-detail](./2026-05-23-signalr-notification-not-reloading-detail.md) |
| 2 | Signal atualizado fora da NgZone | `zone.run()` em [signalr-ngzone-outside-zone](./2026-05-23-signalr-ngzone-outside-zone.md) |
| 3 | `connect()` nunca chamado após refresh | `AppComponent.ngOnInit()` — este fix |
