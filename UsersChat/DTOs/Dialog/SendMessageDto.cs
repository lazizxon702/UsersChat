namespace UsersChat.DTOs;

public class SendMessageDto
{
    public long DialogId { get; set; }
    public string Text { get; set; } = "";
}