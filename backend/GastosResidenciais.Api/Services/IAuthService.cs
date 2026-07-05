using GastosResidenciais.Api.DTOs;

namespace GastosResidenciais.Api.Services;

/// <summary>Contrato do serviço de autenticação (registro e login).</summary>
public interface IAuthService
{
    /// <summary>Registra um novo usuário e retorna um token para acesso imediato.</summary>
    Task<AuthResponse> RegistrarAsync(RegistrarRequest request);

    /// <summary>Autentica um usuário existente e retorna o token JWT.</summary>
    Task<AuthResponse> LoginAsync(LoginRequest request);
}
