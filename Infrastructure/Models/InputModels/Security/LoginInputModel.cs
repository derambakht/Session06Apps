namespace Infrastructure.InputModels.Security;

public class LoginInputModel
{
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class GenerteNewTokenInputModel
{
    public string RefreshToken { get; set; }
}