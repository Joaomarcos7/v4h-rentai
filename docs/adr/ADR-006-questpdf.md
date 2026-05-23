# ADR-006: QuestPDF para exportação de PDF

**Data:** 2026-05-21  
**Status:** Aceito

## Contexto

O sistema deve exportar detalhes de uma teleconsultoria (dados do paciente, hipótese diagnóstica, histórico clínico, pareceres) em formato PDF para download.

## Decisão

Usar a biblioteca **QuestPDF** (licença Community — gratuita para projetos não comerciais):

- Geração de PDF em memória (`byte[]`) via Fluent API
- Sem dependência de libgdiplus, Ghostscript ou headless browser
- Implementado em `QuestPdfExportService : IPdfExportService`
- `IPdfExportService` definida na camada Application (porta); implementação na Infrastructure

## Consequências

**Positivo:**
- API fluente e legível — layout declarativo similar a CSS Flexbox
- Licença Community adequada para ambiente acadêmico/avaliativo
- Sem dependências nativas — funciona em Linux/Docker sem libgdiplus

**Negativo:**
- Curva de aprendizado para layouts complexos (tabelas, imagens embutidas)
- Layout atual é simples; relatórios mais ricos requerem mais código de layout

## Alternativas Consideradas

- **iTextSharp/iText7:** licença AGPL obriga abertura do código ou licença comercial paga
- **PdfSharp:** boa opção open source, mas API de baixo nível mais verbosa
- **Puppeteer (headless Chrome):** flexível para layouts HTML→PDF, mas adiciona dependência pesada de Node.js/Chrome no container
