using Application.IServices.Security;
using Infrastructure.InputModels.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleAPI.Controllers;

namespace SampleAPI.Controllers.Security;

[ApiController]
[Route("[controller]")]
public class AuthenticateController : Controller {

    private readonly IAuthenticateService authenticateService;

    public AuthenticateController(IAuthenticateService authenticateService)
    {
        this.authenticateService = authenticateService;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromForm] UserRegisterInputModel model)
    {
        var result = await authenticateService.Register(model);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromForm] LoginInputModel model)
    {
        var result  = await authenticateService.Login(model);
        return Ok(result);
    }

    [HttpPost("GenerateNewToken")]
    public async Task<IActionResult> GenerateNewToken([FromForm] GenerteNewTokenInputModel model)
    {
        var result = await authenticateService.GenerateNewToken(model);
        return Ok(result);
    }

}