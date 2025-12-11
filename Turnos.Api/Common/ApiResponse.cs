
namespace Turnos.Api.Common
{
    /// <summary>
    /// Envelope padrão para respostas da API, garantindo consistência.
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }          // Operação deu certo?
        public string? Message { get; set; }       // Mensagem amigável (contexto)
        public List<string>? Errors { get; set; }  // Lista de erros (validação, negócio)
        public T? Data { get; set; }               // Dados quando sucesso
        public PaginationInfo? Pagination { get; set; } // Metadados de paginação

        public static ApiResponse<T> Ok(T data, string? message = null, PaginationInfo? pagination = null)
            => new() { Success = true, Data = data, Message = message, Pagination = pagination };

        public static ApiResponse<T> Fail(List<string> errors, string? message = null)
            => new() { Success = false, Message = message, Errors = errors };
    }

    /// <summary>Informações de paginação para listas.</summary>
    public class PaginationInfo
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}
