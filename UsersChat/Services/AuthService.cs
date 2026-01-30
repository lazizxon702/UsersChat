using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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


    public async Task<DefaultResponse<string>> Register(RegisterDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Username) ||
                string.IsNullOrWhiteSpace(dto.PhoneNumber) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                var error = new ErrorResponse("Maydonlar bo‘sh bo‘lmasin", (int)ResponseCode.ValidationError);
                return new DefaultResponse<string>(error);
            }

            var exists = await db.Users.AnyAsync(u => u.PhoneNumber == dto.PhoneNumber);
            if (exists)
            {
                var error = new ErrorResponse("Bu telefon raqam avval ro‘yxatdan o‘tgan", (int)ResponseCode.Conflict);
                return new DefaultResponse<string>(error);
            }

            var user = new User
            {
                Username = dto.Username.Trim(),
                PhoneNumber = dto.PhoneNumber.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            return new DefaultResponse<string>(token, "Ro'yxatdan o'tish muvaffaqiyatli!");
        }
        catch
        {
            var error = new ErrorResponse("Register jarayonida xatolik yuz berdi", (int)ResponseCode.ServerError);
            return new DefaultResponse<string>(error);
        }
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