using System.ComponentModel.DataAnnotations;

namespace LU2_WebApi.Models;
public class Environment2D
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [Range(0, 10000, ErrorMessage = "MaxLength must be between 0 and 10,000.")]
    public int MaxLength { get; set; }

    [Required]
    [Range(0, 10000, ErrorMessage = "MaxHeight must be between 0 and 10,000.")]
    public int MaxHeight { get; set; }
}
