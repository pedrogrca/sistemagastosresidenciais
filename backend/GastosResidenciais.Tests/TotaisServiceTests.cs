using GastosResidenciais.Api.Models;
using GastosResidenciais.Api.Services;

namespace GastosResidenciais.Tests;

public class TotaisServiceTests : TesteComBanco
{
    [Fact]
    public async Task ObterAsync_DeveCalcularTotaisPorPessoaEGeral()
    {
        // Arrange: Ana (receita 5000, despesa 1500) e Bia (despesa 50).
        var ana = SemearPessoa("Ana", 30);
        var bia = SemearPessoa("Bia", 15);
        SemearTransacao(ana.Id, "Salário", 5000m, TipoTransacao.Receita);
        SemearTransacao(ana.Id, "Aluguel", 1500m, TipoTransacao.Despesa);
        SemearTransacao(bia.Id, "Lanche", 50m, TipoTransacao.Despesa);

        using var contexto = CriarContexto();
        var servico = new TotaisService(contexto);

        var totais = await servico.ObterAsync();

        // Totais da Ana.
        var totalAna = totais.Pessoas.Single(p => p.PessoaId == ana.Id);
        Assert.Equal(5000m, totalAna.TotalReceitas);
        Assert.Equal(1500m, totalAna.TotalDespesas);
        Assert.Equal(3500m, totalAna.Saldo);

        // Total geral (Ana + Bia).
        Assert.Equal(5000m, totais.TotalGeral.TotalReceitas);
        Assert.Equal(1550m, totais.TotalGeral.TotalDespesas);
        Assert.Equal(3450m, totais.TotalGeral.SaldoLiquido);
    }

    [Fact]
    public async Task ObterAsync_PessoaSemTransacoes_DeveAparecerZerada()
    {
        // O desafio pede listar TODAS as pessoas, mesmo sem transações.
        var carlos = SemearPessoa("Carlos", 40);

        using var contexto = CriarContexto();
        var servico = new TotaisService(contexto);

        var totais = await servico.ObterAsync();

        var totalCarlos = totais.Pessoas.Single(p => p.PessoaId == carlos.Id);
        Assert.Equal(0m, totalCarlos.TotalReceitas);
        Assert.Equal(0m, totalCarlos.TotalDespesas);
        Assert.Equal(0m, totalCarlos.Saldo);
    }
}
