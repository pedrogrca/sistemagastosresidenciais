namespace GastosResidenciais.Api.Models;

/// <summary>
/// Transação financeira (uma receita ou uma despesa) pertencente a uma pessoa.
/// </summary>
public class Transacao
{
    /// <summary>
    /// Identificador único, gerado automaticamente pelo banco (auto-incremento).
    /// </summary>
    public int Id { get; set; }

    /// <summary>Descrição da transação (ex.: "Conta de luz", "Salário").</summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>Valor monetário da transação. Sempre positivo.</summary>
    public decimal Valor { get; set; }

    /// <summary>Indica se a transação é uma despesa ou uma receita.</summary>
    public TipoTransacao Tipo { get; set; }

    /// <summary>
    /// Chave estrangeira: identificador da pessoa dona da transação.
    /// </summary>
    public int PessoaId { get; set; }

    /// <summary>
    /// Propriedade de navegação para a pessoa dona da transação (EF Core).
    /// </summary>
    public Pessoa? Pessoa { get; set; }
}
