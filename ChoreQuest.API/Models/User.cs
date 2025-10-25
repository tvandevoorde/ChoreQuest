namespace ChoreQuest.API.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<ChoreList> OwnedChoreLists { get; set; } = new List<ChoreList>();
    public ICollection<ChoreListShare> SharedChoreLists { get; set; } = new List<ChoreListShare>();
    public ICollection<Chore> AssignedChores { get; set; } = new List<Chore>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
