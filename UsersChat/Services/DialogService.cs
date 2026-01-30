using Microsoft.EntityFrameworkCore;
using UsersChat.DTOs;
using UsersChat.Interface;
using UsersChat.Models;
using UsersChat.Models.Response;

namespace UsersChat.Services;

public class DialogService(AppDbContext db , IConfiguration config) : IDialogService
{
    public async Task<DefaultResponse<long>> GetOrCreateDialogAsync(long myUserId, long otherUserId)
    {
        try
        {
            if (myUserId == otherUserId)
            {
                var error = new ErrorResponse(
                    "O‘zingiz bilan chat ochib bo‘lmaydi",
                    (int)ResponseCode.ValidationError);

                return new DefaultResponse<long>(error);
            }

            var otherExists = await db.Users.AnyAsync(u => u.Id == otherUserId);
            if (!otherExists)
            {
                var error = new ErrorResponse(
                    "Bunday foydalanuvchi topilmadi",
                    (int)ResponseCode.NotFound);

                return new DefaultResponse<long>(error);
            }

            var u1 = Math.Min(myUserId, otherUserId);
            var u2 = Math.Max(myUserId, otherUserId);

            var dialog = await db.Dialogs
                .FirstOrDefaultAsync(d => d.User1Id == u1 && d.User2Id == u2);

            if (dialog != null)
            {
                return new DefaultResponse<long>(
                    dialog.Id,
                    "Mavjud dialog topildi");
            }

            var newDialog = new Dialog
            {
                User1Id = u1,
                User2Id = u2,
                CreatedAt = DateTime.UtcNow
            };

            db.Dialogs.Add(newDialog);
            await db.SaveChangesAsync();

            return new DefaultResponse<long>(
                newDialog.Id,
                "Yangi dialog yaratildi");
        }
        catch
        {
            var error = new ErrorResponse(
                "Dialog yaratishda xatolik yuz berdi",
                (int)ResponseCode.ServerError);

            return new DefaultResponse<long>(error);
        }
    }

    public async Task<DefaultResponse<MessageDto>> SendMessageAsync(long myUserId, SendMessageDto dto)
    {
        
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Text))
            {
                var error = new ErrorResponse("Xabar bo‘sh bo‘lmasin", (int)ResponseCode.ValidationError);
                return new DefaultResponse<MessageDto>(error);
            }

            var dialog = await db.Dialogs.FirstOrDefaultAsync(d => d.Id == dto.DialogId);
            if (dialog == null)
            {
                var error = new ErrorResponse("Dialog topilmadi", (int)ResponseCode.NotFound);
                return new DefaultResponse<MessageDto>(error);
            }

            var isMember = dialog.User1Id == myUserId || dialog.User2Id == myUserId;
            if (!isMember)
            {
                var error = new ErrorResponse("Siz bu dialogga tegishli emassiz", (int)ResponseCode.Forbidden);
                return new DefaultResponse<MessageDto>(error);
            }

            var message = new Message
            {
                DialogId = dto.DialogId,
                SenderId = myUserId,
                Text = dto.Text.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            db.Messages.Add(message);
            await db.SaveChangesAsync();

            var result = new MessageDto
            {
                Id = message.Id,
                DialogId = message.DialogId,
                SenderId = message.SenderId,
                Text = message.Text,
                CreatedAt = message.CreatedAt
            };

            return new DefaultResponse<MessageDto>(result, "Xabar yuborildi");
        }
        catch
        {
            var error = new ErrorResponse("Xabar yuborishda xatolik yuz berdi", (int)ResponseCode.ServerError);
            return new DefaultResponse<MessageDto>(error);
        }
    }


    public async Task<DefaultResponse<List<MessageDto>>> GetMessagesAsync(long myUserId, long dialogId, int take = 50)
    {
       try
       {
        if (dialogId <= 0)
        {
            var error = new ErrorResponse(
                "DialogId noto‘g‘ri",
                (int)ResponseCode.ValidationError);

            return new DefaultResponse<List<MessageDto>>(error);
        }

        if (take <= 0) take = 50;
        if (take > 200) take = 200;

        var dialog = await db.Dialogs.FirstOrDefaultAsync(d => d.Id == dialogId);
        if (dialog == null)
        {
            var error = new ErrorResponse(
                "Dialog topilmadi",
                (int)ResponseCode.NotFound);

            return new DefaultResponse<List<MessageDto>>(error);
        }

        var isMember = dialog.User1Id == myUserId || dialog.User2Id == myUserId;
        if (!isMember)
        {
            var error = new ErrorResponse(
                "Siz bu dialogga tegishli emassiz",
                (int)ResponseCode.Forbidden);
                                                                     
             return new DefaultResponse<List<MessageDto>>(error);
        }
                                                                 
        var messages = await db.Messages
            .Where(m => m.DialogId == dialogId)
            .OrderByDescending(m => m.CreatedAt)
            .Take(take)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                DialogId = m.DialogId,
                SenderId = m.SenderId,
                Text = m.Text,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync();
                                                                                    
        messages.Reverse();
                                                                            
        return new DefaultResponse<List<MessageDto>>(
            messages,
            "Xabarlar muvaffaqiyatli olindi");
       }
       catch
       {
           var error = new ErrorResponse(
               "Xabarlarni olishda xatolik yuz berdi",
               (int)ResponseCode.ServerError);
                                                                    
           return new DefaultResponse<List<MessageDto>>(error);
       }
    }
}