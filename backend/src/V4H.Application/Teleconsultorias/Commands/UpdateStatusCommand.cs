using MediatR;
using V4H.Domain.Enums;

namespace V4H.Application.Teleconsultorias.Commands;

public record UpdateStatusCommand(
    Guid TeleconsultoriaId,
    TeleconsultoriaStatus NewStatus,
    Guid ChangedById,
    string? Notes) : IRequest;
