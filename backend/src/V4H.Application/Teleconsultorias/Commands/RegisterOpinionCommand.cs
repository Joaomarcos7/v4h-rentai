using MediatR;

namespace V4H.Application.Teleconsultorias.Commands;

public record RegisterOpinionCommand(
    Guid TeleconsultoriaId,
    Guid SpecialistId,
    string Content) : IRequest<Guid>;
