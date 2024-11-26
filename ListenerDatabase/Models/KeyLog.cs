using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListenerDatabase.Models;

public class KeyLog
{
    public int KeyLogId { get; set; } // Унікальний ID запису
    public string KeyPressed { get; set; } // Натиснута клавіша
    public DateTime Timestamp { get; set; } // Час натискання клавіші

    // Зв'язок з користувачем
    public int UserId { get; set; }
    public User User { get; set; }
}