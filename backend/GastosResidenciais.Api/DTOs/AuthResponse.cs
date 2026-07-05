namespace GastosResidenciais.Api.DTOs;

/// <summary>
/// Retorno do registro/login: o token JWT (usado nas próximas requisições)
/// e os dados básicos do usuário autenticado.
/// </summary>
public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
