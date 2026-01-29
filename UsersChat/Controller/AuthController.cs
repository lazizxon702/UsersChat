using Microsoft.AspNetCore.Mvc;
using UsersChat.DTOs;
using UsersChat.Interface;
using UsersChat.Models.Response;

namespace UsersChat.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<DefaultResponse<string>> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
        {
            var error = new ErrorResponse("Kechirasiz bunday Login topilmadi!!!", (int)ResponseCode.BadRequest);
            return new DefaultResponse<string>(error);
        }

        var result = await authService.Login(dto);
        return result;
    }
    
    [HttpPost("register")]
    public async Task<DefaultResponse<string>> Register([FromBody] RegisterDto dto)
    {
        var result = await authService.Register(dto);
        return result;
    }
    
}