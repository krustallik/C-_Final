using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListenerDatabase.Models;

public class Screenshot
{
    public int ScreenshotId { get; set; } // Унікальний ID скріншота
    public byte[] ImageData { get; set; } // Масив байтів для зберігання зображення
    public DateTime Timestamp { get; set; } // Час створення скріншота

    // Зв'язок з користувачем
    public int UserId { get; set; }
    public User User { get; set; }
}