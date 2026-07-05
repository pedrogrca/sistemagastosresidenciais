namespace GastosResidenciais.Api.Models;

/// <summary>
/// Usuário do sistema, utilizado para autenticação (registro e login).
/// Por segurança, a senha nunca é armazenada em texto puro — guardamos apenas
/// o seu hash em <see cref="SenhaHash"/>.
/// </summary>
public class Usuario
{
    public int Id { get; set; }

    /// <summary>Nome de exibição do usuário.</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>E-mail usado como login (único no sistema).</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Hash da senha (gerado pelo PasswordHasher).</summary>
    public string SenhaHash { get; set; } = string.Empty;
}
