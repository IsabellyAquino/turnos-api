<script setup>
// Formulário simples para criar um turno chamando POST /turnos.
// Campos mínimos, com validações básicas.
import { ref } from 'vue'
import api from '../services/api'
import { useRouter } from 'vue-router'

const router = useRouter()

// Estado do formulário
const form = ref({
  data: '',
  horaInicio: '',
  horaFim: '',
  motivo: '',
  status: 'Pendente',
  analistaId: 1,  // Id do analista (use os do seed)
  projetoId: 1,   // opcional
  observacoes: '',
  ativo: true
})

const erro = ref('')
const sucesso = ref('')

//Normaliza uma string de hora para o formato HH:mm:ss
function toHHmmss(h) {
  if (!h) return ''
  // Já possui segundos? Mantém.
  if (/^\d{1,2}:\d{2}:\d{2}$/.test(h)) {
    const [hh, mm, ss] = h.split(':')
    return `${hh.padStart(2, '0')}:${mm}:${ss}`
  }
  // Formato comum de <input type="time">: "HH:mm" (ou "H:mm")
  if (/^\d{1,2}:\d{2}$/.test(h)) {
    const [hh, mm] = h.split(':')
    return `${hh.padStart(2, '0')}:${mm}:00`
  }
  // Último recurso: tenta parsear algo diferente (evitar quebra silenciosa)
  try {
    const parts = h.split(':')
    const hh = (parts[0] ?? '0').padStart(2, '0')
    const mm = (parts[1] ?? '0').padStart(2, '0')
    const ss = (parts[2] ?? '0').padStart(2, '0')
    return `${hh}:${mm}:${ss}`
  } catch {
    return h // devolve como veio; backend pode validar e informar erro
  }
}


//Normaliza a data "YYYY-MM-DD" para "YYYY-MM-DDT00:00:00",
//mantendo compatibilidade com DTO que usa DateTime.
function toDateAtMidnight(d) {
  if (!d) return null
  // Evita duplicar T00:00:00 se já estiver no formato ISO
  if (/^\d{4}-\d{2}-\d{2}T/.test(d)) return d
  return `${d}T00:00:00`
}


async function salvar() {
  try {
    erro.value = ''
    sucesso.value = ''

    const payload = {
      ...form.value,
      
      data: toDateAtMidnight(form.value.data),
      horaInicio: toHHmmss(form.value.horaInicio),
      horaFim: toHHmmss(form.value.horaFim),
      // Assegurar tipos corretos
      analistaId: Number(form.value.analistaId),
      projetoId: form.value.projetoId ? Number(form.value.projetoId) : null,
      ativo: !!form.value.ativo
    }

    const { data } = await api.post('/turnos', payload)

    if (data.success) {
      sucesso.value = data.message || 'Turno criado com sucesso.'
      // Redireciona para a listagem após salvar (pequeno delay para usuário ver feedback)
      setTimeout(() => router.push('/'), 800)
    } else {
      // Mostra erros de negócio agrupados (vindos do ApiResponse.Fail)
      erro.value = (data.errors && data.errors.join('; ')) || data.message || 'Falha ao criar turno.'
    }
  } catch (e) {
    // Diferenciar erro de rede (CORS/HTTPS) de erro de API (4xx/5xx com body)
    if (e?.response) {
      const d = e.response.data
      erro.value = (d?.errors && d.errors.join('; ')) || d?.message || `Erro ${e.response.status}`
    } else {
      erro.value = e?.message || 'Network Error'
    }
  }
}
</script>


<template>
  <section class="container">
    <h1>Novo Turno</h1>

    <div class="grid">
      <label>Data
        <input v-model="form.data" type="date" required />
      </label>
      <label>Analista Id
        <input v-model.number="form.analistaId" type="number" required />
      </label>
      <label>Hora Início
        <input v-model="form.horaInicio" type="time" step="60" required />
      </label>
      <label>Hora Fim
        <input v-model="form.horaFim" type="time" step="60" required />
      </label>
      <label>Motivo
        <input v-model="form.motivo" type="text" required />
      </label>
      <label>Status
        <select v-model="form.status">
          <option value="Concluido">Concluído</option>
          <option value="Pendente">Pendente</option>
          <option value="Cancelado">Cancelado</option>
        </select>
      </label>
      <label>Projeto Id (opcional)
        <input v-model.number="form.projetoId" type="number" />
      </label>
      <label>Observações
        <input v-model="form.observacoes" type="text" />
      </label>
      <label>Ativo
        <input v-model="form.ativo" type="checkbox" />
      </label>
    </div>

    <div class="acoes">
      <button @click="salvar">Salvar</button>
      <router-link to="/"><button>Cancelar</button></router-link>
    </div>

    <p v-if="erro" class="erro">{{ erro }}</p>
    <p v-if="sucesso" class="sucesso">{{ sucesso }}</p>
  </section>
</template>

<style scoped>
.container { padding: 16px; max-width: 800px; margin: 0 auto; }
.grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 12px; }
.acoes { margin-top: 16px; display: flex; gap: 8px; }
.erro { color: #b00020; }
.sucesso { color: #2e7d32; }
label { display: flex; flex-direction: column; gap: 6px; }
</style>
