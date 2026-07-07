/* Tipos TypeScript que espelham os DTOs do back-end. Manter esses tipos
   alinhados com a API dá autocompletar e checagem de tipos em todo o front-end. */

/** Tipo de uma transação (mesmos valores do enum do back-end). */
export type TipoTransacao = 'Despesa' | 'Receita'

/** Categoria de uma transação (mesmos valores do enum do back-end). */
export type CategoriaTransacao =
  | 'Moradia'
  | 'Alimentacao'
  | 'Transporte'
  | 'Saude'
  | 'Educacao'
  | 'Lazer'
  | 'Salario'
  | 'Outros'

/** Pessoa retornada pela API. */
export interface Pessoa {
  id: number
  nome: string
  idade: number
  menorDeIdade: boolean
}

/** Dados enviados para cadastrar uma pessoa. */
export interface CriarPessoaRequest {
  nome: string
  idade: number
}

/** Transação retornada pela API. */
export interface Transacao {
  id: number
  descricao: string
  valor: number
  tipo: TipoTransacao
  categoria: CategoriaTransacao
  pessoaId: number
  pessoaNome: string
}

/** Dados enviados para cadastrar uma transação. */
export interface CriarTransacaoRequest {
  descricao: string
  valor: number
  tipo: TipoTransacao
  categoria: CategoriaTransacao
  pessoaId: number
}

/* ---------- Totais ---------- */

/** Totais consolidados de uma pessoa. */
export interface TotalPorPessoa {
  pessoaId: number
  nome: string
  totalReceitas: number
  totalDespesas: number
  saldo: number
}

/** Totais somando todas as pessoas. */
export interface TotalGeral {
  totalReceitas: number
  totalDespesas: number
  saldoLiquido: number
}

/** Resposta completa da consulta de totais. */
export interface Totais {
  pessoas: TotalPorPessoa[]
  totalGeral: TotalGeral
}
