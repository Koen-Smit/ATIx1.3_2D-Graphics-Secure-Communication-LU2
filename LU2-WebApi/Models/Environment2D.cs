using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Environment2D
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    // Naam Environment: 1-25 characters, max 5 per user
    [Required]
    [Range(1, 25, ErrorMessage = "Name cannot be longer than 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(20, 200, ErrorMessage = "MaxLength must be between 20 and 200.")]
    public int MaxLength { get; set; }

    [Required]
    [Range(10, 100, ErrorMessage = "MaxHeight must be between 10 and 100.")]
    public int MaxHeight { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    public int? EnvironmentType { get; set; }
}

public class Environment2DDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MaxLength { get; set; }
    public int MaxHeight { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? EnvironmentType { get; set; }
}

public class CreateEnvironmentRequest
{
    [Required]
    [StringLength(25, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 25 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int EnvironmentType { get; set; }
}
