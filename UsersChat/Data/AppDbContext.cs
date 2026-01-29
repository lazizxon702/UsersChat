using Microsoft.EntityFrameworkCore;
using UsersChat.Models;

namespace UsersChat;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Dialog> Dialogs => Set<Dialog>();
    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

 
        modelBuilder.Entity<Dialog>()
            .HasIndex(x => new { x.User1Id, x.User2Id })
            .IsUnique();

        modelBuilder.Entity<Message>()
            .HasOne(x => x.Dialog)
            .WithMany(x => x.Messages)
            .HasForeignKey(x => x.DialogId);

       
        modelBuilder.Entity<Message>()
            .HasOne(x => x.Sender)
            .WithMany(x => x.Messages)
            .HasForeignKey(x => x.SenderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
