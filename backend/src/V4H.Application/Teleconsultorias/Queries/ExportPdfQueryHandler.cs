using MediatR;
using V4H.Application.Common.Exceptions;
using V4H.Application.Common.Interfaces;
using V4H.Domain.Entities;
using V4H.Domain.Interfaces.Repositories;

namespace V4H.Application.Teleconsultorias.Queries;

public class ExportPdfQueryHandler : IRequestHandler<ExportPdfQuery, byte[]>
{
    private readonly ITeleconsultoriaRepository _repo;
    private readonly IPdfExportService _pdf;

    public ExportPdfQueryHandler(ITeleconsultoriaRepository repo, IPdfExportService pdf)
    {
        _repo = repo;
        _pdf = pdf;
    }

    public async Task<byte[]> Handle(ExportPdfQuery request, CancellationToken cancellationToken)
    {
        var tc = await _repo.GetByIdWithDetailsAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Teleconsultoria), request.Id);

        return _pdf.Export(tc);
    }
}
