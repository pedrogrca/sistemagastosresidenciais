namespace GastosResidenciais.Api.Exceptions;

/// <summary>
/// Lançada quando um recurso solicitado não existe (ex.: remover uma pessoa
/// por um Id inexistente). É traduzida para HTTP 404 (Not Found) pelo handler global.
/// </summary>
public class NaoEncontradoException : Exception
{
    public NaoEncontradoException(string mensagem) : base(mensagem)
    {
    }
}
