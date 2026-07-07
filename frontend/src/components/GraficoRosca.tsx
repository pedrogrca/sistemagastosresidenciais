import type { CategoriaTransacao } from '../api/types'
import type { FatiaCategoria } from '../utils/categorias'
import { formatarMoeda } from '../utils/formato'

interface Props {
  fatias: FatiaCategoria[]
  selecionada: CategoriaTransacao | null
  onSelecionar: (chave: CategoriaTransacao | null) => void
  rotuloCentro: string
}

// Dimensões do desenho (em unidades do viewBox).
const RAIO = 70
const ESPESSURA = 32
const CIRCUNFERENCIA = 2 * Math.PI * RAIO

/**
 * Gráfico de rosca (donut) desenhado em SVG puro, sem biblioteca externa.
 *
 * Técnica: cada categoria é o MESMO círculo, mas com um "traço" (stroke) que
 * cobre só a fração correspondente ao seu valor (via stroke-dasharray) e é
 * deslocado para começar onde o anterior terminou (stroke-dashoffset).
 * Clicar numa fatia ou na legenda seleciona a categoria (destaque + filtro).
 */
export function GraficoRosca({ fatias, selecionada, onSelecionar, rotuloCentro }: Props) {
  const total = fatias.reduce((soma, f) => soma + f.valor, 0)

  if (total <= 0) {
    return <p className="vazio">Sem dados para exibir.</p>
  }

  // Pré-calcula o comprimento e o deslocamento de cada fatia.
  let acumulado = 0
  const segmentos = fatias.map((f) => {
    const comprimento = (f.valor / total) * CIRCUNFERENCIA
    const segmento = { ...f, comprimento, offset: -acumulado }
    acumulado += comprimento
    return segmento
  })

  const destaque = fatias.find((f) => f.chave === selecionada) ?? null

  return (
    <div className="grafico-wrap">
      <svg viewBox="0 0 200 200" className="grafico-svg" role="img" aria-label="Distribuição por categoria">
        {/* rotate(-90) faz a primeira fatia começar no topo. */}
        <g transform="rotate(-90 100 100)">
          {segmentos.map((s) => (
            <circle
              key={s.chave}
              cx={100}
              cy={100}
              r={RAIO}
              fill="none"
              stroke={s.cor}
              strokeWidth={ESPESSURA}
              strokeDasharray={`${s.comprimento} ${CIRCUNFERENCIA - s.comprimento}`}
              strokeDashoffset={s.offset}
              opacity={selecionada && selecionada !== s.chave ? 0.25 : 1}
              style={{ cursor: 'pointer', transition: 'opacity 0.15s' }}
              onClick={() => onSelecionar(selecionada === s.chave ? null : s.chave)}
            />
          ))}
        </g>

        {/* Texto no centro da rosca (total ou a categoria selecionada). */}
        <text x="100" y="94" textAnchor="middle" className="grafico-centro-rotulo">
          {destaque ? destaque.rotulo : rotuloCentro}
        </text>
        <text x="100" y="115" textAnchor="middle" className="grafico-centro-valor">
          {formatarMoeda(destaque ? destaque.valor : total)}
        </text>
      </svg>

      <ul className="legenda">
        {fatias.map((f) => {
          const percentual = ((f.valor / total) * 100).toFixed(0)
          const ativo = selecionada === f.chave
          return (
            <li key={f.chave}>
              <button
                type="button"
                className={ativo ? 'legenda-item ativo' : 'legenda-item'}
                onClick={() => onSelecionar(ativo ? null : f.chave)}
              >
                <span className="cat-dot" style={{ background: f.cor }} />
                <span className="legenda-rotulo">{f.rotulo}</span>
                <span className="legenda-valor">
                  {formatarMoeda(f.valor)} · {percentual}%
                </span>
              </button>
            </li>
          )
        })}
      </ul>
    </div>
  )
}
