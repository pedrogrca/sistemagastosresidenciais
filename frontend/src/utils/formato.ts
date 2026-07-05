// Formatador de moeda em Real brasileiro, criado uma única vez (reutilizável).
const formatadorMoeda = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
})

/** Formata um número como moeda brasileira. Ex.: 1500 -> "R$ 1.500,00". */
export function formatarMoeda(valor: number): string {
  return formatadorMoeda.format(valor)
}
