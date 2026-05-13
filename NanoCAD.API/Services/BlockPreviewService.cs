using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace NanoCAD.API.Services
{
    public class BlockPreviewService
    {
        private readonly string _previewsPath;

        public BlockPreviewService()
        {
            string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            _previewsPath = Path.Combine(assemblyDir, "Resources", "Previews");
        }

        // Загрузить превью в PictureBox по коду блока (PNSH, PVSH)
        public void LoadPreview(PictureBox pictureBox, string blockCode)
        {
            pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
            string filePath = Path.Combine(_previewsPath, $"{blockCode}_preview.png");

            if (File.Exists(filePath))
            {
                pictureBox.Image = Image.FromFile(filePath);
            }
            else
            {
                DrawTextPreview(pictureBox, "Нет превью");
            }
        }

        // Нарисовать текст в PictureBox
        private void DrawTextPreview(PictureBox pictureBox, string text)
        {
            Bitmap bmp = new Bitmap(pictureBox.Width, pictureBox.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.WhiteSmoke);
                using (Font font = new Font("Segoe UI", 9))
                {
                    SizeF textSize = g.MeasureString(text, font);
                    g.DrawString(text, font, Brushes.Gray,
                        (bmp.Width - textSize.Width) / 2,
                        (bmp.Height - textSize.Height) / 2);
                }
            }
            pictureBox.Image = bmp;
        }
    }
}