namespace UsersChat.Models;

public class Message
{
    public long Id { get; set; }

    public long DialogId { get; set; }
    public Dialog? Dialog { get; set; }

    public long SenderId { get; set; }
    public User? Sender { get; set; }

    public string Text { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}