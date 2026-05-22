using MediatR;

namespace V4H.Application.Teleconsultorias.Queries;

public record ExportPdfQuery(Guid Id) : IRequest<byte[]>;
