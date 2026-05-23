# ADR-001: Clean Architecture como estrutura do backend

**Data:** 2026-05-21  
**Status:** Aceito

## Contexto

O sistema de teleconsultoria requer separação clara de responsabilidades entre regras de negócio, casos de uso e detalhes de infraestrutura. O time de avaliação espera maturidade arquitetural e código testável de forma isolada.

## Decisão

Adotar Clean Architecture com quatro projetos:

| Projeto | Responsabilidade |
|---------|-----------------|
| `V4H.Domain` | Entidades, enums, interfaces de repositório |
| `V4H.Application` | Casos de uso (CQRS), DTOs, interfaces de portas |
| `V4H.Infrastructure` | EF Core, repositórios, serviços externos |
| `V4H.API` | Controllers, middleware, configuração DI |

**Regra de dependência:** `Domain ← Application ← Infrastructure → API`. API depende de Application e Infrastructure via DI.

## Consequências

**Positivo:**
- Regras de negócio testáveis sem banco de dados (mocks via NSubstitute)
- Infraestrutura substituível (banco, AI, storage) sem alterar Application
- Estrutura familiar para times .NET com experiência em DDD

**Negativo:**
- Mais projetos e arquivos comparado a uma estrutura simples (MVC monolítico)
- Overhead inicial de setup; justificado pelo escopo do desafio

## Alternativas Consideradas

- **MVC monolítico:** mais rápido para protótipos, mas dificulta testes e separação de responsabilidades
- **Vertical Slice Architecture:** excelente para equipes maiores, mas overhead desnecessário para escopo único de entrega
