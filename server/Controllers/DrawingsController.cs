using Microsoft.AspNetCore.Mvc;
using Server.Models;

[ApiController]
[Route("api/[controller]")]
public class DrawingsController : ControllerBase
{
    private static readonly List<DrawingData> _drawings = new();

    [HttpPost]
    public IActionResult SaveDrawing([FromBody] DrawingData data)
    {
        data.Id = _drawings.Count + 1;
        data.CreatedAt = DateTime.UtcNow;
        _drawings.Add(data);
        return Ok(new { id = data.Id });
    }

    [HttpGet("{id}")]
    public IActionResult GetDrawing(int id)
    {
        var drawing = _drawings.FirstOrDefault(d => d.Id == id);
        if (drawing == null)
            return NotFound();

        return Ok(drawing);
    }
}
