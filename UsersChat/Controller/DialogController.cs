using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersChat.DTOs;
using UsersChat.Interface;
using UsersChat.Models.Response;

namespace UsersChat.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DialogController : ControllerBase
{
    private readonly IDialogService _dialogService;

    public DialogController(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    private long GetMyUserId()
    {
        var idStr = User.FindFirstValue("UserId")
                    ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(idStr) || !long.TryParse(idStr, out var myId))
            throw new UnauthorizedAccessException("Token ichida UserId yo‘q yoki noto‘g‘ri");

        return myId;
    }

    [HttpPost("get-or-create")]
    public async Task<DefaultResponse<long>> GetOrCreate([FromQuery] long otherUserId)
    {
        var myUserId = GetMyUserId();
        return await _dialogService.GetOrCreateDialogAsync(myUserId, otherUserId);
    }

    [HttpPost("send")]
    public async Task<DefaultResponse<MessageDto>> Send([FromBody] SendMessageDto dto)
    {
        var myUserId = GetMyUserId();
        return await _dialogService.SendMessageAsync(myUserId, dto);
    }

    [HttpGet("{dialogId:long}/messages")]
    public async Task<DefaultResponse<List<MessageDto>>> GetMessages(
        [FromRoute] long dialogId,
        [FromQuery] int take = 50)
    {
        var myUserId = GetMyUserId();
        return await _dialogService.GetMessagesAsync(myUserId, dialogId, take);
    }
}