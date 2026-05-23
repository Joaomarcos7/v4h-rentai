# ADR-005: Serviço de IA como mock com interface real

**Data:** 2026-05-21  
**Status:** Aceito

## Contexto

O edital exige integração com validação de documentos via IA. Não existe uma API de IA disponibilizada pelo projeto; integração com serviços externos (OpenAI, AWS Rekognition) está fora do escopo do prazo.

## Decisão

Definir a interface `IDocumentValidationService` na camada Application e implementar um mock configurável na camada Infrastructure:

```csharp
public interface IDocumentValidationService
{
    Task<ValidationResult> ValidateAsync(Stream file, string mimeType, CancellationToken ct = default);
}

public record ValidationResult(decimal Score, string Provider, DateTimeOffset Timestamp);
```

**Comportamento do mock:**
- `AI__MockScore` configurado → retorna score fixo (útil para testes)
- Sem configuração → score por mimeType: PDF → 0.92, JPEG/PNG → 0.78, outros → 0.25
- Threshold configurável via `AI__ValidationThreshold` (padrão 0.6)
- Score abaixo do threshold → `DocumentValidationException` → HTTP 422 com score

**Substituição:** trocar `MockDocumentValidationService` por implementação real não requer alteração em Application ou API — apenas o registro no DI Container.

## Consequências

**Positivo:**
- Demonstra design correto de inversão de dependência
- Testável: handlers testados com `IDocumentValidationService` mockado via NSubstitute
- Produção-ready: um `RealDocumentValidationService` pode ser injetado sem refatoração

**Negativo:**
- Não valida conteúdo real dos documentos (intencional)

## Alternativas Consideradas

- **Integração direta com OpenAI Vision:** requer chave de API paga e adiciona latência; fora de escopo
- **Hardcode do score:** impossível testar casos de rejeição e aprovação separadamente
