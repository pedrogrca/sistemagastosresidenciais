import { useEffect, useState, type FormEvent } from 'react'
import { pessoasApi } from '../api/client'
import type { Pessoa } from '../api/types'

/**
 * Tela de cadastro de pessoas: formulário para adicionar + tabela com a
 * listagem e a ação de excluir. Todo o estado (dados, carregamento, erro e
 * campos do formulário) vive aqui, mantendo a tela autocontida.
 */
export function PessoasPage() {
  const [pessoas, setPessoas] = useState<Pessoa[]>([])
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState<string | null>(null)

  // Campos do formulário de cadastro.
  const [nome, setNome] = useState('')
  const [idade, setIdade] = useState('')
  const [enviando, setEnviando] = useState(false)

  // Carrega a lista assim que a tela é aberta.
  useEffect(() => {
    carregarPessoas()
  }, [])

  async function carregarPessoas() {
    setCarregando(true)
    setErro(null)
    try {
      setPessoas(await pessoasApi.listar())
    } catch (e) {
      setErro((e as Error).message)
    } finally {
      setCarregando(false)
    }
  }

  async function handleCriar(evento: FormEvent) {
    evento.preventDefault()
    setEnviando(true)
    setErro(null)
    try {
      await pessoasApi.criar({ nome: nome.trim(), idade: Number(idade) })
      // Limpa o formulário e recarrega a lista somente após o sucesso.
      setNome('')
      setIdade('')
      await carregarPessoas()
    } catch (e) {
      setErro((e as Error).message)
    } finally {
      setEnviando(false)
    }
  }

  async function handleRemover(pessoa: Pessoa) {
    const confirmado = window.confirm(
      `Excluir "${pessoa.nome}"? Todas as transações dessa pessoa também serão apagadas.`,
    )
    if (!confirmado) return

    setErro(null)
    try {
      await pessoasApi.remover(pessoa.id)
      await carregarPessoas()
    } catch (e) {
      setErro((e as Error).message)
    }
  }

  // Validação simples do formulário no cliente (o back-end valida de novo).
  const formularioValido =
    nome.trim().length > 0 && idade !== '' && Number(idade) >= 0

  return (
    <>
      {erro && <div className="alerta-erro">{erro}</div>}

      <div className="card">
        <h2>Cadastrar pessoa</h2>
        <form onSubmit={handleCriar}>
          <div className="form-grid">
            <div className="campo">
              <label htmlFor="nome">Nome</label>
              <input
                id="nome"
                type="text"
                value={nome}
                maxLength={150}
                placeholder="Ex.: Maria Silva"
                onChange={(e) => setNome(e.target.value)}
              />
            </div>
            <div className="campo">
              <label htmlFor="idade">Idade</label>
              <input
                id="idade"
                type="number"
                min={0}
                max={130}
                value={idade}
                placeholder="Ex.: 30"
                onChange={(e) => setIdade(e.target.value)}
              />
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
        </form>
      </div>

      <div className="card">
        <h2>Pessoas cadastradas</h2>
        {carregando ? (
          <p className="vazio">Carregando...</p>
        ) : pessoas.length === 0 ? (
          <p className="vazio">Nenhuma pessoa cadastrada ainda.</p>
        ) : (
          <table className="tabela">
            <thead>
              <tr>
                <th>Id</th>
                <th>Nome</th>
                <th className="num">Idade</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {pessoas.map((pessoa) => (
                <tr key={pessoa.id}>
                  <td>{pessoa.id}</td>
                  <td>
                    {pessoa.nome}
                    {pessoa.menorDeIdade && (
                      <span className="selo selo-menor" style={{ marginLeft: 8 }}>
                        menor de idade
                      </span>
                    )}
                  </td>
                  <td className="num">{pessoa.idade}</td>
                  <td className="num">
                    <button
                      className="btn btn-perigo"
                      onClick={() => handleRemover(pessoa)}
                    >
                      Excluir
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </>
  )
}
