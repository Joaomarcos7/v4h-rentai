using MediatR;
using Microsoft.AspNetCore.Mvc;
using V4H.Application.Auth.Commands;
using V4H.Domain.Enums;

namespace V4H.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var id = await _mediator.Send(new RegisterCommand(req.Name, req.Email, req.Password, req.Role));
        return CreatedAtAction(nameof(Register), new { userId = id });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var result = await _mediator.Send(new LoginCommand(req.Email, req.Password));
        return Ok(result);
    }
}

public record RegisterRequest(string Name, string Email, string Password, UserRole Role);
public record LoginRequest(string Email, string Password);
