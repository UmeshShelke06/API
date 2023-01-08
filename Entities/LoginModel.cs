using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class LoginModel
    {

    [Key]
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public byte[]? PasswordHash { get; set; }

    public string? UserEmail { get; set; }

    public byte[]? PasswordSalt { get; set; }

    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

    public ICollection<AppUserRoles> UserRoles { get; set; }
        
    }
}