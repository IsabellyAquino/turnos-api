<!-- src/views/TurnosList.vue -->
<script setup>
// 🔹 Componente em <script setup>: imports diretos e estado local
//    - ref: estado reativo (variáveis)
//    - onMounted: executa ao montar o componente
import { ref, onMounted } from 'vue'
import api from '../services/api'

// 🔹 Filtros usados na querystring do GET /turnos
const filtros = ref({
  analistaId: '',
  projetoId: '',
  status: '',      // Concluido | Pendente | Cancelado
  dataInicio: '',
  dataFim: '',
  page: 1,
  pageSize: 20
})

// 🔹 Estados de tela (loading/erro/dados)
const carregando = ref(false)
const erro = ref('')
const turnos = ref([])
const pagination = ref(null)

// 🔹 Função que chama a API com filtros
async function carregar() {
  try {
    carregando.value = true
    erro.value = ''

    // Monta a query só com valores preenchidos
    const params = {}
    if (filtros.value.analistaId) params.analistaId = filtros.value.analistaId
    if (filtros.value.projetoId) params.projetoId = filtros.value.projetoId
    if (filtros.value.status) params.status = filtros.value.status
    if (filtros.value.dataInicio) params.dataInicio = filtros.value.dataInicio
    if (filtros.value.dataFim) params.dataFim = filtros.value.dataFim
    params.page = filtros.value.page
    params.pageSize = filtros.value.pageSize

    // 🔹 Chama GET /turnos passando filtros (params).
    const { data } = await api.get('/turnos', { params })

    // 🔹 Trata o envelope ApiResponse<T> (success/data/pagination)
    if (data.success) {
      turnos.value = data.data || []
      pagination.value = data.pagination || null
    } else {
      erro.value = data.message || 'Falha ao listar turnos.'
    }
  } catch (e) {
    // 🔹 Erro de rede ou erro padronizado pela API
    erro.value = e?.response?.data?.message || e.message || 'Erro inesperado.'
  } finally {
    carregando.value = false
  }
}

// 🔹 Carregar automaticamente ao montar
onMounted(carregar)
</script>

<template>
  <section class="container">
    <h1>Turnos</h1>

    <!-- 🔹 Filtros / ações -->
    <div class="filtros">
      <input v-model="filtros.analistaId" type="number" placeholder="Analista Id" />
      <input v-model="filtros.projetoId" type="number" placeholder="Projeto Id (opcional)" />

      <select v-model="filtros.status">
        <option value="">(todos)</option>
        <option value="Concluido">Concluído</option>
        <option value="Pendente">Pendente</option>
        <option value="Cancelado">Cancelado</option>
      </select>

      <input v-model="filtros.dataInicio" type="date" />
      <input v-model="filtros.dataFim" type="date" />

      <button @click="carregar">Filtrar</button>
      <router-link to="/novo"><button>Novo turno</button></router-link>
    </div>

    <!-- 🔹 Estados de carregamento/erro -->
    <div v-if="carregando">Carregando...</div>
    <div v-if="erro" class="erro">{{ erro }}</div>

    <!-- 🔹 Tabela de resultados -->
    <table v-if="turnos.length">
      <thead>
        <tr>
          <th>Id</th>
          <th>Data</th>
          <th>Início</th>
          <th>Fim</th>
          <th>Duração (min)</th>
          <th>Motivo</th>
          <th>Status</th>
          <th>Analista</th>
          <th>Projeto</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="t in turnos" :key="t.id">
          <td>{{ t.id }}</td>
          <td>{{ new Date(t.data).toLocaleDateString() }}</td>
          <td>{{ t.horaInicio }}</td>
          <td>{{ t.horaFim }}</td>
          <td>{{ t.duracaoMinutos }}</td>
          <td>{{ t.motivo }}</td>
          <td>{{ t.status }}</td>
          <td>{{ t.analistaId }}</td>
          <td>{{ t.projetoId ?? '-' }}</td>
        </tr>
      </tbody>
    </table>

    <!-- 🔹 Paginação simples -->
    <div v-if="pagination" class="paginacao">
      Página {{ pagination.page }} de {{ pagination.totalPages }} —
      Total: {{ pagination.totalItems }}
    </div>

    <!-- 🔹 Quando não há resultados -->
    <div v-else-if="!carregando && !erro" class="no-data">
      Nenhum turno encontrado.
    </div>
  </section>
</template>

<style scoped>
.container { padding: 16px; }
.filtros { display: flex; gap: 8px; margin-bottom: 12px; flex-wrap: wrap; }
.erro { color: #b00020; margin-top: 8px; }
table { width: 100%; border-collapse: collapse; margin-top: 8px; }
th, td { border: 1px solid #ddd; padding: 6px; text-align: left; }
.paginacao { margin-top: 8px; }
.no-data { color: #555; margin-top: 8px; }
</style>
