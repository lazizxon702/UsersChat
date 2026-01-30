using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;

namespace UsersChat.Models;

public class User
{
    public long Id { get; set; }

    public string Username { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string PasswordHash { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigations (shart emas, lekin qulay)
    public IEnumerable<Message>? Messages { get; set; } 
}