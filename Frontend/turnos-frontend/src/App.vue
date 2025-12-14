<template>
 
    <!-- TOPBAR -->
    <header class="topbar">
      <h2>Gestão de Turnos</h2>

      <!-- NAVEGAÇÃO PRINCIPAL -->
      <nav>
        <router-link to="/" exact-active-class="active">
          Listagem
        </router-link>

        <router-link to="/novo" exact-active-class="active">
          Novo turno
        </router-link>
      </nav>
    </header>

    <!-- CONTEÚDO DAS ROTAS -->
    <main>
      <router-view />
    </main>

</template>

<style scoped>

/* TOPBAR*/
.topbar {
  position: sticky;
  top: 0;
  z-index: 100;

  display: flex;
  justify-content: space-between;
  align-items: center;

  padding: 14px 28px;

  background: linear-gradient(135deg, #7b1fa2, #4a148c);
  border-bottom: 1px solid rgba(187, 134, 252, .25);
  box-shadow: 0 8px 30px rgba(0, 0, 0, .45);
}

/* TÍTULO */
h2 {
  margin: 0;
  font-size: 1.15rem;
  font-weight: 600;
  color: #fff;
  letter-spacing: .4px;
}

/* NAVEGAÇÃO */
nav {
  display: flex;
  gap: 14px;

}

/* Links padrão */
nav a {
  position: relative;
  text-decoration: none;
  color: #e1d6f5;
  font-size: 0.85rem;
  padding: 6px 10px;
  border-radius: 8px;
  transition: all .2s;
}

/* Hover */
nav a:hover {
  background: rgba(255, 255, 255, .12);
  color: #fff;
}

/* Rota ativa */
nav a.active {
  background: rgba(187, 134, 252, .25);
  color: #fff;
  font-weight: 500;
}

/* MAIN */
main {
  /* evitar que o App.vue imponha espaçamento, deixando cada tela controlar o próprio layout */
  padding: 0;
}
</style>
