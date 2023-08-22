
using Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace SampleAPI.Controllers;

public class CompanyWithLayeringController:BaseController
{
    private readonly ICompanyService companyService;

    public CompanyWithLayeringController(ICompanyService companyService)
    {
        this.companyService = companyService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await companyService.GetById(id);
        return Ok(result);
    }

}