using Microsoft.AspNetCore.Mvc;
using DrawingServer.Models;

namespace DrawingServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DrawingsController : ControllerBase
    {
        private readonly ILogger<DrawingsController> _logger;

        public DrawingsController(ILogger<DrawingsController> logger)
        {
            _logger = logger;
        }

        // רשימה זמנית בזיכרון (לשלב ראשון)
        private static readonly List<DrawingData> _drawings = new();

        [HttpPost]
        public IActionResult SaveDrawing([FromBody] DrawingData data)
        {
            data.Id = _drawings.Count + 1;
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
}
