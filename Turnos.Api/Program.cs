
// Program.cs
using System.Text.Json.Serialization;
using System.Linq;                         // Necessário para Any(), AddRange() no seed
using Microsoft.EntityFrameworkCore;
using Turnos.Api.Infrastructure;
using Turnos.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// 1) Controllers + JSON com enums como string (ex.: "Concluido" ao invés de 1)
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// 2) DbContext (SQLite) - usa appsettings.json ou fallback para "turnos.db"
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? "Data Source=turnos.db"; // Se não houver appsettings.json, usa esse arquivo local
builder.Services.AddDbContext<AppDbContext>(opts => opts.UseSqlite(connectionString));

// 3) Registrar serviços de regras de negócio
builder.Services.AddScoped<ITurnoService, TurnoService>();

// 4) Swagger (documentação/teste de endpoints)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 5) Configurar CORS para permitir chamadas do frontend em localhost
    //Compartilhamento de Recursos entre Origens), 
    //é um mecanismo de segurança de navegador que permite que um site peça recursos (como fontes ou dados de API) de um domínio diferente
    // Antes de builder.Build():
    var corsPolicy = "AllowLocalDev";
    builder.Services.AddCors(opts =>
    {
        opts.AddPolicy(corsPolicy, p =>
        {
            p.WithOrigins(
                "http://localhost:5173", // Vite padrão
                "http://localhost:5174"  // caso mude
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
    });


var app = builder.Build();



// 6) Ativar Swagger em tempo de execução
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();

// 7) Seed simples: cria dados se vazio (para facilitar testes iniciais)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated(); // garante que o banco existe

    if (!db.Analistas.Any())
    {
        db.Analistas.AddRange(
            new Turnos.Api.Domain.Analista { Nome = "Isabelly Aquino", Email = "isabelly@linx.com", Ativo = true },
            new Turnos.Api.Domain.Analista { Nome = "Analista 2", Email = "analista2@linx.com", Ativo = true }
        );
        db.SaveChanges();
    }

    if (!db.Projetos.Any())
    {
        db.Projetos.AddRange(
            new Turnos.Api.Domain.Projeto { Nome = "Projeto Xavier", Ativo = true },
            new Turnos.Api.Domain.Projeto { Nome = "Portal do Cliente", Ativo = true }
        );
        db.SaveChanges();
    }
    
    
}

app.Run();
