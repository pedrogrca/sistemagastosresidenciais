namespace GastosResidenciais.Api.Exceptions;

/// <summary>
/// Lançada quando as credenciais informadas são inválidas (login).
/// É traduzida para HTTP 401 (Unauthorized) pelo handler global.
/// </summary>
public class NaoAutorizadoException : Exception
{
    public NaoAutorizadoException(string mensagem) : base(mensagem)
    {
    }
}
