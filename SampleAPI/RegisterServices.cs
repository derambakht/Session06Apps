using System.Text;
using Application.IServices;
using Application.IServices.Security;
using Application.Services;
using Application.Services.Securrity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace SampleAPI;
public static class RegisterServices
{   
    public static IServiceCollection AddApplicationService(this IServiceCollection services)
    {
        services.AddScoped<ICompanyService, EFCompanyService>();
        services.AddScoped<IAuthenticateService, AuthenticateService>();
        return services;
    }

     public static IServiceCollection AddJWT(this IServiceCollection services)
        {
            var sp = services.BuildServiceProvider();
         

            var configuration = sp.GetService<IConfiguration>();
            var key = Encoding.UTF8.GetBytes(configuration["Security:TokenKey"]);
            var timeOut = Convert.ToUInt32(configuration["Security:TokenTimeout"]);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    //برای کنترل زمان توکن
                    ClockSkew = TimeSpan.FromMinutes(timeOut),
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            return services;
        }
}