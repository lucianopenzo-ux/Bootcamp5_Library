using System.ComponentModel.DataAnnotations;

namespace TheLibrary.Common.DTOs
{
    public class BookCreateDTO
    {
        [Required]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "El título debe tener entre 1 y 200 caracteres.")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "El autor debe tener entre 1 y 100 caracteres.")]
        public string Author { get; set; } = string.Empty;

        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "El ISBN debe tener entre 3 y 20 caracteres.")]
        public string ISBN { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe tener un año")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "El año debe tener 4 dígitos.")]
        public string Year { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "LibraryBranchId debe ser mayor a cero.")]
        public int LibraryBranchId { get; set; }
    }
}
