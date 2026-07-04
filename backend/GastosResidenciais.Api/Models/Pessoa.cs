using System.ComponentModel.DataAnnotations.Schema;

namespace GastosResidenciais.Api.Models;

/// <summary>
/// Pessoa cadastrada no sistema. Cada pessoa pode possuir várias transações.
/// </summary>
public class Pessoa
{
    /// <summary>
    /// Identificador único, gerado automaticamente pelo banco (auto-incremento).
    /// </summary>
    public int Id { get; set; }

    /// <summary>Nome da pessoa.</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>Idade em anos completos.</summary>
    public int Idade { get; set; }

    /// <summary>
    /// Transações vinculadas a esta pessoa (propriedade de navegação do EF Core).
    /// </summary>
    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();

    /// <summary>
    /// Regra de negócio: considera-se menor de idade quem tem menos de 18 anos.
    /// Marcada como [NotMapped] porque é um valor calculado — não é uma coluna do banco.
    /// </summary>
    [NotMapped]
    public bool EhMenorDeIdade => Idade < 18;
}
