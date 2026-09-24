using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AndroidWebAPI.Data;
using AndroidWebAPI.Models;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly AppDbContext _context;
    public NotificationsController(AppDbContext context) => _context = context;

    // GET api/Notifications/parent/{parentId}
    // All notifications for a parent, newest first
    [HttpGet("parent/{parentId}")]
    public async Task<IActionResult> GetByParent(Guid parentId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.ParentID == parentId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return Ok(notifications);
    }

    // GET api/Notifications/unread-count/{parentId}
    [HttpGet("unread-count/{parentId}")]
    public async Task<IActionResult> GetUnreadCount(Guid parentId)
    {
        int count = await _context.Notifications
            .CountAsync(n => n.ParentID == parentId && !n.IsRead);
        return Ok(new { count });
    }

    // GET api/Notifications/user/{userId}
    // Staff/doctor-facing equivalent of GetByParent — used by the Doctor
    // dashboard's notification bell. Requires the Notifications.UserID
    // column added in Migration_DoctorModule.sql.
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(Guid userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserID == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return Ok(notifications);
    }

    // PATCH api/Notifications/mark-all-read/user/{userId}
    // Staff/doctor-facing equivalent of mark-all-read/{parentId}.
    [HttpPatch("mark-all-read/user/{userId}")]
    public async Task<IActionResult> MarkAllReadForUser(Guid userId)
    {
        var unread = await _context.Notifications
            .Where(n => n.UserID == userId && !n.IsRead)
            .ToListAsync();
        unread.ForEach(n => n.IsRead = true);
        await _context.SaveChangesAsync();
        return Ok(new { marked = unread.Count });
    }

    // PATCH api/Notifications/mark-read/{notificationId}
    [HttpPatch("mark-read/{notificationId}")]
    public async Task<IActionResult> MarkRead(Guid notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification == null) return NotFound();
        notification.IsRead = true;
        await _context.SaveChangesAsync();
        return Ok();
    }

    // PATCH api/Notifications/mark-all-read/{parentId}
    [HttpPatch("mark-all-read/{parentId}")]
    public async Task<IActionResult> MarkAllRead(Guid parentId)
    {
        var unread = await _context.Notifications
            .Where(n => n.ParentID == parentId && !n.IsRead)
            .ToListAsync();
        unread.ForEach(n => n.IsRead = true);
        await _context.SaveChangesAsync();
        return Ok(new { marked = unread.Count });
    }
}

