
using System.ComponentModel.DataAnnotations;

namespace Turnos.Api.Domain
{
    /// <summary>Registro de turno (atividade) do analista.</summary>
    public class Turno
    {
        public int Id { get; set; }

        /// <summary>Data do turno (apenas data, horário separado).</summary>
        [Required]
        public DateTime Data { get; set; }

        /// <summary>Hora de início do turno.</summary>
        [Required]
        public TimeSpan HoraInicio { get; set; }

        /// <summary>Hora de fim do turno.</summary>
        [Required]
        public TimeSpan HoraFim { get; set; }

        /// <summary>Duração calculada em minutos.</summary>
        [Range(1, int.MaxValue)]
        public int DuracaoMinutos { get; set; }

        /// <summary>Motivo/descrição da atividade.</summary>
        [Required]
        public string Motivo { get; set; } = default!;

        /// <summary>Status atual do turno.</summary>
        [Required]
        public StatusTurno Status { get; set; } = StatusTurno.Pendente;

        /// <summary>FK obrigatória para Analista.</summary>
        [Required]
        public int AnalistaId { get; set; }
        public Analista? Analista { get; set; }

        /// <summary>FK opcional para Projeto.</summary>
        public int? ProjetoId { get; set; }
        public Projeto? Projeto { get; set; }

        /// <summary>Observações adicionais.</summary>
        public string? Observacoes { get; set; }

        /// <summary>Ativo=false indica cancelamento mantendo histórico.</summary>
        public bool Ativo { get; set; } = true;
    }
}
