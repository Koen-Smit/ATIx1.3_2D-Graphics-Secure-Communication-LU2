using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

public class AccountViewModel
{
    public IdentityUser? User { get; set; }
    public IList<Claim>? ClaimsIdentity { get; set; }
}
