namespace GastosResidenciais.Api.Models;

/// <summary>
/// Categoria de uma transação, usada para agrupar os gastos e ganhos
/// residenciais (útil nos gráficos e filtros do front-end).
/// </summary>
public enum CategoriaTransacao
{
    Moradia,
    Alimentacao,
    Transporte,
    Saude,
    Educacao,
    Lazer,
    Salario,
    Outros
}
