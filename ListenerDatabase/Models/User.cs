using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListenerDatabase.Models;

public class User
{
    public int UserId { get; set; } // Унікальний ID користувача
    public string ComputerName { get; set; } // Назва комп'ютера

    // Зв'язки
    public ICollection<KeyLog> KeyLogs { get; set; }
    public ICollection<Screenshot> Screenshots { get; set; }
}