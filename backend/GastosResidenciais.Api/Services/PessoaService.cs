using GastosResidenciais.Api.Data;
using GastosResidenciais.Api.DTOs;
using GastosResidenciais.Api.Exceptions;
using GastosResidenciais.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.Api.Services;

/// <summary>
/// Regras e operações de negócio relacionadas a pessoas.
/// Concentra o acesso ao banco, deixando o controller apenas com o "roteamento" HTTP.
/// </summary>
public class PessoaService : IPessoaService
{
    private readonly AppDbContext _context;

    public PessoaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PessoaResponse> CriarAsync(CriarPessoaRequest request)
    {
        var pessoa = new Pessoa
        {
            // Trim() remove espaços acidentais no início/fim do nome.
            Nome = request.Nome.Trim(),
            Idade = request.Idade
        };

        _context.Pessoas.Add(pessoa);
        await _context.SaveChangesAsync();

        return MapearParaResponse(pessoa);
    }

    public async Task<IEnumerable<PessoaResponse>> ListarAsync()
    {
        // AsNoTracking: consulta somente-leitura, mais performática (o EF não
        // precisa monitorar mudanças nas entidades retornadas).
        return await _context.Pessoas
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .Select(p => new PessoaResponse
            {
                Id = p.Id,
                Nome = p.Nome,
                Idade = p.Idade,
                MenorDeIdade = p.Idade < 18
            })
            .ToListAsync();
    }

    public async Task RemoverAsync(int id)
    {
        var pessoa = await _context.Pessoas.FindAsync(id)
            ?? throw new NaoEncontradoException($"Pessoa com Id {id} não encontrada.");

        // As transações vinculadas são apagadas automaticamente pela regra de
        // cascata configurada no AppDbContext (OnDelete: Cascade).
        _context.Pessoas.Remove(pessoa);
        await _context.SaveChangesAsync();
    }

    /// <summary>Converte a entidade de domínio no DTO exposto pela API.</summary>
    private static PessoaResponse MapearParaResponse(Pessoa pessoa) => new()
    {
        Id = pessoa.Id,
        Nome = pessoa.Nome,
        Idade = pessoa.Idade,
        MenorDeIdade = pessoa.EhMenorDeIdade
    };
}
