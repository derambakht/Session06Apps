namespace Infrastructure.InputModels.Security;

public class UserRegisterInputModel
{
    public string UserName {get; set;}
    public string Password {get; set;}
    public string ConfirmPassword {get; set;}
    public string FirstName {get; set;}
    public string LastName {get; set;}
}