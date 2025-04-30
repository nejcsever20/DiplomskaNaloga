using Microsoft.AspNetCore.Mvc;
using diplomska.Data;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Diplomska_Naloga.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Constructor to inject the DbContext
        public TransportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // API method to toggle Sp status
        [HttpPost("ToggleSp")]
        public async Task<IActionResult> ToggleSp([FromBody] SpToggleDto dto)
        {
            var transport = await _context.Transport.FindAsync(dto.Id);
            if (transport == null)
                return NotFound();

            transport.Sp = dto.IsChecked;
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        // DTO for ToggleSp action
        public class SpToggleDto
        {
            public int Id { get; set; }
            public bool IsChecked { get; set; }
        }
    }
}
