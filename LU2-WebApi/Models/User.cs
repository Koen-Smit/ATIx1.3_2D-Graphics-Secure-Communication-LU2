using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [Range(1, 25, ErrorMessage = "Username must be between 1-25 characters")]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string NormalizedUserName { get; set; } = string.Empty;

    public virtual ICollection<UserClaim> UserClaims { get; set; } = new HashSet<UserClaim>();
}
public class UserClaim
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string ClaimType { get; set; } = string.Empty;

    [Required]
    [StringLength(250)]
    public string ClaimValue { get; set; } = string.Empty;
}
public class UserClaimDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string ClaimType { get; set; } = string.Empty;
    public string ClaimValue { get; set; } = string.Empty;
}