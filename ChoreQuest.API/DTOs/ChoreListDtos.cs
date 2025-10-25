namespace ChoreQuest.API.DTOs;

public class CreateChoreListDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class UpdateChoreListDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class ChoreListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int OwnerId { get; set; }
    public string OwnerUsername { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int ChoreCount { get; set; }
    public int CompletedChoreCount { get; set; }
    public List<ShareDto> Shares { get; set; } = new();
}

public class ShareDto
{
    public int Id { get; set; }
    public int SharedWithUserId { get; set; }
    public string SharedWithUsername { get; set; } = string.Empty;
    public string Permission { get; set; } = string.Empty;
    public DateTime SharedAt { get; set; }
}
