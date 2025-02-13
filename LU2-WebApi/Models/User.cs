using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
    public ICollection<Environment2D> Environments { get; set; } = new List<Environment2D>();
}
