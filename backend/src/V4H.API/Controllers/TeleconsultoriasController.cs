using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using V4H.Application.Teleconsultorias.Commands;
using V4H.Application.Teleconsultorias.Queries;
using V4H.Domain.Enums;

namespace V4H.API.Controllers;

[ApiController]
[Route("api/teleconsultorias")]
[Authorize]
public class TeleconsultoriasController : ControllerBase
{
    private readonly IMediator _mediator;

    public TeleconsultoriasController(IMediator mediator) => _mediator = mediator;

    private Guid CurrentUserId
    {
        get
        {
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue("sub");
            return Guid.Parse(sub ?? throw new InvalidOperationException("Missing user ID claim."));
        }
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] Specialty? specialty,
        [FromQuery] string? patient,
        [FromQuery] TeleconsultoriaStatus? status,
        [FromQuery] DateTimeOffset? dateFrom,
        [FromQuery] DateTimeOffset? dateTo)
    {
        var result = await _mediator.Send(
            new ListTeleconsultoriasQuery(specialty, patient, status, dateFrom, dateTo));
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Solicitante")]
    public async Task<IActionResult> Create([FromBody] CreateTeleconsultoriaRequest req)
    {
        var id = await _mediator.Send(new CreateTeleconsultoriaCommand(
            req.PatientName, req.BirthDate, req.Specialty,
            req.DiagnosticHypothesis, req.ClinicalHistory, CurrentUserId));
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetTeleconsultoriaDetailQuery(id));
        return Ok(result);
    }

    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = "Especialista")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest req)
    {
        await _mediator.Send(new UpdateStatusCommand(id, req.Status, CurrentUserId, req.Notes));
        return NoContent();
    }

    [HttpPost("{id:guid}/documents")]
    [Authorize(Roles = "Solicitante")]
    public async Task<IActionResult> UploadDocument(Guid id, IFormFile file)
    {
        var docId = await _mediator.Send(new UploadDocumentCommand(
            id, file.OpenReadStream(), file.FileName, file.ContentType, CurrentUserId));
        return CreatedAtAction(nameof(GetById), new { id }, new { id = docId });
    }

    [HttpPost("{id:guid}/opinions")]
    [Authorize(Roles = "Especialista")]
    public async Task<IActionResult> RegisterOpinion(Guid id, [FromBody] RegisterOpinionRequest req)
    {
        var opinionId = await _mediator.Send(
            new RegisterOpinionCommand(id, CurrentUserId, req.Content));
        return CreatedAtAction(nameof(GetById), new { id }, new { id = opinionId });
    }

    [HttpGet("{id:guid}/export/pdf")]
    public async Task<IActionResult> ExportPdf(Guid id)
    {
        var bytes = await _mediator.Send(new ExportPdfQuery(id));
        return File(bytes, "application/pdf", $"teleconsultoria-{id}.pdf");
    }
}

public record CreateTeleconsultoriaRequest(
    string PatientName,
    DateOnly BirthDate,
    Specialty Specialty,
    string DiagnosticHypothesis,
    string ClinicalHistory);

public record UpdateStatusRequest(TeleconsultoriaStatus Status, string? Notes);
public record RegisterOpinionRequest(string Content);
