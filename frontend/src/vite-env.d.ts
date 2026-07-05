/// <reference types="vite/client" />

interface ImportMetaEnv {
  /** URL base da API. Opcional; se ausente, usa o back-end local. */
  readonly VITE_API_URL?: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
