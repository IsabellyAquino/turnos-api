<!-- src/views/TurnosList.vue -->
<script setup>
// Componente em <script setup>: imports diretos e estado local
// - ref: estado reativo (variáveis)
// - onMounted: executa ao montar o componente
import { ref, onMounted } from 'vue'
import api from '../services/api'

// Filtros usados na querystring do GET /turnos
const filtros = ref({
  analistaId: '',
  projetoId: '',
  status: '',      // Concluido | Pendente | Cancelado
  dataInicio: '',
  dataFim: '',
  page: 1,
  pageSize: 20
})

// Estados de tela (loading/erro/dados)
const carregando = ref(false)
const erro = ref('')
const turnos = ref([])
const pagination = ref(null)

// Função que chama a API com filtros
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

    // Chama GET /turnos passando filtros (params).
    const { data } = await api.get('/turnos', { params })

    // Trata o envelope ApiResponse<T> (success/data/pagination)
    if (data.success) {
      turnos.value = data.data || []
      pagination.value = data.pagination || null
    } else {
      erro.value = data.message || 'Falha ao listar turnos.'
    }
  } catch (e) {
    // Erro de rede ou erro padronizado pela API
    erro.value = e?.response?.data?.message || e.message || 'Erro inesperado.'
  } finally {
    carregando.value = false
  }
}

// Carregar automaticamente ao montar
onMounted(carregar)
</script>

<template>
  <!-- FUNDO -->
  <section class="page">

    <!-- CONTAINER -->
    <div class="container">

      <h1>Turnos</h1>

      <!-- FILTROS / AÇÕES -->
      <div class="filtros">

        <input
          v-model="filtros.analistaId"
          type="number"
          placeholder="Analista Id"
        />

        <input
          v-model="filtros.projetoId"
          type="number"
          placeholder="Projeto Id (opcional)"
        />

        <select v-model="filtros.status">
          <option value="">(todos)</option>
          <option value="Concluido">Concluído</option>
          <option value="Pendente">Pendente</option>
          <option value="Cancelado">Cancelado</option>
        </select>

        <input v-model="filtros.dataInicio" type="date" />
        <input v-model="filtros.dataFim" type="date" />

        <button class="btn ghost" @click="carregar">Filtrar</button>

        <router-link to="/novo">
          <button class="btn primary">Novo turno</button>
        </router-link>
      </div>

      <!-- ESTADOS -->
      <div v-if="carregando" class="loading">Carregando...</div>
      <div v-if="erro" class="erro">{{ erro }}</div>

      <!-- TABELA -->
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
            <td>
              <span class="status" :class="t.status.toLowerCase()">
                {{ t.status }}
              </span>
            </td>
            <td>{{ t.analistaId }}</td>
            <td>{{ t.projetoId ?? '-' }}</td>
          </tr>
        </tbody>
      </table>

      <!-- PAGINAÇÃO -->
      <div v-if="pagination" class="paginacao">
        Página {{ pagination.page }} de {{ pagination.totalPages }} —
        Total: {{ pagination.totalItems }}
      </div>

      <!-- SEM DADOS -->
      <div v-else-if="!carregando && !erro" class="no-data">
        Nenhum turno encontrado.
      </div>

    </div>
  </section>
</template>

<style scoped>
/* FUNDO */
.page {
  min-height: 100vh;
  padding: 24px;
  background:
    radial-gradient(circle at top left, #7b1fa2, transparent 40%),
    radial-gradient(circle at bottom right, #4a148c, transparent 45%),
    #0e0b14;
}

.container {
  margin: 0 auto;
  background: rgba(20, 16, 30, 0.95);
  border-radius: 18px;
  padding: 28px 32px;
  box-shadow: 0 30px 60px rgba(0,0,0,.6);
}

/* TÍTULO */
h1 {
  color: #fff;
  font-size: 1.6rem;
  margin-bottom: 20px;
  padding-left: 14px;
}


/* FILTROS */
.filtros {
  display: flex;
  gap: 10px;
  margin-bottom: 18px;
  flex-wrap: wrap;
}

input,
select {
  background: #1c162b;
  border: 1px solid #3a2f55;
  border-radius: 10px;
  padding: 9px 12px;
  color: #fff;
  font-size: 0.85rem;
}

input:focus,
select:focus {
  outline: none;
  border-color: #bb86fc;
  box-shadow: 0 0 0 2px rgba(187,134,252,.3);
}


/* BOTÕES */
.btn {
  padding: 9px 18px;
  border-radius: 12px;
  font-size: 0.85rem;
  border: none;
  cursor: pointer;
  transition: all .2s;
}

.btn.primary {
  background: linear-gradient(135deg, #bb86fc, #7b1fa2);
  color: #fff;
}

.btn.primary:hover {
  transform: translateY(-1px);
  box-shadow: 0 10px 25px rgba(187,134,252,.45);
}

.btn.ghost {
  background: transparent;
  color: #c7b7e2;
  border: 1px solid #3a2f55;
}

.btn.ghost:hover {
  background: #221a35;
}


/* TABELA  */
table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 12px;
  font-size: 0.85rem;
}

thead {
  background: #1c162b;
}

th {
  color: #c7b7e2;
  padding: 10px;
  text-align: left;
}

td {
  padding: 10px;
  border-top: 1px solid #2f2446;
  color: #eee;
}

tbody tr:hover {
  background: rgba(187,134,252,.06);
}


/* STATUS */
.status {
  padding: 4px 10px;
  border-radius: 20px;
  font-size: 0.75rem;
}

.status.pendente {
  color: #ffc107;
  background: rgba(255,193,7,.15);
}

.status.concluido {
  color: #4caf50;
  background: rgba(76,175,80,.15);
}

.status.cancelado {
  color: #f44336;
  background: rgba(244,67,54,.15);
}


/* ESTADOS */
.loading {
  color: #bb86fc;
}

.erro {
  color: #ef5350;
  margin-top: 8px;
}

.no-data {
  color: #aaa;
  margin-top: 12px;
}

.paginacao {
  margin-top: 12px;
  color: #c7b7e2;
}
</style>
