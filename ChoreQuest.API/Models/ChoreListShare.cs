namespace ChoreQuest.API.Models;

public class ChoreListShare
{
    public int Id { get; set; }
    public int ChoreListId { get; set; }
    public ChoreList ChoreList { get; set; } = null!;
    public int SharedWithUserId { get; set; }
    public User SharedWithUser { get; set; } = null!;
    public SharePermission Permission { get; set; } = SharePermission.View;
    public DateTime SharedAt { get; set; } = DateTime.UtcNow;
}

public enum SharePermission
{
    View,
    Edit,
    Admin
}
