
using System.ComponentModel.DataAnnotations;

namespace Turnos.Api.Domain
{
    /// <summary>Representa um analista do time.</summary>
    public class Analista
    {
        public int Id { get; set; }

        /// <summary>Nome é opcional.</summary>
        public string? Nome { get; set; }

        /// <summary>Email é obrigatório e único.</summary>
        [Required, EmailAddress]
        public string Email { get; set; } = default!;

        /// <summary>Define se pode receber turnos.</summary>
        public bool Ativo { get; set; } = true;

        /// <summary>Relação 1:N com Turnos (apenas navegação, não obrigatório usar já).</summary>
        public List<Turno>? Turnos { get; set; }
    }
}
