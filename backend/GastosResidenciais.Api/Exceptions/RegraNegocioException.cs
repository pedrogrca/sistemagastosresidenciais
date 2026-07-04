namespace GastosResidenciais.Api.Exceptions;

/// <summary>
/// Lançada quando uma regra de negócio é violada (ex.: menor de idade tentando
/// cadastrar uma receita). É traduzida para HTTP 400 (Bad Request) pelo handler global.
/// </summary>
public class RegraNegocioException : Exception
{
    public RegraNegocioException(string mensagem) : base(mensagem)
    {
    }
}
