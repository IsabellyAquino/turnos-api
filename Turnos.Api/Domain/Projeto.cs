
// Domain/Projeto.cs
using System.ComponentModel.DataAnnotations;

namespace Turnos.Api.Domain
{
    /// <summary>Projeto ao qual um turno pode ser vinculado (opcional).</summary>
    public class Projeto
    {
        public int Id { get; set; }

        /// <summary>Nome do projeto (obrigatório).</summary>
        [Required]
        public string Nome { get; set; } = default!;

        /// <summary>Indica se o projeto pode ser utilizado (vínculo com turnos).</summary>
        public bool Ativo { get; set; } = true;

        /// <summary>Navegação 1:N com turnos (não é obrigatório usar agora).</summary>
        public List<Turno>? Turnos { get; set; }
    }
}
