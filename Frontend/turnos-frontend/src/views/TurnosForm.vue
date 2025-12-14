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
  projetoId: null,   // opcional
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
  <!-- FUNDO DA PÁGINA -->
  <section class="page">

    <!-- CARD PRINCIPAL -->
    <div class="card">

      <!-- TÍTULO -->
      <h1>Novo Turno</h1>

      <!-- FORMULÁRIO -->
       <div class="grid">

        <label>
          Data
          <input type="date" required v-model="form.data" />
        </label>

        <label>
          Analista ID
          <input type="number" required v-model.number="form.analistaId" />
        </label>

        <label>
          Hora Início
          <input type="time" required v-model="form.horaInicio" />
        </label>

        <label>
          Hora Fim
          <input type="time" required v-model="form.horaFim" />
        </label>

        <!-- Campo grande -->
        <label class="span-2">
          Motivo
          <input type="text" required v-model="form.motivo" />
        </label>

        <label>
          Status
          <select v-model="form.status">
            <option>Pendente</option>
            <option>Concluido</option>
            <option>Cancelado</option>
          </select>
        </label>

        <label>
          Projeto ID (Opcional)
          <input type="number" v-model.number="form.projetoId" />
        </label>

        <!-- Campo grande -->
        <label class="span-2">
          Observações (Opcional)
          <input type="text" v-model="form.observacoes" />
        </label>

        <!-- Checkbox -->
        <label class="checkbox">
          <input type="checkbox" v-model="form.ativo" />
          Ativo
        </label>

      </div>

      <!-- AÇÕES -->
      <div class="acoes">
        <button class="btn primary" @click="salvar">Salvar</button>
        <router-link to="/">
          <button class="btn ghost">Cancelar</button>
        </router-link>
      </div>

      <!-- FEEDBACK -->
      <p v-if="erro" class="erro">{{ erro }}</p>
      <p v-if="sucesso" class="sucesso">{{ sucesso }}</p>

    </div>
  </section>
</template>

<style scoped>
/* FUNDO */
.page {
  min-height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background:
    radial-gradient(circle at top left, #7b1fa2, transparent 40%),
    radial-gradient(circle at bottom right, #4a148c, transparent 45%),
    #0e0b14;
  padding: 24px;
}

/* CARD PRINCIPAL */
.card {
  width: 100%;
  max-width: 850px;
  background: rgba(20, 16, 30, 0.95);
  backdrop-filter: blur(8px);
  border-radius: 18px;
  padding: 32px;
  box-shadow: 0 30px 60px rgba(0,0,0,.6);
}

/* TÍTULO */
h1 {
  color: #fff;
  font-size: 1.7rem;
  margin-bottom: 28px;
  padding-left: 14px;
}

/* GRID DO FORM */
.grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 18px 22px;
}

/* Campo ocupando 2 colunas */
.span-2 {
  grid-column: span 2;
}

/* LABELS E INPUTS */
label {
  display: flex;
  flex-direction: column;
  font-size: 0.85rem;
  color: #c7b7e2;
  gap: 6px;
}

input,
select {
  background: #1c162b;
  border: 1px solid #3a2f55;
  border-radius: 10px;
  padding: 11px 14px;
  color: #fff;
  font-size: 0.95rem;
  transition: all .2s;
}

input:focus,
select:focus {
  outline: none;
  border-color: #bb86fc;
  box-shadow: 0 0 0 2px rgba(187,134,252,.3);
}

/* CHECKBOX */
.checkbox {
  flex-direction: row;
  align-items: center;
  gap: 10px;
  margin-top: 8px;
}

.checkbox input {
  width: 18px;
  height: 18px;
}

/* BOTÕES */
.acoes {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  margin-top: 32px;
}

.btn {
  padding: 10px 22px;
  border-radius: 12px;
  font-size: 0.9rem;
  cursor: pointer;
  border: none;
  transition: all .2s;
}

/* Botão principal roxo */
.btn.primary {
  background: linear-gradient(135deg, #bb86fc, #7b1fa2);
  color: #fff;
}

.btn.primary:hover {
  transform: translateY(-1px);
  box-shadow: 0 10px 25px rgba(187,134,252,.45);
}

/* Botão secundário */
.btn.ghost {
  background: transparent;
  color: #c7b7e2;
  border: 1px solid #3a2f55;
}

.btn.ghost:hover {
  background: #221a35;
}

/* FEEDBACK */
.erro {
  margin-top: 18px;
  color: #ef5350;
}

.sucesso {
  margin-top: 18px;
  color: #66bb6a;
}
</style>