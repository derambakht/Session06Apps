using Infrastructure.Common;
using Infrastructure.InputModels.Security;
using Infrastructure.ViewModels.Security;

namespace Application.IServices.Security;

public interface IAuthenticateService
{
    Task<MellatActionResult<RegisterResultViewModel>> Register(UserRegisterInputModel model);
    Task<MellatActionResult<LoginViewModel>> Login(LoginInputModel model);
    Task<MellatActionResult<GenerateTokenViewModel>> GenerateNewToken(GenerteNewTokenInputModel model);
}