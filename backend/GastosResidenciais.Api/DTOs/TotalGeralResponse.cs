namespace GastosResidenciais.Api.DTOs;

/// <summary>
/// Totais somando TODAS as pessoas cadastradas.
/// </summary>
public class TotalGeralResponse
{
    /// <summary>Soma das receitas de todas as pessoas.</summary>
    public decimal TotalReceitas { get; set; }

    /// <summary>Soma das despesas de todas as pessoas.</summary>
    public decimal TotalDespesas { get; set; }

    /// <summary>Saldo líquido geral (total de receitas − total de despesas).</summary>
    public decimal SaldoLiquido { get; set; }
}
