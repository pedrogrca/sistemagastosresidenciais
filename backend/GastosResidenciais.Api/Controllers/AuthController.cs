using GastosResidenciais.Api.DTOs;
using GastosResidenciais.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GastosResidenciais.Api.Controllers;

/// <summary>
/// Endpoints de autenticação. São públicos ([AllowAnonymous]) — afinal, o
/// usuário precisa conseguir se registrar e logar sem já estar autenticado.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Registra um novo usuário e já devolve um token para acesso imediato.</summary>
    [HttpPost("registrar")]
    public async Task<ActionResult<AuthResponse>> Registrar([FromBody] RegistrarRequest request)
    {
        var resposta = await _authService.RegistrarAsync(request);
        return Ok(resposta);
    }

    /// <summary>Autentica um usuário e devolve o token JWT.</summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var resposta = await _authService.LoginAsync(request);
        return Ok(resposta);
    }
}
