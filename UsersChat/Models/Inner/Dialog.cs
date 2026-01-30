using System;
using System.Collections.Generic;

namespace UsersChat.Models;

public class Dialog
{
    public long Id { get; set; }

   
    public long User1Id { get; set; }
    public User? User1 { get; set; }

    public long User2Id { get; set; }
    public User? User2 { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public IEnumerable<Message>? Messages { get; set; } 
}
