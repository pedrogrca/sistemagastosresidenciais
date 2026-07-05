import { useState } from 'react'
import { PessoasPage } from './pages/PessoasPage'
import { TransacoesPage } from './pages/TransacoesPage'
import { TotaisPage } from './pages/TotaisPage'

/** Abas disponíveis na aplicação. */
type Aba = 'pessoas' | 'transacoes' | 'totais'

/**
 * Componente raiz: barra superior com a marca e a navegação por abas.
 * A navegação usa estado local (useState), sem biblioteca de rotas — suficiente
 * e simples para três telas.
 */
function App() {
  const [abaAtiva, setAbaAtiva] = useState<Aba>('pessoas')

  return (
    <div className="app-layout">
      <header className="topbar">
        <div className="topbar-conteudo">
          <div className="marca">
            <span>💰</span>
            <span>Gastos Residenciais</span>
          </div>
        </div>
      </header>

      <main className="conteudo">
        <nav className="abas">
          <button
            className={abaAtiva === 'pessoas' ? 'aba ativa' : 'aba'}
            onClick={() => setAbaAtiva('pessoas')}
          >
            Pessoas
          </button>
          <button
            className={abaAtiva === 'transacoes' ? 'aba ativa' : 'aba'}
            onClick={() => setAbaAtiva('transacoes')}
          >
            Transações
          </button>
          <button
            className={abaAtiva === 'totais' ? 'aba ativa' : 'aba'}
            onClick={() => setAbaAtiva('totais')}
          >
            Totais
          </button>
        </nav>

        {abaAtiva === 'pessoas' && <PessoasPage />}
        {abaAtiva === 'transacoes' && <TransacoesPage />}
        {abaAtiva === 'totais' && <TotaisPage />}
      </main>
    </div>
  )
}

export default App
