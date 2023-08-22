using Microsoft.AspNetCore.Http;

namespace Infrastructure.InputModels;

public class ComapnyInputModel
{
    public int? Id {get; set;}
    public string CompanyName {get; set;}
    public string Address {get; set;}
    public string Phone {get; set;}
    public IFormFile Logo {get; set;}
}