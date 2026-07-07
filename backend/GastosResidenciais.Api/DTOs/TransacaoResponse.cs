using GastosResidenciais.Api.Models;

namespace GastosResidenciais.Api.DTOs;

/// <summary>
/// Representação de uma transação retornada pela API.
/// Inclui o nome da pessoa (<see cref="PessoaNome"/>) por conveniência,
/// evitando que o front-end precise cruzar os dados manualmente na listagem.
/// </summary>
public class TransacaoResponse
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public TipoTransacao Tipo { get; set; }
    public CategoriaTransacao Categoria { get; set; }
    public int PessoaId { get; set; }
    public string PessoaNome { get; set; } = string.Empty;
}
