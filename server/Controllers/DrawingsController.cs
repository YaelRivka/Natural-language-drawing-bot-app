using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Models;
using server.Data;


[ApiController]
[Route("api/[controller]")]
public class DrawingsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DrawingsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> SaveDrawing([FromBody] Drawing data)
    {
        data.CreatedAt = DateTime.UtcNow;
        _context.Drawings.Add(data);
        await _context.SaveChangesAsync();

        return Ok(new { id = data.Id });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDrawing(int id)
    {
        var drawing = await _context.Drawings
            .FirstOrDefaultAsync(d => d.Id == id);

        if (drawing == null)
            return NotFound();

        return Ok(drawing);
    }


    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetDrawingsByUser(int userId)
    {
        var drawings = await _context.Drawings
      .Where(d => d.UserId == userId)
      .Select(d => new DrawingSummaryDto
      {
          Id = d.Id,
      })
      .ToListAsync();

        return Ok(drawings);
    }
}
