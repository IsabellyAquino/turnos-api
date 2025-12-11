// Define rotas/telas da aplicação.
import { createRouter, createWebHistory } from 'vue-router'
import TurnosList from '../views/TurnosList.vue'
import TurnosForm from '../views/TurnosForm.vue'

const routes = [
  { path: '/', name: 'home', component: TurnosList },
  { path: '/novo', name: 'novo-turno', component: TurnosForm }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
