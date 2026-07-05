import { useEffect, useState } from 'react'
import { totaisApi } from '../api/client'
import type { Totais } from '../api/types'
import { formatarMoeda } from '../utils/formato'

/**
 * Tela de consulta de totais: cartões de resumo geral no topo e, abaixo, a
 * tabela com os totais de cada pessoa (com o total geral no rodapé).
 */
export function TotaisPage() {
  const [dados, setDados] = useState<Totais | null>(null)
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState<string | null>(null)

  useEffect(() => {
    carregar()
  }, [])

  async function carregar() {
    setCarregando(true)
    setErro(null)
    try {
      setDados(await totaisApi.obter())
    } catch (e) {
      setErro((e as Error).message)
    } finally {
      setCarregando(false)
    }
  }

  // Saldo positivo aparece em verde; negativo, em vermelho.
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
