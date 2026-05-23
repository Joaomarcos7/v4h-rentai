# ADR-002: CQRS via MediatR

**Data:** 2026-05-21  
**Status:** Aceito

## Contexto

Os casos de uso do sistema têm naturezas diferentes: criação/upload/opiniões (comandos que modificam estado) e listagem/detalhes/exportação (consultas somente-leitura). Misturar essas responsabilidades em services genéricos dificulta testes e evolução.

## Decisão

Usar o padrão CQRS com a biblioteca MediatR:

- **Commands:** `RegisterCommand`, `CreateTeleconsultoriaCommand`, `UploadDocumentCommand`, `RegisterOpinionCommand`, `UpdateStatusCommand`
- **Queries:** `ListTeleconsultoriasQuery`, `GetTeleconsultoriaDetailQuery`, `ExportPdfQuery`

Cada handler é uma classe com responsabilidade única, injetada via MediatR no pipeline.

## Consequências

**Positivo:**
- Cada caso de uso é um handler isolado — testável individualmente com mocks
- Nenhum "service" genérico com dezenas de métodos
- Pipeline do MediatR permite adicionar behaviors (logging, validação) de forma transversal

**Negativo:**
- Proliferação de classes (um arquivo por comando/query); aceitável para o escopo
- Curva de aprendizado para desenvolvedores sem experiência com MediatR

## Alternativas Consideradas

- **Application Services tradicionais:** uma interface por agregado; resulta em classes grandes e dificulta cobertura de testes
- **CQRS com barramentos externos (NServiceBus):** overkill para a escala deste projeto
