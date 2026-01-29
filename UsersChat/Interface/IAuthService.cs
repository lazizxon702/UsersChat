using UsersChat.DTOs;
using UsersChat.Models.Response;

namespace UsersChat.Interface;

public interface IAuthService
{
    Task<DefaultResponse<string>> Login(LoginDto dto);
    Task<DefaultResponse<string>> Register(RegisterDto dto);
}