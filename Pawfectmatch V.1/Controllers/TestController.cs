using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pawfectmatch_V._1.Data;
using Pawfectmatch_V._1.Models;

namespace Pawfectmatch_V._1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "Backend is working!", timestamp = DateTime.Now });
        }

        [HttpGet("pets")]
        public async Task<IActionResult> GetPets()
        {
            try
            {
                var pets = await _context.Pets.Take(5).ToListAsync();
                return Ok(new { 
                    message = "Database connection successful!", 
                    petCount = pets.Count,
                    pets = pets 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { 
                status = "healthy", 
                timestamp = DateTime.Now,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
            });
        }
    }
} 