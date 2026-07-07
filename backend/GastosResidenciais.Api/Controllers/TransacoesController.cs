using GastosResidenciais.Api.DTOs;
using GastosResidenciais.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GastosResidenciais.Api.Controllers;

/// <summary>
/// Endpoints REST para o cadastro de transações (criação, listagem e exclusão).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TransacoesController : ControllerBase
{
    private readonly ITransacaoService _transacaoService;

    public TransacoesController(ITransacaoService transacaoService)
    {
        _transacaoService = transacaoService;
    }

    /// <summary>Lista todas as transações cadastradas.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransacaoResponse>>> Listar()
    {
        var transacoes = await _transacaoService.ListarAsync();
        return Ok(transacoes);
    }

    /// <summary>Cadastra uma nova transação.</summary>
    [HttpPost]
    public async Task<ActionResult<TransacaoResponse>> Criar([FromBody] CriarTransacaoRequest request)
    {
        var transacao = await _transacaoService.CriarAsync(request);
        return CreatedAtAction(nameof(Listar), new { id = transacao.Id }, transacao);
    }

    /// <summary>Remove uma transação (a pessoa dona não é afetada).</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        await _transacaoService.RemoverAsync(id);
        return NoContent();
    }
}
