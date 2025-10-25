using Microsoft.EntityFrameworkCore;
using ChoreQuest.API.Models;

namespace ChoreQuest.API.Data;

public class ChoreQuestDbContext : DbContext
{
    public ChoreQuestDbContext(DbContextOptions<ChoreQuestDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<ChoreList> ChoreLists { get; set; }
    public DbSet<Chore> Chores { get; set; }
    public DbSet<ChoreListShare> ChoreListShares { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User entity configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
            entity.Property(u => u.PasswordHash).IsRequired();
        });

        // ChoreList entity configuration
        modelBuilder.Entity<ChoreList>(entity =>
        {
            entity.HasKey(cl => cl.Id);
            entity.Property(cl => cl.Name).IsRequired().HasMaxLength(100);
            entity.Property(cl => cl.Description).HasMaxLength(500);
            
            entity.HasOne(cl => cl.Owner)
                .WithMany(u => u.OwnedChoreLists)
                .HasForeignKey(cl => cl.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Chore entity configuration
        modelBuilder.Entity<Chore>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Title).IsRequired().HasMaxLength(200);
            entity.Property(c => c.Description).HasMaxLength(1000);
            
            entity.HasOne(c => c.ChoreList)
                .WithMany(cl => cl.Chores)
                .HasForeignKey(c => c.ChoreListId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(c => c.AssignedTo)
                .WithMany(u => u.AssignedChores)
                .HasForeignKey(c => c.AssignedToId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ChoreListShare entity configuration
        modelBuilder.Entity<ChoreListShare>(entity =>
        {
            entity.HasKey(cls => cls.Id);
            
            entity.HasOne(cls => cls.ChoreList)
                .WithMany(cl => cl.Shares)
                .HasForeignKey(cls => cls.ChoreListId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(cls => cls.SharedWithUser)
                .WithMany(u => u.SharedChoreLists)
                .HasForeignKey(cls => cls.SharedWithUserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasIndex(cls => new { cls.ChoreListId, cls.SharedWithUserId }).IsUnique();
        });

        // Notification entity configuration
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(n => n.Id);
            entity.Property(n => n.Title).IsRequired().HasMaxLength(200);
            entity.Property(n => n.Message).IsRequired().HasMaxLength(1000);
            
            entity.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // PasswordResetToken entity configuration
        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(prt => prt.Id);
            entity.Property(prt => prt.Token).IsRequired();
            
            entity.HasOne(prt => prt.User)
                .WithMany()
                .HasForeignKey(prt => prt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
