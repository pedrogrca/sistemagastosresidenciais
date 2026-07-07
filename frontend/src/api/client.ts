import type {
  CriarPessoaRequest,
  CriarTransacaoRequest,
  Pessoa,
  Totais,
  Transacao,
} from './types'

/**
 * URL base da API. Pode ser sobrescrita pela variável de ambiente VITE_API_URL;
 * caso contrário, usa o endereço padrão do back-end em desenvolvimento.
 */
const API_BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5049/api'

/**
 * Wrapper genérico sobre o fetch: monta a URL, envia/recebe JSON e converte
 * respostas de erro do back-end (ProblemDetails) em Error com mensagem legível.
 */
async function request<T>(caminho: string, opcoes: RequestInit = {}): Promise<T> {
  const resposta = await fetch(`${API_BASE_URL}${caminho}`, {
    ...opcoes,
    headers: {
      'Content-Type': 'application/json',
      ...(opcoes.headers as Record<string, string> | undefined),
    },
  })

  if (!resposta.ok) {
    throw new Error(await extrairMensagemDeErro(resposta))
  }

  // 204 No Content (ex.: DELETE) não possui corpo para desserializar.
  if (resposta.status === 204) {
    return undefined as T
  }

  return (await resposta.json()) as T
}

/** Extrai a melhor mensagem de erro possível do corpo da resposta. */
async function extrairMensagemDeErro(resposta: Response): Promise<string> {
  try {
    const corpo = await resposta.json()

    // Erros de validação do ASP.NET vêm em { errors: { campo: [mensagens] } }.
    if (corpo.errors) {
      const mensagens = Object.values(corpo.errors).flat()
      if (mensagens.length > 0) {
        return mensagens.join(' ')
      }
    }

    // Erros de regra de negócio vêm em { detail } ou { title }.
    return corpo.detail ?? corpo.title ?? `Erro ${resposta.status}`
  } catch {
    return `Erro ${resposta.status}`
  }
}

/** Operações do cadastro de pessoas. */
export const pessoasApi = {
  listar: () => request<Pessoa[]>('/pessoas'),
  criar: (dados: CriarPessoaRequest) =>
    request<Pessoa>('/pessoas', { method: 'POST', body: JSON.stringify(dados) }),
  remover: (id: number) => request<void>(`/pessoas/${id}`, { method: 'DELETE' }),
}

/** Operações do cadastro de transações. */
export const transacoesApi = {
  listar: () => request<Transacao[]>('/transacoes'),
  criar: (dados: CriarTransacaoRequest) =>
    request<Transacao>('/transacoes', {
      method: 'POST',
      body: JSON.stringify(dados),
    }),
  remover: (id: number) => request<void>(`/transacoes/${id}`, { method: 'DELETE' }),
}

/** Consulta de totais. */
export const totaisApi = {
  obter: () => request<Totais>('/totais'),
}
