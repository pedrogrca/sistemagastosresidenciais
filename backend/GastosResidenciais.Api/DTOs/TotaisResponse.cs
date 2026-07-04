namespace GastosResidenciais.Api.DTOs;

/// <summary>
/// Resposta completa da consulta de totais: a lista por pessoa e o total geral.
/// </summary>
public class TotaisResponse
{
    /// <summary>Totais de cada pessoa cadastrada (inclusive quem não tem transações).</summary>
    public IEnumerable<TotalPorPessoaResponse> Pessoas { get; set; } = new List<TotalPorPessoaResponse>();

    /// <summary>Totais somando todas as pessoas.</summary>
    public TotalGeralResponse TotalGeral { get; set; } = new();
}
