namespace ChoreQuest.API.DTOs;

public class ShareChoreListDto
{
    public int SharedWithUserId { get; set; }
    public string Permission { get; set; } = "View";
}

public class NotificationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? RelatedChoreId { get; set; }
}
