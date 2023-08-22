using Application.IServices.Security;
using Core;
using Core.Entities.Security;
using Infrastructure.Common;
using Infrastructure.InputModels.Security;
using Infrastructure.Resources;
using Infrastructure.Utility;
using Infrastructure.ViewModels.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client.Extensibility;

namespace Application.Services.Securrity;

public class AuthenticateService : IAuthenticateService
{
    private readonly SampleDbContext dbContext;
    private readonly EncryptionUtility encryptionUtility;
    private readonly IConfiguration configuration;
    private IMemoryCache memoryCache;

    public AuthenticateService(SampleDbContext dbContext, EncryptionUtility encryptionUtility, IConfiguration configuration, IMemoryCache memoryCache)
    {
        this.dbContext = dbContext;
        this.encryptionUtility = encryptionUtility;
        this.configuration = configuration;
        this.memoryCache = memoryCache;
    }

    public async Task<MellatActionResult<GenerateTokenViewModel>> GenerateNewToken(GenerteNewTokenInputModel model)
    {
        var result = new MellatActionResult<GenerateTokenViewModel>();

         var refreshTokenTimeout = Convert.ToInt32(configuration["Security:RefreshTokenTimeout"]);

         var userRefreshToken = await dbContext.UserRefreshTokens
         .FirstOrDefaultAsync(q => q.RefreshToken == model.RefreshToken);

         if(userRefreshToken == null)
         {
            result.IsSuccess = false;
            result.Message = "توکن صحیح ارسال کنید";
            return result;
         }

         if(!userRefreshToken.IsValid)
         {
            result.IsSuccess = false;
            result.Message = "مجدد لاگین بفرمایید";
            return result;
         }

         //createDate 13:45 => 13:45 + 10 = 13:55 < 14:00

         if(userRefreshToken.CreateDate.AddMinutes(refreshTokenTimeout) < DateTime.Now)
         {
            result.IsSuccess = false;
            result.Message = "زمان رفرش توکن شما گذشته است";
            return result;
         }

         string refreshToken = encryptionUtility.GenerateRefreshToken();
         userRefreshToken.RefreshToken = refreshToken;
         userRefreshToken.CreateDate = DateTime.Now;
         await dbContext.SaveChangesAsync();

         var resultModel = new GenerateTokenViewModel
         {
            RefreshToken = refreshToken,
            Token = encryptionUtility.GenerateToken(userRefreshToken.UserId),
         };

         result.IsSuccess = true;
         result.Data = resultModel;

         return result;
    }

    public async Task<MellatActionResult<LoginViewModel>> Login(LoginInputModel model)
    {
        var result = new MellatActionResult<LoginViewModel>();

        //validation

        var user = await dbContext.Users.SingleOrDefaultAsync(q => q.UserName == model.UserName);
        if(user == null)
        {
            result.IsSuccess = false;
            result.Message = "نام کاربری صحیح نمی باشد";
            return result;
        }

        var hashPassword = encryptionUtility.HashPassword(model.Password, user.PasswordSalt);
        if(hashPassword != user.Password)
        {
              result.IsSuccess = false;
            result.Message = "رمز عبور صحیح نمی باشد";
            return result;
        }
        
        var refreshToken = encryptionUtility.GenerateRefreshToken();
        var userRefreshToken  = await dbContext.UserRefreshTokens
        .FirstOrDefaultAsync(q => q.UserId == user.Id);
        if(userRefreshToken != null)
        {
            //update Refresh Token
            userRefreshToken.CreateDate = DateTime.Now;
            userRefreshToken.RefreshToken = refreshToken;
            userRefreshToken.IsValid = true;
            await dbContext.SaveChangesAsync();
        } else 
        {
            //insert Refresh Token
            var newUserRefreshToken = new UserRefreshToken
            {
                CreateDate = DateTime.Now,
                Id = Guid.NewGuid(),
                IsValid = true,
                RefreshToken = refreshToken,
                UserId = user.Id,
            };
            await dbContext.UserRefreshTokens.AddAsync(newUserRefreshToken);
            await dbContext.SaveChangesAsync();
        }
       

        var userResult = new LoginViewModel
        {
            FullName = $"{user.FirstName} {user.LastName}",
            UserId = user.Id,
            Token = encryptionUtility.GenerateToken(user.Id),
            RefreshToken = refreshToken,
            //TokenTimeout = "",
        };

        result.IsSuccess = true;
        result.Data = userResult;
        return result;
    }

    public async Task<MellatActionResult<RegisterResultViewModel>> Register(UserRegisterInputModel model)
    {
        var result = new MellatActionResult<RegisterResultViewModel>();

        //validation

        //already user exists
        if(await dbContext.Users.AnyAsync(q => q.UserName == model.UserName))
        {
            result.IsSuccess = false;
            result.Message = "نام کاربری تکرار است";
            return result;
        }

        var user = new User();
        user.Id = Guid.NewGuid();
        user.UserName = model.UserName;
        user.IsActive = true;
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.PasswordSalt  = Guid.NewGuid().ToString();
        user.Password = encryptionUtility.HashPassword(model.Password, user.PasswordSalt);

        await dbContext.AddAsync(user);
        //await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = new RegisterResultViewModel {UserId = user.Id};
        return result;
    }
}