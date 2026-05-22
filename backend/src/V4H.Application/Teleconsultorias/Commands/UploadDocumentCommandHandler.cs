using MediatR;
using Microsoft.Extensions.Configuration;
using V4H.Application.Common.Exceptions;
using V4H.Application.Common.Interfaces;
using V4H.Domain.Entities;
using V4H.Domain.Interfaces.Repositories;

namespace V4H.Application.Teleconsultorias.Commands;

public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, Guid>
{
    private readonly ITeleconsultoriaRepository _repo;
    private readonly IDocumentValidationService _validator;
    private readonly IFileStorageService _storage;
    private readonly IConfiguration _config;

    public UploadDocumentCommandHandler(
        ITeleconsultoriaRepository repo,
        IDocumentValidationService validator,
        IFileStorageService storage,
        IConfiguration config)
    {
        _repo = repo;
        _validator = validator;
        _storage = storage;
        _config = config;
    }

    public async Task<Guid> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        var tc = await _repo.GetByIdWithDetailsAsync(request.TeleconsultoriaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Teleconsultoria), request.TeleconsultoriaId);

        if (tc.RequesterId != request.RequesterId)
            throw new UnauthorizedException("Only the requester can upload documents.");

        using var ms = new MemoryStream();
        await request.FileStream.CopyToAsync(ms, cancellationToken);

        var threshold = _config.GetValue<decimal>("AI:ValidationThreshold", 0.6m);
        ms.Position = 0;
        var validationResult = await _validator.ValidateAsync(ms, request.MimeType, cancellationToken);

        if (validationResult.Score < threshold)
            throw new DocumentValidationException(validationResult.Score);

        ms.Position = 0;
        var storedPath = await _storage.SaveAsync(ms, request.FileName, cancellationToken);

        var doc = TeleconsultoriaDocument.Create(
            tc.Id,
            request.FileName,
            storedPath,
            validationResult.Score,
            validationResult.Provider,
            threshold,
            validationResult.Timestamp);

        await _repo.AddDocumentAsync(doc, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);

        return doc.Id;
    }
}
