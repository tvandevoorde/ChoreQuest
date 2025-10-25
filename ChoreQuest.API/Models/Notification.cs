namespace ChoreQuest.API.Models;

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? RelatedChoreId { get; set; }
}

public enum NotificationType
{
    ChoreAssigned,
    ChoreDueSoon,
    ChoreCompleted,
    ListShared,
    ChoreOverdue
}
