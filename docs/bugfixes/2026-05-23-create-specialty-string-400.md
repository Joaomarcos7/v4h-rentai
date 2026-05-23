# Bug Report: Criação de teleconsultoria retornando 400 ao selecionar especialidade diferente

**Data:** 2026-05-23  
**Severidade:** Alta — criação bloqueada para qualquer especialidade selecionada pelo usuário  
**Status:** Corrigido

---

## Descrição

Ao criar uma teleconsultoria e selecionar qualquer especialidade diferente da primeira opção no dropdown, o endpoint `POST /api/teleconsultorias` retornava **400 Bad Request**. A primeira opção funcionava por acaso — o valor inicial `specialty: 1` era um número, mas após interação do usuário com o `<select>`, o valor tornava-se uma string.

---

## Causa Raiz

Mesma raiz dos bugs anteriores de enum (`register-role-string-400`, `update-status-string-500`): o template usava `[value]` nas options:

```html
<option [value]="1">Cardiologia</option>
<option [value]="2">Cirurgia Robótica</option>
...
```

`[value]` vincula ao atributo DOM (sempre string). Após interação do usuário, `form.specialty` tornava-se `"2"` (string). O `TeleconsultoriaService.create` serializa `{ specialty: "2" }`. O backend não consegue deserializar string para o enum `Specialty` → 400.

A primeira opção (`Cardiologia`) só funcionava enquanto o usuário não interagia com o dropdown, pois `form.specialty = 1` era inicializado como number no componente.

---

## Solução

### Fix 1 — Template: `[value]` → `[ngValue]`

```html
<option [ngValue]="1">Cardiologia</option>
<option [ngValue]="2">Cirurgia Robótica</option>
<option [ngValue]="3">Odontologia</option>
<option [ngValue]="4">Doenças Raras</option>
<option [ngValue]="5">Oxigenoterapia</option>
```

### Fix 2 — Service: coerção defensiva `+specialty`

```typescript
create(dto: CreateTeleconsultoriaDto) {
  return this.http.post<{ id: string }>(this.base, { ...dto, specialty: +dto.specialty });
}
```

---

## Teste

```typescript
it('should send specialty as number (not string) in create request', () => {
  (component.form as any).specialty = '3'; // simula DOM string coercion

  component.submit();

  const req = http.expectOne(`${environment.apiUrl}/teleconsultorias`);
  expect(typeof req.request.body['specialty']).toBe('number');
  expect(req.request.body['specialty']).toBe(3);
});
```

---

## Padrão recorrente

Este é o terceiro bug do mesmo padrão `[value]` vs `[ngValue]` no projeto. Todos os selects com valores numéricos agora usam `[ngValue]` + coerção defensiva no service. Ver também:
- [register-role-string-400](./2026-05-23-register-role-string-400.md)
- [update-status-string-500](./2026-05-23-update-status-string-500.md)
