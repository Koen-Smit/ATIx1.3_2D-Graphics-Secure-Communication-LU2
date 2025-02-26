using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Share
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    public Guid SharedUserId { get; set; }

    [Required]
    public Guid EnvironmentId { get; set; }

    [ForeignKey("SharedUserId")]
    public virtual User SharedUser { get; set; } = null!;

    [ForeignKey("EnvironmentId")]
    public virtual Environment Environment { get; set; } = null!;
}

public class ShareDTO
{
    public Guid EnvironmentId { get; set; }
    public string EnvironmentName { get; set; } = string.Empty;
}

public class ShareRequest
{
    public string SharedUserName { get; set; } = string.Empty;
    public Guid WorldId { get; set; }
}
