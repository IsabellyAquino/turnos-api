
// Serviço central de API com axios, lendo baseURL do .env
// e interceptadores para tratamento global de erros.
// O Axios é uma biblioteca JavaScript baseada em promessas para fazer requisições HTTP (como GET, POST, PUT, DELETE)
import axios from 'axios'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL
})

// Interceptadores (opcional): logs, tratamento de erros...
api.interceptors.response.use(
  (resp) => resp,
  (err) => {
    // Normaliza erro vindo no envelope ApiResponse ou erro de rede
    const msg = err?.response?.data?.message || err.message || 'Erro inesperado.'
    console.error('[API ERROR]', msg, err?.response?.data?.errors)
    return Promise.reject(err)
  }
)

export default api
