namespace GastosResidenciais.Api.Models;

/// <summary>
/// Representa o tipo de uma transação financeira.
/// </summary>
public enum TipoTransacao
{
    /// <summary>Saída de dinheiro (gasto).</summary>
    Despesa = 0,

    /// <summary>Entrada de dinheiro (ganho).</summary>
    Receita = 1
}
