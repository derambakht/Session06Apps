namespace Core.Entities.Security;

public class UserPermission
{
    public int Id {get; set;}
    public Guid UserId {get; set;}
    public virtual User User { get; set;}
    public int PermissionId {get; set;}
    public virtual Permission Permission { get; set;}
}