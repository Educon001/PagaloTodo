using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Token;
public static class GenerarToken
{
    public static string GenerateToken(LoginResponse result)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("aA1$Bb2&Cc3^Dd4#Ee5!Ff6*Gg7(Hh8)Ii9Jj0");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, result.Id.ToString()),
                new Claim("UserType", result.UserType!)
            }),
            Expires = DateTime.UtcNow.AddMinutes(60),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);
        return tokenString;
    }
}