
// Services/TurnoService.cs
using Microsoft.EntityFrameworkCore;
using Turnos.Api.Common;
using Turnos.Api.Domain;
using Turnos.Api.DTOs;
using Turnos.Api.Infrastructure;

namespace Turnos.Api.Services
{
    /// <summary>Implementa validações e operações de negócio para Turnos.</summary>
    public class TurnoService : ITurnoService
    {
        private readonly AppDbContext _db;

        public TurnoService(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Cria um turno validando horários, status e vínculos (analista/projeto).
        /// </summary>
        public async Task<ApiResponse<TurnoResponse>> CriarAsync(TurnoCreateRequest req)
        {
            var errors = new List<string>();

            // 1) Validar analista
            var analista = await _db.Analistas.FirstOrDefaultAsync(a => a.Id == req.AnalistaId);
            if (analista is null)
                errors.Add("Analista não encontrado.");
            else if (!analista.Ativo)
                errors.Add("Analista inativo, não é possível vincular turno.");

            // 2) Validar projeto (se informado)
            Projeto? projeto = null;
            if (req.ProjetoId.HasValue)
            {
                projeto = await _db.Projetos.FirstOrDefaultAsync(p => p.Id == req.ProjetoId.Value);
                if (projeto is null)
                    errors.Add("Projeto informado não foi encontrado.");
                else if (!projeto.Ativo)
                    errors.Add("Projeto está inativo.");
            }

            // 3) Validar horários e calcular duração
            if (req.HoraFim <= req.HoraInicio)
                errors.Add("Hora fim deve ser maior que hora início.");

            var duracao = (int)(req.HoraFim - req.HoraInicio).TotalMinutes;
            if (duracao <= 0)
                errors.Add("Duração calculada inválida.");

            // 4) Validar status (negócio simples)
            if (req.Status == StatusTurno.Cancelado && req.Ativo)
                errors.Add("Turno com status 'Cancelado' deve ser Ativo = false.");

            if (errors.Count > 0)
                return ApiResponse<TurnoResponse>.Fail(errors, "Falha ao criar turno.");

            // 5) Montar entidade
            var turno = new Turno
            {
                Data = req.Data.Date, // garante apenas data
                HoraInicio = req.HoraInicio,
                HoraFim = req.HoraFim,
                DuracaoMinutos = duracao,
                Motivo = req.Motivo,
                Status = req.Status,
                AnalistaId = req.AnalistaId,
                ProjetoId = req.ProjetoId,
                Observacoes = req.Observacoes,
                Ativo = req.Ativo
            };

            // 6) Persistir
            _db.Turnos.Add(turno);
            await _db.SaveChangesAsync();

            // 7) Mapear resposta
            var resp = Map(turno);
            return ApiResponse<TurnoResponse>.Ok(resp, "Turno criado com sucesso.");
        }

        /// <summary>
        /// Lista turnos com filtros + paginação (default: page=1, pageSize=20).
        /// </summary>
        public async Task<ApiResponse<List<TurnoResponse>>> ListarAsync(TurnoFilterQuery filtro)
        {
            // Base query
            var query = _db.Turnos
                .AsNoTracking()
                .OrderByDescending(t => t.Data)
                .ThenByDescending(t => t.HoraInicio)
                .AsQueryable();

            // Aplicar filtros condicionais
            if (filtro.AnalistaId.HasValue)
                query = query.Where(t => t.AnalistaId == filtro.AnalistaId.Value);

            if (filtro.ProjetoId.HasValue)
                query = query.Where(t => t.ProjetoId == filtro.ProjetoId.Value);

            if (filtro.Status.HasValue)
                query = query.Where(t => t.Status == filtro.Status.Value);

            if (filtro.DataInicio.HasValue)
                query = query.Where(t => t.Data.Date >= filtro.DataInicio.Value.Date);

            if (filtro.DataFim.HasValue)
                query = query.Where(t => t.Data.Date <= filtro.DataFim.Value.Date);

            // Paginação
            var total = await query.CountAsync();
            var items = await query
                .Skip((filtro.Page - 1) * filtro.PageSize)
                .Take(filtro.PageSize)
                .ToListAsync();

            var data = items.Select(Map).ToList();
            var pagination = new PaginationInfo
            {
                Page = filtro.Page,
                PageSize = filtro.PageSize,
                TotalItems = total
            };

            return ApiResponse<List<TurnoResponse>>.Ok(data, "Lista de turnos", pagination);
        }

        private static TurnoResponse Map(Turno t) => new()
        {
            Id = t.Id,
            Data = t.Data,
            HoraInicio = t.HoraInicio,
            HoraFim = t.HoraFim,
            DuracaoMinutos = t.DuracaoMinutos,
            Motivo = t.Motivo,
            Status = t.Status,
            AnalistaId = t.AnalistaId,
            ProjetoId = t.ProjetoId,
            Observacoes = t.Observacoes,
            Ativo = t.Ativo
        };
    }
}

