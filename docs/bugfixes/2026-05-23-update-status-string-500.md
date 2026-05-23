# Bug Report: Atualização de status retornando 500 e dropdown não refletindo status atual

**Data:** 2026-05-23  
**Severidade:** Alta — especialista não conseguia atualizar status da teleconsultoria  
**Status:** Corrigido

---

## Descrição

Dois problemas no fluxo de atualização de status pelo Especialista:

1. Ao clicar em "Atualizar", o endpoint `PUT /api/teleconsultorias/{id}/status` retornava **500 Internal Server Error**
2. O dropdown de status sempre iniciava em "Em Andamento" independente do status real da teleconsultoria

---

## Causa Raiz

### Problema 1 — 500 no PUT

Mesma raiz do bug de cadastro: `[value]="1"` nas options do `<select>` vincula ao atributo DOM (sempre string). O `ngModel` atribui a string `"2"` à propriedade `newStatus`. O `TeleconsultoriaService.updateStatus` serializa `{ status: "2" }`. O backend não consegue deserializar string para o enum `TeleconsultoriaStatus` → 500.

### Problema 2 — Dropdown não reflete status atual

`newStatus` era inicializado como `newStatus = 2` (hardcoded). Após carregar a teleconsultoria, o valor não era atualizado para o status real. O backend retorna status como string (`"Pendente"`, `"EmAndamento"`, `"Concluida"`, `"Cancelada"`) via `.ToString()`, mas o dropdown usa valores inteiros — faltava o mapeamento string → int.

---

## Solução

### Fix 1 — Template: `[value]` → `[ngValue]`

```html
<!-- Antes -->
<option [value]="1">Pendente</option>
<option [value]="2">Em Andamento</option>
<option [value]="3">Concluída</option>
<option [value]="4">Cancelada</option>

<!-- Depois -->
<option [ngValue]="1">Pendente</option>
<option [ngValue]="2">Em Andamento</option>
<option [ngValue]="3">Concluída</option>
<option [ngValue]="4">Cancelada</option>
```

### Fix 2 — Service: coerção defensiva `+status`

```typescript
// Antes
updateStatus(id: string, status: number, notes?: string) {
  return this.http.put(`${this.base}/${id}/status`, { status, notes });
}

// Depois
updateStatus(id: string, status: number, notes?: string) {
  return this.http.put(`${this.base}/${id}/status`, { status: +status, notes });
}
```

### Fix 3 — Component: inicializar `newStatus` do status atual

```typescript
private static readonly statusMap: Record<string, number> = {
  Pendente: 1, EmAndamento: 2, Concluida: 3, Cancelada: 4
};

load() {
  this.loading.set(true);
  this.tcService.getById(this.id).subscribe({
    next: (data) => {
      this.tc.set(data);
      this.newStatus = DetailComponent.statusMap[data.status] ?? 1; // ← novo
      this.loading.set(false);
    },
    error: () => this.loading.set(false)
  });
}
```

---

## Testes

Adicionado `detail.component.spec.ts` com dois casos:

```typescript
it('should initialize newStatus as integer matching current tc status', () => {
  // tc.status = "EmAndamento" → newStatus deve ser 2
  expect(component.newStatus).toBe(2);
});

it('should send status as number (not string) in updateStatus request', () => {
  (component as any).newStatus = '3'; // simula coerção de string pelo DOM
  component.updateStatus();
  const req = http.expectOne(...);
  expect(typeof req.request.body['status']).toBe('number');
  expect(req.request.body['status']).toBe(3);
});
```

---

## Relação com Bug Anterior

Mesmo padrão do bug `2026-05-23-register-role-string-400.md`: `[value]` vs `[ngValue]` em selects Angular com valores não-string. A correção segue a mesma estratégia: `[ngValue]` no template + coerção `+` no service.
