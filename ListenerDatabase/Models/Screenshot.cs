using System;

namespace ListenerDatabase.Models;

public class Screenshot
{
    public int ScreenshotId { get; set; } // Унікальний ID скріншота
    public string FilePath { get; set; } // Шлях до збереженого файлу
    public DateTime Timestamp { get; set; } // Час створення скріншота

    // Зв'язок з користувачем
    public int UserId { get; set; }
    public User User { get; set; }
}
