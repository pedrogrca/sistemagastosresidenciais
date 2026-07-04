using GastosResidenciais.Api.DTOs;

namespace GastosResidenciais.Api.Services;

/// <summary>
/// Contrato do serviço de consulta de totais.
/// </summary>
public interface ITotaisService
{
    /// <summary>
    /// Calcula os totais (receitas, despesas e saldo) de cada pessoa e o total geral.
    /// </summary>
    Task<TotaisResponse> ObterAsync();
}
