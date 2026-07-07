import { useEffect, useState } from 'react'
import { totaisApi, transacoesApi } from '../api/client'
import type { Totais, Transacao } from '../api/types'
import { formatarMoeda } from '../utils/formato'
import { PainelCategorias } from '../components/PainelCategorias'

/**
 * Tela de consulta de totais (dashboard coletivo): cartões de resumo, gráfico
 * por categoria de todas as pessoas, e a tabela com os totais de cada pessoa.
 */
export function TotaisPage() {
  const [dados, setDados] = useState<Totais | null>(null)
  const [transacoes, setTransacoes] = useState<Transacao[]>([])
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState<string | null>(null)

  useEffect(() => {
    carregar()
  }, [])

  async function carregar() {
    setCarregando(true)
    setErro(null)
    try {
      // Totais (para os cartões/tabela) e transações (para o gráfico) em paralelo.
      const [totais, listaTransacoes] = await Promise.all([
        totaisApi.obter(),
        transacoesApi.listar(),
      ])
      setDados(totais)
      setTransacoes(listaTransacoes)
    } catch (e) {
      setErro((e as Error).message)
    } finally {
      setCarregando(false)
    }
  }

  function classeSaldo(valor: number) {
    return valor >= 0 ? 'num valor-positivo' : 'num valor-negativo'
  }

  if (erro) {
    return <div className="alerta-erro">{erro}</div>
  }

  if (carregando || !dados) {
    return (
      <div className="card">
        <p className="vazio">Carregando...</p>
      </div>
    )
  }

  const { pessoas, totalGeral } = dados

  return (
    <>
      <div className="stats">
        <div className="stat">
          <span>Total de receitas</span>
          <strong className="valor-positivo">
            {formatarMoeda(totalGeral.totalReceitas)}
          </strong>
        </div>
        <div className="stat">
          <span>Total de despesas</span>
          <strong className="valor-negativo">
            {formatarMoeda(totalGeral.totalDespesas)}
          </strong>
        </div>
        <div className="stat">
          <span>Saldo líquido</span>
          <strong
            className={
              totalGeral.saldoLiquido >= 0 ? 'valor-positivo' : 'valor-negativo'
            }
          >
            {formatarMoeda(totalGeral.saldoLiquido)}
          </strong>
        </div>
      </div>

      <div className="card">
        <h2>Distribuição por categoria (todos)</h2>
        {transacoes.length === 0 ? (
          <p className="vazio">Nenhuma transação cadastrada ainda.</p>
        ) : (
          <PainelCategorias transacoes={transacoes} />
        )}
      </div>

      <div className="card">
        <h2>Totais por pessoa</h2>
        {pessoas.length === 0 ? (
          <p className="vazio">Nenhuma pessoa cadastrada ainda.</p>
        ) : (
          <div className="tabela-wrap">
            <table className="tabela">
              <thead>
                <tr>
                  <th>Pessoa</th>
                  <th className="num">Receitas</th>
                  <th className="num">Despesas</th>
                  <th className="num">Saldo</th>
                </tr>
              </thead>
              <tbody>
                {pessoas.map((p) => (
                  <tr key={p.pessoaId}>
                    <td>{p.nome}</td>
                    <td className="num valor-positivo">
                      {formatarMoeda(p.totalReceitas)}
                    </td>
                    <td className="num valor-negativo">
                      {formatarMoeda(p.totalDespesas)}
                    </td>
                    <td className={classeSaldo(p.saldo)}>
                      {formatarMoeda(p.saldo)}
                    </td>
                  </tr>
                ))}
              </tbody>
              <tfoot>
                <tr>
                  <td>Total geral</td>
                  <td className="num valor-positivo">
                    {formatarMoeda(totalGeral.totalReceitas)}
                  </td>
                  <td className="num valor-negativo">
                    {formatarMoeda(totalGeral.totalDespesas)}
                  </td>
                  <td className={classeSaldo(totalGeral.saldoLiquido)}>
                    {formatarMoeda(totalGeral.saldoLiquido)}
                  </td>
                </tr>
              </tfoot>
            </table>
          </div>
        )}
      </div>
    </>
  )
}
