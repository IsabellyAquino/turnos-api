
using Turnos.Api.Domain;

namespace Turnos.Api.DTOs
{
    /// <summary>Dados para criar um turno (request).</summary>
    public class TurnoCreateRequest
    {
        public DateTime Data { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFim { get; set; }
        public string Motivo { get; set; } = default!;
        public StatusTurno Status { get; set; } = StatusTurno.Pendente;
        public int AnalistaId { get; set; }
        public int? ProjetoId { get; set; }
        public string? Observacoes { get; set; }
        public bool Ativo { get; set; } = true;
    }

    /// <summary>Filtros para listar turnos (querystring).</summary>
    public class TurnoFilterQuery
    {
        public int? AnalistaId { get; set; }
        public int? ProjetoId { get; set; }
        public StatusTurno? Status { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    /// <summary>Resposta simplificada de turno (response).</summary>
    public class TurnoResponse
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFim { get; set; }
        public int DuracaoMinutos { get; set; }
        public string Motivo { get; set; } = default!;
        public StatusTurno Status { get; set; }
        public int AnalistaId { get; set; }
        public int? ProjetoId { get; set; }
        public string? Observacoes { get; set; }
        public bool Ativo { get; set; }
    }
}
