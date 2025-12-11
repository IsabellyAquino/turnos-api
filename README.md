# 📘 Gestão de Turnos — Backend (.NET + SQLite) & Frontend (Vue 3 + Vite)

Sistema web para **gerenciar turnos** de analistas com registros de **data, hora, motivo e status**, incluindo **cadastro, listagem e filtros**. 
---

## 🧩 Stack & Arquitetura

* **Backend**: .NET 8, API REST, Entity Framework Core + SQLite
* **Frontend**: Vue 3 (Vite), Axios, Vue Router
* **Padrão de retorno**: `ApiResponse<T>` (success, message, errors, data, pagination)
* **Camadas**:

  * **Domain** (Entidades/Enum): `Analista`, `Projeto`, `Turno`, `StatusTurno`
  * **Infrastructure** (Persistência): `AppDbContext` (EF Core, SQLite)
  * **Services** (Regras de negócio): `TurnoService`, `ITurnoService`
  * **DTOs** (Contratos): `TurnoCreateRequest`, `TurnoFilterQuery`, `TurnoResponse`
  * **Controllers** (API): `TurnosController`
  * **Common**: `ApiResponse<T>`

---

## 🗂️ Estrutura do Repositório

```
turnos-api/
├─ Turnos.Api/                 # Backend (.NET API)
│  ├─ Common/                  # ApiResponse
│  ├─ Controllers/             # TurnosController
│  ├─ Domain/                  # Analista, Projeto, Turno, StatusTurno
│  ├─ DTOs/                    # DTOs
│  ├─ Infrastructure/          # AppDbContext (EF Core + SQLite)
│  ├─ Services/                # ITurnoService, TurnoService
│  ├─ Properties/launchSettings.json
│  ├─ Program.cs
│  └─ appsettings.json
│
└─ Frontend/
   └─ turnos-frontend/         # Vue 3 + Vite
      ├─ src/
      │  ├─ services/api.js
      │  ├─ router/index.js
      │  ├─ views/TurnosList.vue
      │  ├─ views/TurnosForm.vue
      │  └─ App.vue, main.js
      └─ .env                  # VITE_API_BASE_URL
```

---

## ✅ Pré-requisitos

### Backend

* .NET SDK 8
* Ferramenta de migrations:

```
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
```

### Frontend

* Node.js 18+
* npm

---

## 🚀 Rodando o Backend

### 1. Acesse o diretório

```
cd turnos-api/Turnos.Api
```

### 2. Restaurar e compilar

```
dotnet restore
dotnet build
```

### 3. Criar migration e banco

```
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Executar a API

```
dotnet run
```

A API iniciará, por exemplo, em:

* `http://localhost:5031`
* Swagger: `/swagger`

### 📌 Padronizar para portas 5000/5001

Ajuste `launchSettings.json` conforme necessário.

### CORS

Certifique-se de permitir o front (`http://localhost:5173`).

---

## 🖥️ Rodando o Frontend

### 1. Acesse o diretório

```
cd turnos-api/Frontend/turnos-frontend
```

### 2. Instalar dependências

```
npm install
npm install axios
```

### 3. Criar arquivo `.env`

```
VITE_API_BASE_URL=http://localhost:5031
```

### 4. Executar o projeto

```
npm run dev
```

Acesse: `http://localhost:5173`

---

## 📡 Endpoints principais

### **GET /turnos** — Listagem com filtros

Exemplos:

```
/turnos
/turnos?analistaId=1&dataInicio=2025-12-01&dataFim=2025-12-31
/turnos?status=Pendente&page=2&pageSize=10
```

### **POST /turnos** — Criar turno

Payload exemplo:

```json
{
  "data": "2025-12-08T00:00:00",
  "horaInicio": "08:00:00",
  "horaFim": "12:30:00",
  "motivo": "Atendimento Premier + análise fiscal",
  "status": "Concluido",
  "analistaId": 1,
  "projetoId": 1,
  "observacoes": "Cliente solicitou relatório detalhado",
  "ativo": true
}
```

### Envelope de resposta (ApiResponse)

```json
{
  "success": true,
  "message": "Turno criado com sucesso.",
  "errors": null,
  "data": {}
}
```

---

## 🧪 Testes rápidos (curl)

Criar turno:

```
curl -X POST http://localhost:5031/turnos \
  -H "Content-Type: application/json" \
  -d "{ ... }"
```

Listar turnos:

```
curl "http://localhost:5031/turnos?analistaId=1"
```

---

## 🛠️ Troubleshooting

* **No project was found** ao rodar migrations → execute dentro de `Turnos.Api`.
* **dotnet-ef não reconhecido** → reinstale e ajuste PATH.
* **Erro CORS** → confirme origem `http://localhost:5173`.
* **Erro 400 no POST** → revise o JSON.

---

