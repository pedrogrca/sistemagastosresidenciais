using GastosResidenciais.Api.Data;
using GastosResidenciais.Api.DTOs;
using GastosResidenciais.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.Api.Services;

/// <summary>
/// Serviço responsável por consolidar os totais financeiros do sistema.
/// </summary>
public class TotaisService : ITotaisService
{
    private readonly AppDbContext _context;

    public TotaisService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TotaisResponse> ObterAsync()
    {
        // Carrega todas as pessoas junto de suas transações (Include).
        // Os somatórios são feitos em memória (C#) e não no banco: o SQLite
        // não agrega o tipo decimal de forma confiável, então trazemos os dados
        // e usamos o Sum() do LINQ-to-Objects, garantindo precisão monetária.
        var pessoas = await _context.Pessoas
            .AsNoTracking()
            .Include(p => p.Transacoes)
            .OrderBy(p => p.Id)
            .ToListAsync();

        var totaisPorPessoa = pessoas
            .Select(CalcularTotaisDaPessoa)
            .ToList();

        // O total geral é a soma dos totais individuais já calculados.
        var totalGeral = new TotalGeralResponse
        {
            TotalReceitas = totaisPorPessoa.Sum(p => p.TotalReceitas),
            TotalDespesas = totaisPorPessoa.Sum(p => p.TotalDespesas),
            SaldoLiquido = totaisPorPessoa.Sum(p => p.Saldo)
        };

        return new TotaisResponse
        {
            Pessoas = totaisPorPessoa,
            TotalGeral = totalGeral
        };
    }

    /// <summary>Calcula receitas, despesas e saldo de uma única pessoa.</summary>
    private static TotalPorPessoaResponse CalcularTotaisDaPessoa(Pessoa pessoa)
    {
        var totalReceitas = pessoa.Transacoes
            .Where(t => t.Tipo == TipoTransacao.Receita)
            .Sum(t => t.Valor);

        var totalDespesas = pessoa.Transacoes
            .Where(t => t.Tipo == TipoTransacao.Despesa)
            .Sum(t => t.Valor);

        return new TotalPorPessoaResponse
        {
            PessoaId = pessoa.Id,
            Nome = pessoa.Nome,
            TotalReceitas = totalReceitas,
            TotalDespesas = totalDespesas,
            Saldo = totalReceitas - totalDespesas
        };
    }
}
