using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChoreQuest.API.Data;
using ChoreQuest.API.DTOs;
using ChoreQuest.API.Models;

namespace ChoreQuest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChoreListsController : ControllerBase
{
    private readonly ChoreQuestDbContext _context;

    public ChoreListsController(ChoreQuestDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChoreListDto>>> GetChoreLists([FromQuery] int userId)
    {
        var ownedLists = await _context.ChoreLists
            .Include(cl => cl.Owner)
            .Include(cl => cl.Chores)
            .Include(cl => cl.Shares).ThenInclude(s => s.SharedWithUser)
            .Where(cl => cl.OwnerId == userId)
            .ToListAsync();

        var sharedLists = await _context.ChoreListShares
            .Include(cls => cls.ChoreList).ThenInclude(cl => cl.Owner)
            .Include(cls => cls.ChoreList).ThenInclude(cl => cl.Chores)
            .Include(cls => cls.ChoreList).ThenInclude(cl => cl.Shares).ThenInclude(s => s.SharedWithUser)
            .Where(cls => cls.SharedWithUserId == userId)
            .Select(cls => cls.ChoreList)
            .ToListAsync();

        var allLists = ownedLists.Union(sharedLists).Distinct();

        return Ok(allLists.Select(MapToDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ChoreListDto>> GetChoreList(int id)
    {
        var choreList = await _context.ChoreLists
            .Include(cl => cl.Owner)
            .Include(cl => cl.Chores)
            .Include(cl => cl.Shares).ThenInclude(s => s.SharedWithUser)
            .FirstOrDefaultAsync(cl => cl.Id == id);

        if (choreList == null)
        {
            return NotFound();
        }

        return Ok(MapToDto(choreList));
    }

    [HttpPost]
    public async Task<ActionResult<ChoreListDto>> CreateChoreList([FromQuery] int userId, CreateChoreListDto dto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return BadRequest("User not found");
        }

        var choreList = new ChoreList
        {
            Name = dto.Name,
            Description = dto.Description,
            OwnerId = userId
        };

        _context.ChoreLists.Add(choreList);
        await _context.SaveChangesAsync();

        choreList = await _context.ChoreLists
            .Include(cl => cl.Owner)
            .Include(cl => cl.Chores)
            .Include(cl => cl.Shares)
            .FirstAsync(cl => cl.Id == choreList.Id);

        return CreatedAtAction(nameof(GetChoreList), new { id = choreList.Id }, MapToDto(choreList));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ChoreListDto>> UpdateChoreList(int id, UpdateChoreListDto dto)
    {
        var choreList = await _context.ChoreLists
            .Include(cl => cl.Owner)
            .Include(cl => cl.Chores)
            .Include(cl => cl.Shares).ThenInclude(s => s.SharedWithUser)
            .FirstOrDefaultAsync(cl => cl.Id == id);

        if (choreList == null)
        {
            return NotFound();
        }

        if (dto.Name != null) choreList.Name = dto.Name;
        if (dto.Description != null) choreList.Description = dto.Description;
        choreList.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(MapToDto(choreList));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteChoreList(int id)
    {
        var choreList = await _context.ChoreLists.FindAsync(id);

        if (choreList == null)
        {
            return NotFound();
        }

        _context.ChoreLists.Remove(choreList);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/share")]
    public async Task<ActionResult<ShareDto>> ShareChoreList(int id, ShareChoreListDto dto)
    {
        var choreList = await _context.ChoreLists.FindAsync(id);
        if (choreList == null)
        {
            return NotFound("Chore list not found");
        }

        var user = await _context.Users.FindAsync(dto.SharedWithUserId);
        if (user == null)
        {
            return BadRequest("User not found");
        }

        var existingShare = await _context.ChoreListShares
            .FirstOrDefaultAsync(cls => cls.ChoreListId == id && cls.SharedWithUserId == dto.SharedWithUserId);

        if (existingShare != null)
        {
            return BadRequest("List already shared with this user");
        }

        SharePermission permission;
        if (!Enum.TryParse<SharePermission>(dto.Permission, true, out permission))
        {
            permission = SharePermission.View;
        }

        var share = new ChoreListShare
        {
            ChoreListId = id,
            SharedWithUserId = dto.SharedWithUserId,
            Permission = permission
        };

        _context.ChoreListShares.Add(share);

        var notification = new Notification
        {
            UserId = dto.SharedWithUserId,
            Title = "Chore List Shared",
            Message = $"A chore list '{choreList.Name}' has been shared with you",
            Type = NotificationType.ListShared
        };
        _context.Notifications.Add(notification);

        await _context.SaveChangesAsync();

        share = await _context.ChoreListShares
            .Include(s => s.SharedWithUser)
            .FirstAsync(s => s.Id == share.Id);

        return Ok(new ShareDto
        {
            Id = share.Id,
            SharedWithUserId = share.SharedWithUserId,
            SharedWithUsername = share.SharedWithUser.Username,
            Permission = share.Permission.ToString(),
            SharedAt = share.SharedAt
        });
    }

    [HttpDelete("{id}/share/{shareId}")]
    public async Task<IActionResult> RemoveShare(int id, int shareId)
    {
        var share = await _context.ChoreListShares
            .FirstOrDefaultAsync(cls => cls.Id == shareId && cls.ChoreListId == id);

        if (share == null)
        {
            return NotFound();
        }

        _context.ChoreListShares.Remove(share);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static ChoreListDto MapToDto(ChoreList choreList)
    {
        return new ChoreListDto
        {
            Id = choreList.Id,
            Name = choreList.Name,
            Description = choreList.Description,
            OwnerId = choreList.OwnerId,
            OwnerUsername = choreList.Owner?.Username ?? "",
            CreatedAt = choreList.CreatedAt,
            UpdatedAt = choreList.UpdatedAt,
            ChoreCount = choreList.Chores?.Count ?? 0,
            CompletedChoreCount = choreList.Chores?.Count(c => c.IsCompleted) ?? 0,
            Shares = choreList.Shares?.Select(s => new ShareDto
            {
                Id = s.Id,
                SharedWithUserId = s.SharedWithUserId,
                SharedWithUsername = s.SharedWithUser?.Username ?? "",
                Permission = s.Permission.ToString(),
                SharedAt = s.SharedAt
            }).ToList() ?? new List<ShareDto>()
        };
    }
}
