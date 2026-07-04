using System.ComponentModel.DataAnnotations;

namespace GastosResidenciais.Api.DTOs;

/// <summary>
/// Dados que o cliente envia para cadastrar uma pessoa.
/// Note que NÃO recebemos o Id: ele é gerado automaticamente pelo banco.
/// As anotações (DataAnnotations) fazem a validação de formato automaticamente
/// e retornam HTTP 400 quando algo está inválido.
/// </summary>
public class CriarPessoaRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(150, MinimumLength = 1, ErrorMessage = "O nome deve ter entre 1 e 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Range(0, 130, ErrorMessage = "A idade deve estar entre 0 e 130 anos.")]
    public int Idade { get; set; }
}
