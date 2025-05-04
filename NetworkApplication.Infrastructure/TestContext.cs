using Microsoft.EntityFrameworkCore;
using NetworkApplication.ChatCommon;
using NetworkApplication.Infrastructure;

public class TestContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<ChatMessage> Messages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // Используем SQLite (или другую БД)
        options.UseSqlite("Data Source=chat.db");

        // Для SQL Server:
        // options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ChatDb;Trusted_Connection=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //// Настройка связей
        //modelBuilder.Entity<ChatMessage>()
        //    .HasOne(m => m.FromUser)
        //    .WithMany(u => u.Messages)
        //    .HasForeignKey(m => m.FromUserId)
        //    .OnDelete(DeleteBehavior.Restrict);

        //modelBuilder.Entity<ChatMessage>()
        //    .HasOne(m => m.ToUser)
        //    .WithMany()
        //    .HasForeignKey(m => m.ToUserId)
        //    .OnDelete(DeleteBehavior.Restrict);
    }
}