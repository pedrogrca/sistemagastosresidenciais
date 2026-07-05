import { useState } from 'react'
import { PessoasPage } from './pages/PessoasPage'

/** Abas disponíveis na aplicação. */
type Aba = 'pessoas' | 'transacoes' | 'totais'

/**
 * Componente raiz. Controla qual aba está ativa e renderiza o conteúdo
 * correspondente. A navegação é feita com estado local (useState), sem
 * biblioteca de rotas — suficiente e simples para três telas.
 */
function App() {
  const [abaAtiva, setAbaAtiva] = useState<Aba>('pessoas')

  return (
    <div className="app">
      <header className="cabecalho">
        <h1>Controle de Gastos Residenciais</h1>
        <p>Gerencie pessoas, transações e acompanhe os totais</p>
      </header>

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

      <main className="conteudo">
        {/* As telas reais serão adicionadas nas próximas etapas. */}
        {abaAtiva === 'pessoas' && <PessoasPage />}
        {abaAtiva === 'transacoes' && (
          <div className="card">
            <p className="vazio">Em breve: cadastro de transações.</p>
          </div>
        )}
        {abaAtiva === 'totais' && (
          <div className="card">
            <p className="vazio">Em breve: consulta de totais.</p>
          </div>
        )}
      </main>
    </div>
  )
}

export default App
