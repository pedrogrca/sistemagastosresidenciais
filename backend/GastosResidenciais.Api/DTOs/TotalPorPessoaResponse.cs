namespace GastosResidenciais.Api.DTOs;

/// <summary>
/// Totais consolidados de uma pessoa: quanto recebeu, quanto gastou e o saldo.
/// </summary>
public class TotalPorPessoaResponse
{
    public int PessoaId { get; set; }
    public string Nome { get; set; } = string.Empty;

    /// <summary>Soma de todas as receitas da pessoa.</summary>
    public decimal TotalReceitas { get; set; }

    /// <summary>Soma de todas as despesas da pessoa.</summary>
    public decimal TotalDespesas { get; set; }

    /// <summary>Saldo da pessoa (receitas − despesas). Pode ser negativo.</summary>
    public decimal Saldo { get; set; }
}
