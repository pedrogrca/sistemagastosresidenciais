/* Tipos TypeScript que espelham os DTOs do back-end. Manter esses tipos
   alinhados com a API dá autocompletar e checagem de tipos em todo o front-end. */

/** Tipo de uma transação (mesmos valores do enum do back-end). */
export type TipoTransacao = 'Despesa' | 'Receita'

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
  pessoaId: number
  pessoaNome: string
}

/** Dados enviados para cadastrar uma transação. */
export interface CriarTransacaoRequest {
  descricao: string
  valor: number
  tipo: TipoTransacao
  pessoaId: number
}

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
