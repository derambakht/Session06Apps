namespace Core.Entities.Security;

public class UserRefreshToken
{
    public Guid Id {get; set;}
    public string RefreshToken {get;  set;}
    public DateTime CreateDate { get; set;}
    public bool IsValid { get; set; }
    public Guid UserId { get; set; }
    //Navigation Property
    public virtual User User { get; set; }
}