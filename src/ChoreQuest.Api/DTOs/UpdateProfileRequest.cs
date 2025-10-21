using System.ComponentModel.DataAnnotations;

namespace ChoreQuest.Api.DTOs;

public class UpdateProfileRequest
{
    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }
}
