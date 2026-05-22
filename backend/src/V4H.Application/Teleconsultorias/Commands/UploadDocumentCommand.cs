using MediatR;

namespace V4H.Application.Teleconsultorias.Commands;

public record UploadDocumentCommand(
    Guid TeleconsultoriaId,
    Stream FileStream,
    string FileName,
    string MimeType,
    Guid RequesterId) : IRequest<Guid>;
