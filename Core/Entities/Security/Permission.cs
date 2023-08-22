namespace Core.Entities.Security;

public class Permission
{
    public int Id { get; set; }
    public string GroupName { get; set; }
    public string Title { get; set; }
    public string ControllerName { get; set; }
    public string ActionName { get; set; }
    //Get, Post, Put, Delete
    /// <summary>
    /// Get,Post,Put,Delete
    /// </summary>
    public string MethodType { get; set; }
}