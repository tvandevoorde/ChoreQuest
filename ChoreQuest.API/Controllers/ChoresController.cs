using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChoreQuest.API.Data;
using ChoreQuest.API.DTOs;
using ChoreQuest.API.Models;

namespace ChoreQuest.API.Controllers;

[ApiController]
[Route("api/chorelists/{choreListId}/[controller]")]
public class ChoresController : ControllerBase
{
    private readonly ChoreQuestDbContext _context;

    public ChoresController(ChoreQuestDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChoreDto>>> GetChores(int choreListId)
    {
        var chores = await _context.Chores
            .Include(c => c.AssignedTo)
            .Where(c => c.ChoreListId == choreListId)
            .ToListAsync();

        return Ok(chores.Select(MapToDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ChoreDto>> GetChore(int choreListId, int id)
    {
        var chore = await _context.Chores
            .Include(c => c.AssignedTo)
            .FirstOrDefaultAsync(c => c.Id == id && c.ChoreListId == choreListId);

        if (chore == null)
        {
            return NotFound();
        }

        return Ok(MapToDto(chore));
    }

    [HttpPost]
    public async Task<ActionResult<ChoreDto>> CreateChore(int choreListId, CreateChoreDto dto)
    {
        var choreList = await _context.ChoreLists.FindAsync(choreListId);
        if (choreList == null)
        {
            return NotFound("Chore list not found");
        }

        RecurrencePattern? recurrencePattern = null;
        if (dto.IsRecurring && !string.IsNullOrEmpty(dto.RecurrencePattern))
        {
            if (Enum.TryParse<RecurrencePattern>(dto.RecurrencePattern, true, out var pattern))
            {
                recurrencePattern = pattern;
            }
        }

        var chore = new Chore
        {
            Title = dto.Title,
            Description = dto.Description,
            ChoreListId = choreListId,
            AssignedToId = dto.AssignedToId,
            DueDate = dto.DueDate,
            IsRecurring = dto.IsRecurring,
            RecurrencePattern = recurrencePattern,
            RecurrenceInterval = dto.RecurrenceInterval,
            RecurrenceEndDate = dto.RecurrenceEndDate
        };

        _context.Chores.Add(chore);
        await _context.SaveChangesAsync();

        if (dto.AssignedToId.HasValue)
        {
            var notification = new Notification
            {
                UserId = dto.AssignedToId.Value,
                Title = "Chore Assigned",
                Message = $"You have been assigned to '{dto.Title}'",
                Type = NotificationType.ChoreAssigned,
                RelatedChoreId = chore.Id
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        chore = await _context.Chores
            .Include(c => c.AssignedTo)
            .FirstAsync(c => c.Id == chore.Id);

        return CreatedAtAction(nameof(GetChore), new { choreListId, id = chore.Id }, MapToDto(chore));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ChoreDto>> UpdateChore(int choreListId, int id, UpdateChoreDto dto)
    {
        var chore = await _context.Chores
            .Include(c => c.AssignedTo)
            .FirstOrDefaultAsync(c => c.Id == id && c.ChoreListId == choreListId);

        if (chore == null)
        {
            return NotFound();
        }

        var previousAssignedToId = chore.AssignedToId;

        if (dto.Title != null) chore.Title = dto.Title;
        if (dto.Description != null) chore.Description = dto.Description;
        if (dto.AssignedToId.HasValue) chore.AssignedToId = dto.AssignedToId;
        if (dto.DueDate.HasValue) chore.DueDate = dto.DueDate;
        
        if (dto.IsCompleted.HasValue)
        {
            chore.IsCompleted = dto.IsCompleted.Value;
            if (chore.IsCompleted && !chore.CompletedAt.HasValue)
            {
                chore.CompletedAt = DateTime.UtcNow;
                
                if (chore.IsRecurring && chore.RecurrencePattern.HasValue && chore.DueDate.HasValue)
                {
                    var nextDueDate = CalculateNextDueDate(chore.DueDate.Value, chore.RecurrencePattern.Value, chore.RecurrenceInterval ?? 1);
                    
                    if (!chore.RecurrenceEndDate.HasValue || nextDueDate <= chore.RecurrenceEndDate.Value)
                    {
                        chore.DueDate = nextDueDate;
                        chore.IsCompleted = false;
                        chore.CompletedAt = null;
                    }
                }
            }
            else if (!chore.IsCompleted)
            {
                chore.CompletedAt = null;
            }
        }

        if (dto.IsRecurring.HasValue) chore.IsRecurring = dto.IsRecurring.Value;
        if (dto.RecurrencePattern != null && Enum.TryParse<RecurrencePattern>(dto.RecurrencePattern, true, out var pattern))
        {
            chore.RecurrencePattern = pattern;
        }
        if (dto.RecurrenceInterval.HasValue) chore.RecurrenceInterval = dto.RecurrenceInterval;
        if (dto.RecurrenceEndDate.HasValue) chore.RecurrenceEndDate = dto.RecurrenceEndDate;

        chore.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        if (dto.AssignedToId.HasValue && dto.AssignedToId != previousAssignedToId)
        {
            var notification = new Notification
            {
                UserId = dto.AssignedToId.Value,
                Title = "Chore Assigned",
                Message = $"You have been assigned to '{chore.Title}'",
                Type = NotificationType.ChoreAssigned,
                RelatedChoreId = chore.Id
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        return Ok(MapToDto(chore));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteChore(int choreListId, int id)
    {
        var chore = await _context.Chores
            .FirstOrDefaultAsync(c => c.Id == id && c.ChoreListId == choreListId);

        if (chore == null)
        {
            return NotFound();
        }

        _context.Chores.Remove(chore);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static DateTime CalculateNextDueDate(DateTime currentDueDate, RecurrencePattern pattern, int interval)
    {
        return pattern switch
        {
            RecurrencePattern.Daily => currentDueDate.AddDays(interval),
            RecurrencePattern.Weekly => currentDueDate.AddDays(7 * interval),
            RecurrencePattern.Monthly => currentDueDate.AddMonths(interval),
            RecurrencePattern.Yearly => currentDueDate.AddYears(interval),
            _ => currentDueDate
        };
    }

    private static ChoreDto MapToDto(Chore chore)
    {
        return new ChoreDto
        {
            Id = chore.Id,
            Title = chore.Title,
            Description = chore.Description,
            ChoreListId = chore.ChoreListId,
            AssignedToId = chore.AssignedToId,
            AssignedToUsername = chore.AssignedTo?.Username,
            DueDate = chore.DueDate,
            IsCompleted = chore.IsCompleted,
            CompletedAt = chore.CompletedAt,
            CreatedAt = chore.CreatedAt,
            UpdatedAt = chore.UpdatedAt,
            IsRecurring = chore.IsRecurring,
            RecurrencePattern = chore.RecurrencePattern?.ToString(),
            RecurrenceInterval = chore.RecurrenceInterval,
            RecurrenceEndDate = chore.RecurrenceEndDate
        };
    }
}
