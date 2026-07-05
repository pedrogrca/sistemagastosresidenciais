using GastosResidenciais.Api.DTOs;
using GastosResidenciais.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GastosResidenciais.Api.Controllers;

/// <summary>
/// Endpoint de consulta dos totais (receitas, despesas e saldo) por pessoa
/// e o total geral de todo o sistema.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TotaisController : ControllerBase
{
    private readonly ITotaisService _totaisService;

    public TotaisController(ITotaisService totaisService)
    {
        _totaisService = totaisService;
    }

    /// <summary>Retorna os totais por pessoa e o total geral.</summary>
    [HttpGet]
    public async Task<ActionResult<TotaisResponse>> Obter()
    {
        var totais = await _totaisService.ObterAsync();
        return Ok(totais);
    }
}
