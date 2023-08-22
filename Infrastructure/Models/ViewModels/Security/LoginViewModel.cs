namespace Infrastructure.ViewModels.Security;

public class LoginViewModel
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public int TokenTimeout { get; set; }
    public string FullName { get; set; }
}

public class GenerateTokenViewModel
{
     public string Token { get; set; }
    public string RefreshToken { get; set; }
}