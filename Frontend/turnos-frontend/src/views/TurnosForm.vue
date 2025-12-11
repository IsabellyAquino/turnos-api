<script setup>
// Formulário simples para criar um turno chamando POST /turnos.
// Campos mínimos, com validações básicas.
import { ref } from 'vue'
import api from '../services/api'
import { useRouter } from 'vue-router'

const router = useRouter()

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

async function salvar() {
  try {
    erro.value = ''
    sucesso.value = ''

    // Ajuste de formato: data deve ter "T00:00:00"
    const payload = {
      ...form.value,
      data: form.value.data ? `${form.value.data}T00:00:00` : null,
      horaInicio: form.value.horaInicio.length === 5 ? form.value.horaInicio + ':00' : form.value.horaInicio,
      horaFim: form.value.horaFim.length === 5 ? form.value.horaFim + ':00' : form.value.horaFim
    }

    const { data } = await api.post('/turnos', payload)

    if (data.success) {
      sucesso.value = data.message || 'Turno criado com sucesso.'
      // Redireciona para a listagem após salvar
      setTimeout(() => router.push('/'), 800)
    } else {
      erro.value = (data.errors && data.errors.join('; ')) || data.message || 'Falha ao criar turno.'
    }
  } catch (e) {
    erro.value = e?.response?.data?.message || e.message
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
