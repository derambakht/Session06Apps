using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Utility;

public class EncryptionUtility
{
    private readonly IConfiguration configuration;

    public EncryptionUtility(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public string GenerateRefreshToken()
    {
       return Guid.NewGuid().ToString();
    }

    public string GenerateToken(Guid userId)
    {
        var tokenTimeout = Convert.ToInt32(configuration["Security:TokenTimeout"]);
        var tokenKey = configuration["Security:TokenKey"];

        // Else we generate JSON Web Token
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.UTF8.GetBytes(tokenKey);
		var tokenDescriptor = new SecurityTokenDescriptor
		{
		  Subject = new ClaimsIdentity(new Claim[]
		  {
			 new Claim(ClaimTypes.Name, userId.ToString())                    
		  }),
		   Expires = DateTime.UtcNow.AddMinutes(tokenTimeout),
		   SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
		};
		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
    }

    public string HashPassword(string password, string salt)
    {
        var temp = password + salt;
        return HashSHA256(temp);
    }

    public string HashSHA256(string input)
        {
            using (var sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
}