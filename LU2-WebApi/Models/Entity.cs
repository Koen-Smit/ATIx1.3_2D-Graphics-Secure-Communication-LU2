using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Entity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    public string Prefab_Id { get; set; } = string.Empty;

    [Range(-10000, 10000)]
    public float? PositionX { get; set; }

    [Range(-10000, 10000)]
    public float? PositionY { get; set; }

    [Range(0.1, 10.0)]
    public float? ScaleX { get; set; }

    [Range(0.1, 10.0)]
    public float? ScaleY { get; set; }

    [Range(-360, 360)]
    public float? RotationZ { get; set; }

    public int? SortingLayer { get; set; }

    [Required]
    public Guid EnvironmentId { get; set; }

    [ForeignKey("EnvironmentId")]
    public virtual Environment Environment { get; set; } = null!;
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}

public class EntityDTO
{
    public Guid Id { get; set; }
    public string Prefab_Id { get; set; } = string.Empty;
    public float? PositionX { get; set; }
    public float? PositionY { get; set; }
    public float? ScaleX { get; set; }
    public float? ScaleY { get; set; }
    public float? RotationZ { get; set; }
    public int? SortingLayer { get; set; }
    public Guid EnvironmentId { get; set; }
}
public class EntityRequest
{
    [Required]
    public string Prefab_Id { get; set; } = string.Empty;

    public float? PositionX { get; set; }
    public float? PositionY { get; set; }
    public float? ScaleX { get; set; }
    public float? ScaleY { get; set; }
    public float? RotationZ { get; set; }
    public int? SortingLayer { get; set; }
    [Required]
    public Guid EnvironmentId { get; set; }
}

public class EntityUpdateRequest
{
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float ScaleX { get; set; }
    public float ScaleY { get; set; }
    public float RotationZ { get; set; }
    public int? SortingLayer { get; set; }
}


