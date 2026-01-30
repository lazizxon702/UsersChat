using System;

namespace UsersChat.DTOs;

public class MessageDto
{
    public long Id { get; set; }
    public long DialogId { get; set; }

    public long SenderId { get; set; }
    public string Text { get; set; } = "";

    public DateTime CreatedAt { get; set; }
}