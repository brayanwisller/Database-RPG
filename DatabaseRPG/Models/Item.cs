using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RpgApi.Models;
public class Item{
    [Key]
    public int Id {get; set;}

    [Required(ErrorMessage = "O nome do item é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
    public string Name {get; set;} = string.Empty;

    [Required(ErrorMessage = "A raridade do item é obrigatória.")]
    public string Rarity {get; set;} = "Comum";

    [Required(ErrorMessage = "O preço do item é obrigatório.")]
    [Range(0, 999999, ErrorMessage = "O preço deve ser um valor positivo.")]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal Preco {get; set;}

    public DateTime CriadoEm {get; set;} = DateTime.UtcNow;
}