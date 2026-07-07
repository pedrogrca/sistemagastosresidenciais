using System.ComponentModel.DataAnnotations;
using GastosResidenciais.Api.Models;

namespace GastosResidenciais.Api.DTOs;

/// <summary>
/// Dados que o cliente envia para cadastrar uma transação.
///
/// Detalhe importante: <see cref="Tipo"/> e <see cref="PessoaId"/> são anuláveis
/// e marcados como [Required]. Se fossem tipos de valor "normais" (não anuláveis),
/// o cliente poderia omiti-los e o C# assumiria um padrão silencioso (0 / Despesa),
/// mascarando um erro. Sendo anuláveis, a ausência é corretamente rejeitada com 400.
/// </summary>
public class CriarTransacaoRequest
{
    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(250, MinimumLength = 1, ErrorMessage = "A descrição deve ter entre 1 e 250 caracteres.")]
    public string Descricao { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "O tipo é obrigatório (Despesa ou Receita).")]
    public TipoTransacao? Tipo { get; set; }

    [Required(ErrorMessage = "A categoria é obrigatória.")]
    public CategoriaTransacao? Categoria { get; set; }

    [Required(ErrorMessage = "A pessoa (PessoaId) é obrigatória.")]
    public int? PessoaId { get; set; }
}
