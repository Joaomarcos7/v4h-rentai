# Bug Report: Cadastro de usuário retornando 400 Bad Request

**Data:** 2026-05-23  
**Severidade:** Alta — fluxo de cadastro completamente bloqueado  
**Status:** Corrigido

---

## Descrição

Ao submeter o formulário de cadastro, o endpoint `POST /api/auth/register` retornava **400 Bad Request**. O campo `role` era enviado como string numérica (`"1"` ou `"2"`) em vez de inteiro, e o backend não conseguia deserializar para o enum `UserRole`.

---

## Causa Raiz

O template Angular usava `[value]` nas opções do `<select>`:

```html
<option [value]="1">Solicitante (APS)</option>
<option [value]="2">Especialista</option>
```

`[value]` faz binding ao **atributo DOM** do elemento `<option>`, que é sempre uma string. Quando o usuário seleciona uma opção, o `ngModel` lê o valor do atributo DOM e atribui a string `"1"` ou `"2"` à propriedade `role` do componente — mesmo sendo inicializada como `number`.

Resultado: `AuthService.register` serializa `{ role: "2" }` no corpo JSON. O System.Text.Json (padrão do ASP.NET Core) não deserializa string numérica para enum por padrão → 400.

---

## Solução

### Fix 1 — Template: `[value]` → `[ngValue]`

```html
<!-- Antes -->
<option [value]="1">Solicitante (APS)</option>
<option [value]="2">Especialista</option>

<!-- Depois -->
<option [ngValue]="1">Solicitante (APS)</option>
<option [ngValue]="2">Especialista</option>
```

`[ngValue]` usa o sistema interno do Angular para armazenar o valor, preservando o tipo JavaScript original (number). O `ngModel` recebe o número, não a string do DOM.

### Fix 2 — AuthService: coerção defensiva com `+role`

```typescript
// Antes
register(name: string, email: string, password: string, role: number) {
  return this.http.post(`${environment.apiUrl}/auth/register`, { name, email, password, role });
}

// Depois
register(name: string, email: string, password: string, role: number) {
  return this.http.post(`${environment.apiUrl}/auth/register`, { name, email, password, role: +role });
}
```

`+role` converte qualquer string `"1"`/`"2"` para number antes da serialização JSON. Camada de defesa independente do comportamento do template.

---

## Teste

Adicionado `register.component.spec.ts` simulando a coerção de string que o DOM aplica:

```typescript
it('should send role as number even when DOM select coerces value to string', () => {
  const fixture = TestBed.createComponent(RegisterComponent);
  const component = fixture.componentInstance;

  component.name = 'Test User';
  component.email = 'test@test.com';
  component.password = 'secret123';
  (component as any).role = '2'; // simula [value]="2" coagido para string pelo DOM

  component.submit();

  const req = http.expectOne(`${environment.apiUrl}/auth/register`);
  expect(typeof req.request.body['role']).toBe('number');
  expect(req.request.body['role']).toBe(2);
  req.flush({});
});
```

---

## Diferença: `[value]` vs `[ngValue]`

| | `[value]` | `[ngValue]` |
|---|---|---|
| Binding alvo | Atributo DOM (`string`) | Sistema interno Angular (qualquer tipo) |
| Tipo preservado | Não — sempre `string` | Sim — mantém `number`, `object`, etc. |
| Uso correto | Valores string simples | Valores tipados (number, object, enum) |

---

## Lição

Nunca usar `[value]` em `<option>` quando o modelo espera um tipo não-string. Usar `[ngValue]` para preservar o tipo JS. Adicionar coerção defensiva no service como segunda camada de proteção.
