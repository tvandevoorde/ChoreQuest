namespace ChoreQuest.API.Models;

public class ChoreList
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int OwnerId { get; set; }
    public User Owner { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<Chore> Chores { get; set; } = new List<Chore>();
    public ICollection<ChoreListShare> Shares { get; set; } = new List<ChoreListShare>();
}
