using GastosResidenciais.Api.Data;
using GastosResidenciais.Api.DTOs;
using GastosResidenciais.Api.Exceptions;
using GastosResidenciais.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.Api.Services;

/// <summary>
/// Regras e operações de negócio relacionadas a transações.
/// </summary>
public class TransacaoService : ITransacaoService
{
    private readonly AppDbContext _context;

    public TransacaoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TransacaoResponse> CriarAsync(CriarTransacaoRequest request)
    {
        // Regra 1: a pessoa informada precisa existir no cadastro.
        // (request.PessoaId e request.Tipo já foram garantidos como não-nulos
        //  pela validação [Required] do DTO antes de chegar aqui.)
        var pessoa = await _context.Pessoas.FindAsync(request.PessoaId!.Value)
            ?? throw new NaoEncontradoException(
                $"Pessoa com Id {request.PessoaId} não encontrada.");

        var tipo = request.Tipo!.Value;

        // Regra 2: menor de 18 anos só pode cadastrar despesa, nunca receita.
        if (pessoa.EhMenorDeIdade && tipo == TipoTransacao.Receita)
        {
            throw new RegraNegocioException(
                "Pessoas menores de 18 anos só podem cadastrar despesas, não receitas.");
        }

        var transacao = new Transacao
        {
            Descricao = request.Descricao.Trim(),
            Valor = request.Valor,
            Tipo = tipo,
            PessoaId = pessoa.Id
        };

        _context.Transacoes.Add(transacao);
        await _context.SaveChangesAsync();

        return new TransacaoResponse
        {
            Id = transacao.Id,
            Descricao = transacao.Descricao,
            Valor = transacao.Valor,
            Tipo = transacao.Tipo,
            PessoaId = pessoa.Id,
            PessoaNome = pessoa.Nome
        };
    }

    public async Task<IEnumerable<TransacaoResponse>> ListarAsync()
    {
        return await _context.Transacoes
            .AsNoTracking()
            .OrderBy(t => t.Id)
            .Select(t => new TransacaoResponse
            {
                Id = t.Id,
                Descricao = t.Descricao,
                Valor = t.Valor,
                Tipo = t.Tipo,
                PessoaId = t.PessoaId,
                // t.Pessoa nunca é nulo aqui: toda transação tem uma pessoa (FK obrigatória).
                PessoaNome = t.Pessoa!.Nome
            })
            .ToListAsync();
    }
}
