using GastosResidenciais.Api.DTOs;

namespace GastosResidenciais.Api.Services;

/// <summary>
/// Contrato do serviço de transações.
/// O desafio pede apenas criação e listagem (sem edição/exclusão).
/// </summary>
public interface ITransacaoService
{
    /// <summary>
    /// Cadastra uma nova transação, aplicando as regras de negócio:
    /// a pessoa precisa existir e menores de idade só podem registrar despesas.
    /// </summary>
    Task<TransacaoResponse> CriarAsync(CriarTransacaoRequest request);

    /// <summary>Lista todas as transações cadastradas.</summary>
    Task<IEnumerable<TransacaoResponse>> ListarAsync();
}
