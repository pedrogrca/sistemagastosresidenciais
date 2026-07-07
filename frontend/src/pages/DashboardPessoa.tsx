import { useEffect, useState } from 'react'
import { transacoesApi } from '../api/client'
import type {
  CategoriaTransacao,
  Pessoa,
  TipoTransacao,
  Transacao,
} from '../api/types'
import { formatarMoeda } from '../utils/formato'
import { CATEGORIAS } from '../utils/categorias'
import { PainelCategorias } from '../components/PainelCategorias'

interface Props {
  pessoa: Pessoa
  onVoltar: () => void
}

/**
 * Dashboard individual de uma pessoa: totais, gráfico por categoria e a lista
 * das transações dela (com exclusão). Ao selecionar uma categoria no gráfico,
 * a lista abaixo é filtrada por aquela categoria/tipo.
 */
export function DashboardPessoa({ pessoa, onVoltar }: Props) {
  const [transacoes, setTransacoes] = useState<Transacao[]>([])
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState<string | null>(null)

  // Filtro vindo do gráfico (clicar numa categoria filtra a lista abaixo).
  const [filtroTipo, setFiltroTipo] = useState<TipoTransacao>('Despesa')
  const [filtroCategoria, setFiltroCategoria] = useState<CategoriaTransacao | null>(null)

  useEffect(() => {
    carregar()
  }, [pessoa.id])

  async function carregar() {
    setCarregando(true)
    setErro(null)
    try {
      const todas = await transacoesApi.listar()
      setTransacoes(todas.filter((t) => t.pessoaId === pessoa.id))
    } catch (e) {
      setErro((e as Error).message)
    } finally {
      setCarregando(false)
    }
  }

  async function handleRemover(t: Transacao) {
    if (!window.confirm(`Excluir a transação "${t.descricao}"?`)) return
    setErro(null)
    try {
      await transacoesApi.remover(t.id)
      await carregar()
    } catch (e) {
      setErro((e as Error).message)
    }
  }

  const receitas = transacoes
    .filter((t) => t.tipo === 'Receita')
    .reduce((s, t) => s + t.valor, 0)
  const despesas = transacoes
    .filter((t) => t.tipo === 'Despesa')
    .reduce((s, t) => s + t.valor, 0)
  const saldo = receitas - despesas

  const listaFiltrada = filtroCategoria
    ? transacoes.filter((t) => t.tipo === filtroTipo && t.categoria === filtroCategoria)
    : transacoes

  return (
    <>
      {erro && <div className="alerta-erro">{erro}</div>}

      <div className="dash-topo">
        <button className="btn btn-ghost" onClick={onVoltar}>
          ← Voltar
        </button>
        <h2 className="dash-nome">
          {pessoa.nome}
          {pessoa.menorDeIdade && (
            <span className="selo selo-menor" style={{ marginLeft: 8 }}>
              menor de idade
            </span>
          )}
        </h2>
      </div>

      <div className="stats">
        <div className="stat">
          <span>Receitas</span>
          <strong className="valor-positivo">{formatarMoeda(receitas)}</strong>
        </div>
        <div className="stat">
          <span>Despesas</span>
          <strong className="valor-negativo">{formatarMoeda(despesas)}</strong>
        </div>
        <div className="stat">
          <span>Saldo</span>
          <strong className={saldo >= 0 ? 'valor-positivo' : 'valor-negativo'}>
            {formatarMoeda(saldo)}
          </strong>
        </div>
      </div>

      <div className="card">
        <h2>Distribuição por categoria</h2>
        {carregando ? (
          <p className="vazio">Carregando...</p>
        ) : (
          <PainelCategorias
            transacoes={transacoes}
            onFiltro={(tipo, categoria) => {
              setFiltroTipo(tipo)
              setFiltroCategoria(categoria)
            }}
          />
        )}
      </div>

      <div className="card">
        <h2>
          Transações
          {filtroCategoria &&
            ` · ${filtroTipo === 'Despesa' ? 'Despesas' : 'Receitas'} de ${CATEGORIAS[filtroCategoria].rotulo}`}
        </h2>
        {carregando ? (
          <p className="vazio">Carregando...</p>
        ) : listaFiltrada.length === 0 ? (
          <p className="vazio">
            Nenhuma transação{filtroCategoria ? ' nessa categoria' : ' cadastrada'}.
          </p>
        ) : (
          <div className="tabela-wrap">
            <table className="tabela">
              <thead>
                <tr>
                  <th>Descrição</th>
                  <th>Categoria</th>
                  <th>Tipo</th>
                  <th className="num">Valor</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {listaFiltrada.map((t) => (
                  <tr key={t.id}>
                    <td>{t.descricao}</td>
                    <td>
                      <span
                        className="cat-dot"
                        style={{ background: CATEGORIAS[t.categoria].cor }}
                      />
                      {CATEGORIAS[t.categoria].rotulo}
                    </td>
                    <td>
                      <span
                        className={
                          t.tipo === 'Receita'
                            ? 'selo selo-receita'
                            : 'selo selo-despesa'
                        }
                      >
                        {t.tipo}
                      </span>
                    </td>
                    <td
                      className={
                        t.tipo === 'Receita'
                          ? 'num valor-positivo'
                          : 'num valor-negativo'
                      }
                    >
                      {t.tipo === 'Receita' ? '+ ' : '− '}
                      {formatarMoeda(t.valor)}
                    </td>
                    <td className="num">
                      <button
                        className="btn btn-perigo"
                        onClick={() => handleRemover(t)}
                      >
                        Excluir
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </>
  )
}
