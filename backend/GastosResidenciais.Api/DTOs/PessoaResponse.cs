namespace GastosResidenciais.Api.DTOs;

/// <summary>
/// Representação de uma pessoa retornada pela API.
/// Inclui o campo calculado <see cref="MenorDeIdade"/> para o front-end saber,
/// por exemplo, que só pode oferecer "despesa" ao cadastrar transações dessa pessoa.
/// </summary>
public class PessoaResponse
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Idade { get; set; }
    public bool MenorDeIdade { get; set; }
}
