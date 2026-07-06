using GastosResidenciais.Api.DTOs;
using GastosResidenciais.Api.Exceptions;
using GastosResidenciais.Api.Models;
using GastosResidenciais.Api.Services;

namespace GastosResidenciais.Tests;

public class PessoaServiceTests : TesteComBanco
{
    [Fact]
    public async Task CriarAsync_DeveGerarIdERetornarDados()
    {
        using var contexto = CriarContexto();
        var servico = new PessoaService(contexto);

        var resposta = await servico.CriarAsync(new CriarPessoaRequest { Nome = "Ana", Idade = 30 });

        Assert.True(resposta.Id > 0);
        Assert.Equal("Ana", resposta.Nome);
        Assert.Equal(30, resposta.Idade);
        Assert.False(resposta.MenorDeIdade);
    }

    [Theory]
    [InlineData(17, true)]   // menor de 18 -> menor de idade
    [InlineData(18, false)]  // exatamente 18 -> maior de idade (limite)
    public async Task CriarAsync_DeveMarcarMenorDeIdadePeloLimite(int idade, bool esperadoMenor)
    {
        using var contexto = CriarContexto();
        var servico = new PessoaService(contexto);

        var resposta = await servico.CriarAsync(new CriarPessoaRequest { Nome = "Fulano", Idade = idade });

        Assert.Equal(esperadoMenor, resposta.MenorDeIdade);
    }

    [Fact]
    public async Task ListarAsync_DeveRetornarTodasAsPessoas()
    {
        SemearPessoa("Ana", 30);
        SemearPessoa("Bia", 15);

        using var contexto = CriarContexto();
        var servico = new PessoaService(contexto);

        var pessoas = (await servico.ListarAsync()).ToList();

        Assert.Equal(2, pessoas.Count);
    }

    [Fact]
    public async Task RemoverAsync_DeveExcluirAPessoa()
    {
        var pessoa = SemearPessoa("Ana", 30);

        using (var contexto = CriarContexto())
        {
            await new PessoaService(contexto).RemoverAsync(pessoa.Id);
        }

        using var verificacao = CriarContexto();
        Assert.Empty(verificacao.Pessoas);
    }

    [Fact]
    public async Task RemoverAsync_PessoaInexistente_DeveLancarNaoEncontrado()
    {
        using var contexto = CriarContexto();
        var servico = new PessoaService(contexto);

        await Assert.ThrowsAsync<NaoEncontradoException>(() => servico.RemoverAsync(999));
    }

    [Fact]
    public async Task RemoverAsync_DeveApagarTransacoesEmCascata()
    {
        // Arrange: uma pessoa com duas transações.
        var pessoa = SemearPessoa("Ana", 30);
        SemearTransacao(pessoa.Id, "Salário", 5000m, TipoTransacao.Receita);
        SemearTransacao(pessoa.Id, "Aluguel", 1500m, TipoTransacao.Despesa);

        // Act: remove a pessoa.
        using (var contexto = CriarContexto())
        {
            await new PessoaService(contexto).RemoverAsync(pessoa.Id);
        }

        // Assert: pessoa e transações some por cascata.
        using var verificacao = CriarContexto();
        Assert.Empty(verificacao.Pessoas);
        Assert.Empty(verificacao.Transacoes);
    }
}
