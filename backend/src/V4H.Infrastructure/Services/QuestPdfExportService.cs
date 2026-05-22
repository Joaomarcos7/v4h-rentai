using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using V4H.Application.Common.Interfaces;
using V4H.Domain.Entities;

namespace V4H.Infrastructure.Services;

public class QuestPdfExportService : IPdfExportService
{
    public QuestPdfExportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] Export(Teleconsultoria tc)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Text($"Teleconsultoria — {tc.PatientName}")
                    .SemiBold().FontSize(16).FontColor(Colors.Blue.Medium);

                page.Content().Column(col =>
                {
                    col.Item().Text($"Especialidade: {tc.Specialty}");
                    col.Item().Text($"Status: {tc.Status}");
                    col.Item().Text($"Data de Nascimento: {tc.BirthDate:dd/MM/yyyy}");
                    col.Item().Text($"Hipótese Diagnóstica: {tc.DiagnosticHypothesis}");
                    col.Item().Text($"Histórico Clínico: {tc.ClinicalHistory}");

                    if (tc.Opinions.Any())
                    {
                        col.Item().PaddingTop(10).Text("Pareceres:").SemiBold();
                        foreach (var op in tc.Opinions)
                        {
                            col.Item().Text($"• {op.Specialist?.Name ?? "Especialista"} ({op.CreatedAt:dd/MM/yyyy HH:mm}): {op.Content}");
                        }
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("V4H ReNTAI — Gerado em ");
                    x.Span(DateTimeOffset.UtcNow.ToString("dd/MM/yyyy HH:mm"));
                });
            });
        }).GeneratePdf();
    }
}
