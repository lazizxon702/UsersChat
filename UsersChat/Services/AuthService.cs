using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UsersChat.DTOs;
using UsersChat.Interface;
using UsersChat.Models;
using UsersChat.Models.Response;

namespace UsersChat.Services;

public class AuthService(AppDbContext db ,  IConfiguration config ): IAuthService
{
    public async Task<DefaultResponse<string>> Login(LoginDto dto)
    {
        try
        {
            var user = await db.Users.FirstOrDefaultAsync(u =>
                u.PhoneNumber == dto.PhoneNumber /* && !u.IsDeleted */);

            if (user == null)
            {
                var error = new ErrorResponse("Telefon raqam yoki parol noto'g'ri", (int)ResponseCode.Unauthorized);
                return new DefaultResponse<string>(error);
            }

            var passwordOk = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!passwordOk)
            {
                var error = new ErrorResponse("Telefon raqam yoki parol noto'g'ri", (int)ResponseCode.Unauthorized);
                return new DefaultResponse<string>(error);
            }

            var token = GenerateJwtToken(user);

            return new DefaultResponse<string>(token, "Login muvaffaqiyatli!");
        }
        catch
        {
            var error = new ErrorResponse("Login jarayonida xatolik yuz berdi", (int)ResponseCode.ServerError);
            return new DefaultResponse<string>(error);
        }
    }


    public Task<DefaultResponse<string>> Register(RegisterDto dto)
    {
        throw new NotImplementedException();
    }
    
    private string GenerateJwtToken(User user)
    {
        var secretKey = config["Jwt:SecretKey"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("UserId", user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,  
            expires: DateTime.UtcNow.AddHours(2), 
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}