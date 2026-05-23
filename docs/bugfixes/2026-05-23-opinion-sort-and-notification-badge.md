# Feature: Ordenação da linha do tempo de pareceres + badge de notificação em tempo real

**Data:** 2026-05-23  
**Tipo:** Bugfix (ordenação) + Feature (badge)  
**Status:** Implementado

---

## Ordenação dos pareceres

### Problema
Pareceres exibidos na ordem retornada pela API (não garantida). Usuário via pareceres fora de ordem cronológica.

### Solução
Getter `sortedOpinions` no `DetailComponent` ordena por `createdAt` ascendente (mais antigo no topo, mais recente no final — padrão de linha do tempo / chat):

```typescript
get sortedOpinions() {
  return [...(this.tc()?.opinions ?? [])].sort((a, b) =>
    new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime()
  );
}
```

Template usa `sortedOpinions` em vez de `tc()!.opinions` diretamente.

---

## Badge de notificação em tempo real

### Comportamento
Quando o solicitante está na tela de detalhes e um especialista registra novo parecer:
1. Parecer carregado automaticamente via SignalR
2. Banner verde aparece: "🔔 Novo parecer recebido!" com botão ✕ para fechar

### Implementação

```typescript
newOpinionAlert = signal(false);

onNotification(payload: ...) {
  if (payload && payload.teleconsultoriaId === this.id) {
    this.newOpinionAlert.set(true);  // ← exibe badge
    this.load();                      // ← recarrega dados
  }
}

dismissAlert() {
  this.newOpinionAlert.set(false);
}
```

Template renderiza o banner condicionalmente acima do card de pareceres.

---

## Testes

- `sortedOpinions` retorna pareceres em ordem ascendente independente da ordem recebida
- `newOpinionAlert` inicia `false`, vira `true` ao receber notificação com id correspondente
- `dismissAlert()` reseta para `false`
