import { useState } from 'react'
import type { CategoriaTransacao, TipoTransacao, Transacao } from '../api/types'
import { agruparPorCategoria } from '../utils/categorias'
import { GraficoRosca } from './GraficoRosca'

interface Props {
  transacoes: Transacao[]
  /** Avisa o pai quando o filtro (tipo + categoria) muda — útil para filtrar uma lista. */
  onFiltro?: (tipo: TipoTransacao, categoria: CategoriaTransacao | null) => void
}

/**
 * Painel do gráfico por categoria: um toggle Despesas/Receitas + o gráfico de
 * rosca. Reutilizado no dashboard individual e no coletivo (Totais).
 */
export function PainelCategorias({ transacoes, onFiltro }: Props) {
  const [tipo, setTipo] = useState<TipoTransacao>('Despesa')
  const [selecionada, setSelecionada] = useState<CategoriaTransacao | null>(null)

  const fatias = agruparPorCategoria(transacoes, tipo)

  function trocarTipo(novo: TipoTransacao) {
    if (novo === tipo) return
    setTipo(novo)
    setSelecionada(null)
    onFiltro?.(novo, null)
  }

  function selecionar(categoria: CategoriaTransacao | null) {
    setSelecionada(categoria)
    onFiltro?.(tipo, categoria)
  }

  return (
    <div>
      <div className="mini-toggle">
        <button
          type="button"
          className={tipo === 'Despesa' ? 'ativo' : ''}
          onClick={() => trocarTipo('Despesa')}
        >
          Despesas
        </button>
        <button
          type="button"
          className={tipo === 'Receita' ? 'ativo' : ''}
          onClick={() => trocarTipo('Receita')}
        >
          Receitas
        </button>
      </div>

      {fatias.length === 0 ? (
        <p className="vazio">
          Nenhuma {tipo === 'Despesa' ? 'despesa' : 'receita'} para exibir.
        </p>
      ) : (
        <GraficoRosca
          fatias={fatias}
          selecionada={selecionada}
          onSelecionar={selecionar}
          rotuloCentro={tipo === 'Despesa' ? 'Despesas' : 'Receitas'}
        />
      )}
    </div>
  )
}
