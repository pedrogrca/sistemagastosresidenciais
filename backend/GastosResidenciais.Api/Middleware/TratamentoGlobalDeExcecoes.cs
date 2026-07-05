using GastosResidenciais.Api.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GastosResidenciais.Api.Middleware;

/// <summary>
/// Handler global de exceções (recurso nativo do .NET 8: IExceptionHandler).
/// Captura exceções não tratadas e as converte em respostas HTTP padronizadas
/// (formato ProblemDetails), mantendo os controllers livres de blocos try/catch.
/// </summary>
public class TratamentoGlobalDeExcecoes : IExceptionHandler
{
    private readonly ILogger<TratamentoGlobalDeExcecoes> _logger;

    public TratamentoGlobalDeExcecoes(ILogger<TratamentoGlobalDeExcecoes> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Cada tipo de exceção vira um status HTTP apropriado.
        var (statusCode, titulo) = exception switch
        {
            NaoEncontradoException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
            RegraNegocioException => (StatusCodes.Status400BadRequest, "Regra de negócio violada"),
            _ => (StatusCodes.Status500InternalServerError, "Erro interno do servidor")
        };

        // Erros inesperados (500) são registrados no log; os demais são esperados.
        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Erro não tratado ao processar a requisição.");
        }

        var problema = new ProblemDetails
        {
            Status = statusCode,
            Title = titulo,
            // Não expomos detalhes internos em erros 500 (evita vazar informação sensível).
            Detail = statusCode == StatusCodes.Status500InternalServerError
                ? "Ocorreu um erro inesperado. Tente novamente."
                : exception.Message
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problema, cancellationToken);

        // true = a exceção foi tratada; o pipeline não precisa fazer mais nada.
        return true;
    }
}
