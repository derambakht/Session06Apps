using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleAPI.CustomAttributes;

namespace SampleAPI.Controllers;
[ApiController]
[Route("[controller]")]
[Authorize]
[CheckPermissoin]
public class BaseController:ControllerBase{
    
}