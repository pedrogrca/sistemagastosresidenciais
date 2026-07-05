using GastosResidenciais.Api.Data;
using GastosResidenciais.Api.DTOs;
using GastosResidenciais.Api.Exceptions;
using GastosResidenciais.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.Api.Services;

/// <summary>
/// Regras de autenticação: registro de novos usuários e login.
/// </summary>
public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public AuthService(
        AppDbContext context,
        ITokenService tokenService,
        IPasswordHasher<Usuario> passwordHasher)
    {
        _context = context;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResponse> RegistrarAsync(RegistrarRequest request)
    {
        // Normaliza o e-mail (sem espaços e em minúsculas) para comparações consistentes.
        var email = request.Email.Trim().ToLowerInvariant();

        if (await _context.Usuarios.AnyAsync(u => u.Email == email))
        {
            throw new RegraNegocioException("Já existe um usuário cadastrado com este e-mail.");
        }

        var usuario = new Usuario
        {
            Nome = request.Nome.Trim(),
            Email = email
        };
        // Armazena apenas o hash da senha (nunca o texto puro).
        usuario.SenhaHash = _passwordHasher.HashPassword(usuario, request.Senha);

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return CriarResposta(usuario);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

        // A mensagem é intencionalmente genérica: não revelamos se o erro foi no
        // e-mail ou na senha (boa prática de segurança contra enumeração de contas).
        if (usuario is null)
        {
            throw new NaoAutorizadoException("E-mail ou senha inválidos.");
        }

        var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHash, request.Senha);
        if (resultado == PasswordVerificationResult.Failed)
        {
            throw new NaoAutorizadoException("E-mail ou senha inválidos.");
        }

        return CriarResposta(usuario);
    }

    private AuthResponse CriarResposta(Usuario usuario) => new()
    {
        Token = _tokenService.GerarToken(usuario),
        Nome = usuario.Nome,
        Email = usuario.Email
    };
}
