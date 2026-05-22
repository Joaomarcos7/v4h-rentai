using MediatR;
using V4H.Application.Teleconsultorias.DTOs;

namespace V4H.Application.Teleconsultorias.Queries;

public record GetTeleconsultoriaDetailQuery(Guid Id) : IRequest<TeleconsultoriaDetailDto>;
