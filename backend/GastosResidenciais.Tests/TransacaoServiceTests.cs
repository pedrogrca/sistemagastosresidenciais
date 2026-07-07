using GastosResidenciais.Api.DTOs;
using GastosResidenciais.Api.Exceptions;
using GastosResidenciais.Api.Models;
using GastosResidenciais.Api.Services;

namespace GastosResidenciais.Tests;

public class TransacaoServiceTests : TesteComBanco
{
    [Fact]
    public async Task CriarAsync_AdultoComReceita_DeveCadastrar()
    {
        var ana = SemearPessoa("Ana", 30);

        using var contexto = CriarContexto();
        var servico = new TransacaoService(contexto);

        var resposta = await servico.CriarAsync(new CriarTransacaoRequest
        {
            Descricao = "Salário",
            Valor = 5000m,
            Tipo = TipoTransacao.Receita,
            Categoria = CategoriaTransacao.Salario,
            PessoaId = ana.Id
        });

        Assert.True(resposta.Id > 0);
        Assert.Equal(TipoTransacao.Receita, resposta.Tipo);
        Assert.Equal(CategoriaTransacao.Salario, resposta.Categoria);
        Assert.Equal("Ana", resposta.PessoaNome);
    }

    [Fact]
    public async Task CriarAsync_MenorComDespesa_DeveCadastrar()
    {
        var bia = SemearPessoa("Bia", 15);

        using var contexto = CriarContexto();
        var servico = new TransacaoService(contexto);

        var resposta = await servico.CriarAsync(new CriarTransacaoRequest
        {
            Descricao = "Lanche",
            Valor = 50m,
            Tipo = TipoTransacao.Despesa,
            Categoria = CategoriaTransacao.Alimentacao,
            PessoaId = bia.Id
        });

        Assert.Equal(TipoTransacao.Despesa, resposta.Tipo);
        Assert.Equal(CategoriaTransacao.Alimentacao, resposta.Categoria);
    }

    [Fact]
    public async Task CriarAsync_MenorComReceita_DeveLancarRegraNegocio()
    {
        // Regra central do desafio: menor de idade não pode cadastrar receita.
        var bia = SemearPessoa("Bia", 15);

        using var contexto = CriarContexto();
        var servico = new TransacaoService(contexto);

        await Assert.ThrowsAsync<RegraNegocioException>(() => servico.CriarAsync(new CriarTransacaoRequest
        {
            Descricao = "Mesada",
            Valor = 200m,
            Tipo = TipoTransacao.Receita,
            Categoria = CategoriaTransacao.Outros,
            PessoaId = bia.Id
        }));
    }

    [Fact]
    public async Task CriarAsync_PessoaInexistente_DeveLancarNaoEncontrado()
    {
        using var contexto = CriarContexto();
        var servico = new TransacaoService(contexto);

        await Assert.ThrowsAsync<NaoEncontradoException>(() => servico.CriarAsync(new CriarTransacaoRequest
        {
            Descricao = "Qualquer",
            Valor = 10m,
            Tipo = TipoTransacao.Despesa,
            Categoria = CategoriaTransacao.Outros,
            PessoaId = 999
        }));
    }

    [Fact]
    public async Task ListarAsync_DeveIncluirNomeDaPessoa()
    {
        var ana = SemearPessoa("Ana", 30);
        SemearTransacao(ana.Id, "Salário", 5000m, TipoTransacao.Receita);

        using var contexto = CriarContexto();
        var servico = new TransacaoService(contexto);

        var transacoes = (await servico.ListarAsync()).ToList();

        Assert.Single(transacoes);
        Assert.Equal("Ana", transacoes[0].PessoaNome);
    }

    [Fact]
    public async Task RemoverAsync_DeveExcluirApenasATransacao_MantendoAPessoa()
    {
        var ana = SemearPessoa("Ana", 30);
        SemearTransacao(ana.Id, "Salário", 5000m, TipoTransacao.Receita);

        int transacaoId;
        using (var ctx = CriarContexto())
        {
            transacaoId = ctx.Transacoes.Single().Id;
        }

        using (var contexto = CriarContexto())
        {
            await new TransacaoService(contexto).RemoverAsync(transacaoId);
        }

        using var verificacao = CriarContexto();
        Assert.Empty(verificacao.Transacoes); // transação removida
        Assert.Single(verificacao.Pessoas);   // a pessoa permanece
    }

    [Fact]
    public async Task RemoverAsync_TransacaoInexistente_DeveLancarNaoEncontrado()
    {
        using var contexto = CriarContexto();
        var servico = new TransacaoService(contexto);

        await Assert.ThrowsAsync<NaoEncontradoException>(() => servico.RemoverAsync(999));
    }
}
