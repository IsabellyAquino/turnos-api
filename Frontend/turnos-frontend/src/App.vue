<template>
  <header class="topbar">
    <h2>Gestão de Turnos</h2>
    <nav>
      <router-link to="/">Listagem</router-link>
      <router-link to="/novo">Novo turno</router-link>
    </nav>
  </header>
  <main>
    <router-view />
  </main>
</template>

<style scoped>
.topbar {
  display: flex; justify-content: space-between; align-items: center;
  padding: 10px 16px; background: #8b1b96; border-bottom: 1px solid #080606;
}
nav a { margin-left: 10px; }
main { padding: 16px; }
</style>
