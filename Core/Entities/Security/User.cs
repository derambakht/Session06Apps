using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Security;

[Table("Users", Schema = "Security")]
public class User
{
    public Guid Id {get; set;}
    public string UserName {get; set;}
    public string Password { get; set; }
    public string PasswordSalt { get; set; }
    public string FirstName {get; set;}
    public string LastName {get; set;}
    public bool IsActive {get; set;}

    public virtual ICollection<UserRefreshToken> RefreshTokens {get; set;}
}