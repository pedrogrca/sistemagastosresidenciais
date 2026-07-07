import { useEffect, useRef, useState, type FormEvent } from 'react'
import { pessoasApi, transacoesApi } from '../api/client'
import type {
  CategoriaTransacao,
  Pessoa,
  TipoTransacao,
  Transacao,
} from '../api/types'
import { formatarMoeda } from '../utils/formato'
import { CATEGORIAS, LISTA_CATEGORIAS } from '../utils/categorias'

/**
 * Tela de cadastro de transações: formulário (com a regra do menor de idade
 * refletida na interface) + listagem com exclusão. Carrega as pessoas para o
 * seletor e as transações para a tabela.
 */
export function TransacoesPage() {
  const [transacoes, setTransacoes] = useState<Transacao[]>([])
  const [pessoas, setPessoas] = useState<Pessoa[]>([])
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState<string | null>(null)
  const [sucesso, setSucesso] = useState<string | null>(null)

  // Campos do formulário.
  const [descricao, setDescricao] = useState('')
  const [valor, setValor] = useState('')
  const [tipo, setTipo] = useState<TipoTransacao>('Despesa')
  const [categoria, setCategoria] = useState<CategoriaTransacao | ''>('')
  const [pessoaId, setPessoaId] = useState('')
  const [enviando, setEnviando] = useState(false)

  const descricaoRef = useRef<HTMLInputElement>(null)

  useEffect(() => {
    carregar()
  }, [])

  async function carregar() {
    setCarregando(true)
    setErro(null)
    try {
      const [listaPessoas, listaTransacoes] = await Promise.all([
        pessoasApi.listar(),
        transacoesApi.listar(),
      ])
      setPessoas(listaPessoas)
      setTransacoes(listaTransacoes)
    } catch (e) {
      setErro((e as Error).message)
    } finally {
      setCarregando(false)
    }
  }

  const pessoaSelecionada = pessoas.find((p) => p.id === Number(pessoaId))
  const menorSelecionado = pessoaSelecionada?.menorDeIdade ?? false

  // Regra do desafio refletida na interface: menor de idade só pode despesa.
  useEffect(() => {
    if (menorSelecionado && tipo === 'Receita') {
      setTipo('Despesa')
    }
  }, [menorSelecionado, tipo])

  function mostrarSucesso(mensagem: string) {
    setSucesso(mensagem)
    window.setTimeout(() => setSucesso(null), 3000)
  }

  async function handleCriar(evento: FormEvent) {
    evento.preventDefault()
    setEnviando(true)
    setErro(null)
    const descricaoCriada = descricao.trim()
    try {
      await transacoesApi.criar({
        descricao: descricaoCriada,
        valor: Number(valor),
        tipo,
        categoria: categoria as CategoriaTransacao,
        pessoaId: Number(pessoaId),
      })
      // Mantém tipo, categoria e pessoa para facilitar vários lançamentos.
      setDescricao('')
      setValor('')
      await carregar()
      mostrarSucesso(`Transação "${descricaoCriada}" cadastrada.`)
      descricaoRef.current?.focus()
    } catch (e) {
      setErro((e as Error).message)
    } finally {
      setEnviando(false)
    }
  }

  async function handleRemover(transacao: Transacao) {
    const confirmado = window.confirm(
      `Excluir a transação "${transacao.descricao}"? (a pessoa não é afetada)`,
    )
    if (!confirmado) return

    setErro(null)
    try {
      await transacoesApi.remover(transacao.id)
      await carregar()
      mostrarSucesso(`Transação "${transacao.descricao}" excluída.`)
    } catch (e) {
      setErro((e as Error).message)
    }
  }

  const formularioValido =
    descricao.trim() !== '' &&
    valor !== '' &&
    Number(valor) > 0 &&
    categoria !== '' &&
    pessoaId !== ''

  const semPessoas = !carregando && pessoas.length === 0

  return (
    <>
      {erro && <div className="alerta-erro">{erro}</div>}
      {sucesso && <div className="alerta-sucesso">{sucesso}</div>}

      <div className="card">
        <div className="card-header">
          <h2>Cadastrar transação</h2>
          <p className="card-subtitulo">
            Registre uma receita ou despesa vinculada a uma pessoa e categoria.
          </p>
        </div>

        {semPessoas ? (
          <p className="vazio">
            Cadastre uma pessoa primeiro para poder lançar transações.
          </p>
        ) : (
          <form onSubmit={handleCriar}>
            <div className="form-grid">
              <div className="campo">
                <label htmlFor="descricao">Descrição</label>
                <input
                  id="descricao"
                  ref={descricaoRef}
                  type="text"
                  value={descricao}
                  maxLength={250}
                  autoFocus
                  placeholder="Ex.: Conta de luz"
                  onChange={(e) => setDescricao(e.target.value)}
                />
              </div>
              <div className="campo">
                <label htmlFor="valor">Valor (R$)</label>
                <input
                  id="valor"
                  type="number"
                  min="0.01"
                  step="0.01"
                  value={valor}
                  placeholder="Ex.: 150,00"
                  onChange={(e) => setValor(e.target.value)}
                />
              </div>
              <div className="campo">
                <label htmlFor="pessoa">Pessoa</label>
                <select
                  id="pessoa"
                  value={pessoaId}
                  onChange={(e) => setPessoaId(e.target.value)}
                >
                  <option value="">Selecione...</option>
                  {pessoas.map((p) => (
                    <option key={p.id} value={p.id}>
                      {p.nome}
                      {p.menorDeIdade ? ' (menor)' : ''}
                    </option>
                  ))}
                </select>
              </div>
              <div className="campo">
                <label htmlFor="tipo">Tipo</label>
                <select
                  id="tipo"
                  value={tipo}
                  onChange={(e) => setTipo(e.target.value as TipoTransacao)}
                >
                  <option value="Despesa">Despesa</option>
                  <option value="Receita" disabled={menorSelecionado}>
                    Receita{menorSelecionado ? ' (indisponível p/ menor)' : ''}
                  </option>
                </select>
              </div>
              <div className="campo">
                <label htmlFor="categoria">Categoria</label>
                <select
                  id="categoria"
                  value={categoria}
                  onChange={(e) =>
                    setCategoria(e.target.value as CategoriaTransacao)
                  }
                >
                  <option value="">Selecione...</option>
                  {LISTA_CATEGORIAS.map((c) => (
                    <option key={c} value={c}>
                      {CATEGORIAS[c].rotulo}
                    </option>
                  ))}
                </select>
              </div>
              <div className="campo">
                <button
                  type="submit"
                  className="btn btn-primario"
                  disabled={!formularioValido || enviando}
                >
                  {enviando ? 'Salvando...' : 'Adicionar'}
                </button>
              </div>
            </div>

            {menorSelecionado && (
              <p className="aviso">
                {pessoaSelecionada?.nome} é menor de idade, então só pode
                registrar <strong>despesas</strong>.
              </p>
            )}
          </form>
        )}
      </div>

      <div className="card">
        <h2>Transações</h2>
        {carregando ? (
          <p className="vazio">Carregando...</p>
        ) : transacoes.length === 0 ? (
          <p className="vazio">Nenhuma transação cadastrada ainda.</p>
        ) : (
          <div className="tabela-wrap">
            <table className="tabela">
              <thead>
                <tr>
                  <th>Descrição</th>
                  <th>Categoria</th>
                  <th>Pessoa</th>
                  <th>Tipo</th>
                  <th className="num">Valor</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {transacoes.map((t) => (
                  <tr key={t.id}>
                    <td>{t.descricao}</td>
                    <td>
                      <span
                        className="cat-dot"
                        style={{ background: CATEGORIAS[t.categoria].cor }}
                      />
                      {CATEGORIAS[t.categoria].rotulo}
                    </td>
                    <td>{t.pessoaNome}</td>
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
