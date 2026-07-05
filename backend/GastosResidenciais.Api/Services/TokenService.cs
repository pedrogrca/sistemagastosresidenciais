using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GastosResidenciais.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace GastosResidenciais.Api.Services;

/// <summary>
/// Gera tokens JWT assinados com uma chave secreta (algoritmo HMAC-SHA256).
/// O token guarda a identidade do usuário e é validado a cada requisição
/// pelo middleware de autenticação.
/// </summary>
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GerarToken(Usuario usuario)
    {
        var chaveSecreta = _configuration["Jwt:Chave"]
            ?? throw new InvalidOperationException("A chave JWT (Jwt:Chave) não está configurada.");

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveSecreta));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        // Claims: informações que ficam "dentro" do token.
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim("nome", usuario.Nome),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var horas = int.TryParse(_configuration["Jwt:ExpiracaoHoras"], out var h) ? h : 8;

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Emissor"],
            audience: _configuration["Jwt:Audiencia"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(horas),
            signingCredentials: credenciais);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
