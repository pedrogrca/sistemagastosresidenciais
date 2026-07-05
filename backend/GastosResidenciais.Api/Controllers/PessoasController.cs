using GastosResidenciais.Api.DTOs;
using GastosResidenciais.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GastosResidenciais.Api.Controllers;

/// <summary>
/// Endpoints REST para o cadastro de pessoas.
/// O controller é propositalmente "fino": ele apenas recebe a requisição,
/// chama o serviço e devolve o status HTTP adequado.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PessoasController : ControllerBase
{
    private readonly IPessoaService _pessoaService;

    public PessoasController(IPessoaService pessoaService)
    {
        _pessoaService = pessoaService;
    }

    /// <summary>Lista todas as pessoas cadastradas.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PessoaResponse>>> Listar()
    {
        var pessoas = await _pessoaService.ListarAsync();
        return Ok(pessoas);
    }

    /// <summary>Cadastra uma nova pessoa.</summary>
    [HttpPost]
    public async Task<ActionResult<PessoaResponse>> Criar([FromBody] CriarPessoaRequest request)
    {
        var pessoa = await _pessoaService.CriarAsync(request);

        // 201 Created + cabeçalho Location apontando para a listagem.
        return CreatedAtAction(nameof(Listar), new { id = pessoa.Id }, pessoa);
    }

    /// <summary>Remove uma pessoa (e todas as suas transações, por cascata).</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        await _pessoaService.RemoverAsync(id);
        return NoContent(); // 204: sucesso, sem corpo de resposta.
    }
}
