using LU2_WebApi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Object2D
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string PrefabId { get; set; } = string.Empty;

    public float? PositionX { get; set; }
    public float? PositionY { get; set; }

    public float? ScaleX { get; set; }
    public float? ScaleY { get; set; }

    public float? RotationZ { get; set; }

    public int? SortingLayer { get; set; }

    // Foreign Key
    public int Environment2D_Id { get; set; }

    [ForeignKey("Environment2DId")]
    public Environment2D Environment { get; set; } = null!;
}
