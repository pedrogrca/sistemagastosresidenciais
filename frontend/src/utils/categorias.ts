import type { CategoriaTransacao, TipoTransacao, Transacao } from '../api/types'

/**
 * Metadados de exibição de cada categoria: o rótulo (com acento) e a cor
 * usada nos selos e no gráfico. Concentrar isso aqui evita repetição e mantém
 * as cores consistentes em toda a interface.
 */
export const CATEGORIAS: Record<CategoriaTransacao, { rotulo: string; cor: string }> = {
  Moradia: { rotulo: 'Moradia', cor: '#4f46e5' },
  Alimentacao: { rotulo: 'Alimentação', cor: '#059669' },
  Transporte: { rotulo: 'Transporte', cor: '#0891b2' },
  Saude: { rotulo: 'Saúde', cor: '#e11d48' },
  Educacao: { rotulo: 'Educação', cor: '#d97706' },
  Lazer: { rotulo: 'Lazer', cor: '#db2777' },
  Salario: { rotulo: 'Salário', cor: '#16a34a' },
  Outros: { rotulo: 'Outros', cor: '#64748b' },
}

/** Lista das categorias na ordem de exibição (ex.: no seletor do formulário). */
export const LISTA_CATEGORIAS = Object.keys(CATEGORIAS) as CategoriaTransacao[]

/** Uma "fatia" do gráfico: uma categoria com seu total e cor. */
export interface FatiaCategoria {
  chave: CategoriaTransacao
  rotulo: string
  valor: number
  cor: string
}

/**
 * Soma os valores das transações de um determinado tipo (Despesa/Receita),
 * agrupados por categoria. Retorna só as categorias que têm algum valor,
 * na ordem de LISTA_CATEGORIAS — pronto para alimentar o gráfico.
 */
export function agruparPorCategoria(
  transacoes: Transacao[],
  tipo: TipoTransacao,
): FatiaCategoria[] {
  const somas = new Map<CategoriaTransacao, number>()
  for (const t of transacoes) {
    if (t.tipo !== tipo) continue
    somas.set(t.categoria, (somas.get(t.categoria) ?? 0) + t.valor)
  }

  return LISTA_CATEGORIAS.filter((c) => somas.has(c)).map((c) => ({
    chave: c,
    rotulo: CATEGORIAS[c].rotulo,
    valor: somas.get(c)!,
    cor: CATEGORIAS[c].cor,
  }))
}
