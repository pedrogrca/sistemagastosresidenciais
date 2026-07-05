using GastosResidenciais.Api.Models;

namespace GastosResidenciais.Api.Services;

/// <summary>Contrato do serviço que gera tokens JWT.</summary>
public interface ITokenService
{
    /// <summary>Gera um token JWT assinado para o usuário informado.</summary>
    string GerarToken(Usuario usuario);
}
