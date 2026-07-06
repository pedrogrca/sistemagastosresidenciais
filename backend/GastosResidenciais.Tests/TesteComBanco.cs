using GastosResidenciais.Api.Data;
using GastosResidenciais.Api.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.Tests;

/// <summary>
/// Classe base para os testes que precisam de banco de dados.
///
/// Usa um SQLite EM MEMÓRIA: é rápido, isolado por teste e — ao contrário do
/// provedor "InMemory" do EF — respeita relacionamentos e a exclusão em cascata.
/// A conexão é mantida aberta durante o teste (o banco em memória existe apenas
/// enquanto a conexão viver) e "Foreign Keys=True" garante que a cascata seja
/// realmente aplicada.
/// </summary>
public abstract class TesteComBanco : IDisposable
{
    private readonly SqliteConnection _conexao;
    private readonly DbContextOptions<AppDbContext> _opcoes;

    protected TesteComBanco()
    {
        _conexao = new SqliteConnection("DataSource=:memory:;Foreign Keys=True");
        _conexao.Open();

        _opcoes = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_conexao)
            .Options;

        // Cria o schema (tabelas) a partir do modelo.
        using var contexto = CriarContexto();
        contexto.Database.EnsureCreated();
    }

    /// <summary>Cria um novo contexto apontando para o mesmo banco em memória.</summary>
    protected AppDbContext CriarContexto() => new(_opcoes);

    /// <summary>Insere uma pessoa direto no banco (para o "Arrange" dos testes).</summary>
    protected Pessoa SemearPessoa(string nome, int idade)
    {
        using var contexto = CriarContexto();
        var pessoa = new Pessoa { Nome = nome, Idade = idade };
        contexto.Pessoas.Add(pessoa);
        contexto.SaveChanges();
        return pessoa;
    }

    /// <summary>Insere uma transação direto no banco (para o "Arrange" dos testes).</summary>
    protected void SemearTransacao(int pessoaId, string descricao, decimal valor, TipoTransacao tipo)
    {
        using var contexto = CriarContexto();
        contexto.Transacoes.Add(new Transacao
        {
            Descricao = descricao,
            Valor = valor,
            Tipo = tipo,
            PessoaId = pessoaId
        });
        contexto.SaveChanges();
    }

    public void Dispose()
    {
        _conexao.Dispose();
        GC.SuppressFinalize(this);
    }
}
