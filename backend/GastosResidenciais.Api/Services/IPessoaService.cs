using GastosResidenciais.Api.DTOs;

namespace GastosResidenciais.Api.Services;

/// <summary>
/// Contrato do serviço de pessoas. Trabalhar contra uma interface facilita
/// testes e a substituição da implementação (princípio de inversão de dependência).
/// </summary>
public interface IPessoaService
{
    /// <summary>Cadastra uma nova pessoa e retorna seus dados (já com o Id gerado).</summary>
    Task<PessoaResponse> CriarAsync(CriarPessoaRequest request);

    /// <summary>Lista todas as pessoas cadastradas.</summary>
    Task<IEnumerable<PessoaResponse>> ListarAsync();

    /// <summary>Remove uma pessoa e, por cascata, todas as transações dela.</summary>
    Task RemoverAsync(int id);
}
