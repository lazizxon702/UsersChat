using System.Collections.Generic;
using System.Threading.Tasks;
using UsersChat.DTOs;
using UsersChat.Models.Response;

namespace UsersChat.Interface;

public interface IDialogService
{

 
    Task<DefaultResponse<long>> GetOrCreateDialogAsync(long myUserId, long otherUserId);

  
    Task<DefaultResponse<MessageDto>> SendMessageAsync(long myUserId, SendMessageDto dto);


    Task<DefaultResponse<List<MessageDto>>> GetMessagesAsync(long myUserId, long dialogId, int take = 50);


}