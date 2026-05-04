using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using NanoCAD.API.Helpers;

namespace NanoCAD.UI.Services
{
    // Сервис для получения превью блоков
    public class BlockPreviewService : IDisposable
    {
        // Путь к файлу blocks.dwg
        private readonly string _blocksFilePath;

        // Кэш изображений превью
        private readonly Dictionary<string, Bitmap> _previewCache;

        // Размер превью по умолчанию
        private readonly Size _previewSize;

        public BlockPreviewService(Size previewSize)
        {
            _blocksFilePath = PathHelper.GetBlocksFilePath();
            _previewCache = new Dictionary<string, Bitmap>();
            _previewSize = previewSize;
        }

        #region Публичные методы

        // Получить превью блока по его коду (PNSH, PVSH, ...)
        public Bitmap? GetBlockPreview(string blockCode)
        {
            // Проверяем кэш
            if (_previewCache.TryGetValue(blockCode, out Bitmap? cachedPreview))
            {
                return cachedPreview;
            }

            // Пытаемся загрузить из PNG-файла
            Bitmap? preview = LoadFromFile(blockCode);

            // Если файла нет — рисуем заглушку
            if (preview == null)
            {
                preview = GenerateFallbackPreview(blockCode);
            }

            // Сохраняем в кэш
            if (preview != null)
            {
                _previewCache[blockCode] = preview;
            }

            return preview;
        }

        // Получить все доступные превью
        public Dictionary<string, Bitmap> GetAllPreviews()
        {
            var result = new Dictionary<string, Bitmap>();

            foreach (var blockCode in GetAvailableBlockCodes())
            {
                var preview = GetBlockPreview(blockCode);
                if (preview != null)
                {
                    result[blockCode] = preview;
                }
            }

            return result;
        }

        // Предзагрузить все превью в кэш (вызывать при старте приложения)
        public void PreloadAllPreviews()
        {
            foreach (var blockCode in GetAvailableBlockCodes())
            {
                GetBlockPreview(blockCode);
            }
        }

        // Проверить, есть ли превью для блока
        public bool HasPreview(string blockCode)
        {
            return _previewCache.ContainsKey(blockCode) ||
                   File.Exists(GetPreviewFilePath(blockCode));
        }

        // Очистить кэш
        public void ClearCache()
        {
            foreach (var bitmap in _previewCache.Values)
            {
                bitmap?.Dispose();
            }
            _previewCache.Clear();
        }

        #endregion

        #region Приватные методы

        // Загрузить превью из файла
        private Bitmap? LoadFromFile(string blockCode)
        {
            string filePath = GetPreviewFilePath(blockCode);

            if (!File.Exists(filePath))
                return null;

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var bitmap = new Bitmap(stream);
                    return ResizeBitmap(bitmap, _previewSize);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки превью для {blockCode}: {ex.Message}");
                return null;
            }
        }

        // Получить путь к файлу превью
        private string GetPreviewFilePath(string blockCode)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDir, "Resources", "Previews", $"{blockCode}.png");
        }

        // Сгенерировать заглушку превью
        private Bitmap GenerateFallbackPreview(string blockCode)
        {
            int width = _previewSize.Width;
            int height = _previewSize.Height;

            if (width <= 0 || height <= 0)
                return new Bitmap(1, 1);

            Bitmap bitmap = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // Сглаживание
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Белый фон
                g.Clear(Color.White);

                // Рамка
                using (Pen borderPen = new Pen(Color.FromArgb(200, 200, 200), 1))
                {
                    g.DrawRectangle(borderPen, 1, 1, width - 2, height - 2);
                }

                // Название блока
                string displayText = blockCode switch
                {
                    "PNSH" => "ПНЩ",
                    "PVSH" => "ПВЩ",
                    _ => blockCode
                };

                // Шрифт для названия
                using (Font textFont = new Font("Segoe UI", Math.Max(9, width / 10), FontStyle.Bold))
                {
                    SizeF textSize = g.MeasureString(displayText, textFont);
                    float textX = (width - textSize.Width) / 2;
                    float textY = (height - textSize.Height) / 2;

                    // Тень текста
                    g.DrawString(displayText, textFont, Brushes.LightGray, textX + 1, textY + 1);
                    // Основной текст
                    g.DrawString(displayText, textFont, Brushes.DarkBlue, textX, textY);
                }

                // Схематичное изображение блока
                using (Pen schematicPen = new Pen(Color.FromArgb(100, 150, 200), 1.5f))
                {
                    int margin = width / 4;
                    int schematicSize = width - (margin * 2);

                    if (blockCode == "PNSH")
                    {
                        // Прямоугольник со скруглёнными углами (прибор на щите)
                        g.DrawRectangle(schematicPen, margin, margin + 10, schematicSize, schematicSize - 20);
                    }
                    else if (blockCode == "PVSH")
                    {
                        // Окружность (прибор вне щита)
                        g.DrawEllipse(schematicPen, margin, margin + 10, schematicSize, schematicSize - 20);
                    }
                    else
                    {
                        // Ромб (показывающий/регистрирующий прибор)
                        PointF[] diamondPoints = new PointF[]
                        {
                            new PointF(width / 2f, margin + 10),
                            new PointF(width - margin, height / 2f),
                            new PointF(width / 2f, height - margin - 10),
                            new PointF(margin, height / 2f)
                        };
                        g.DrawPolygon(schematicPen, diamondPoints);
                    }
                }
            }

            return bitmap;
        }

        // Изменить размер изображения с сохранением пропорций
        private Bitmap ResizeBitmap(Bitmap source, Size targetSize)
        {
            if (source.Width == targetSize.Width && source.Height == targetSize.Height)
                return new Bitmap(source);

            // Вычисляем новые размеры с сохранением пропорций
            float scale = Math.Min(
                (float)targetSize.Width / source.Width,
                (float)targetSize.Height / source.Height
            );

            int newWidth = (int)(source.Width * scale);
            int newHeight = (int)(source.Height * scale);

            // Центрируем изображение
            int offsetX = (targetSize.Width - newWidth) / 2;
            int offsetY = (targetSize.Height - newHeight) / 2;

            Bitmap result = new Bitmap(targetSize.Width, targetSize.Height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.Clear(Color.White);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(source, offsetX, offsetY, newWidth, newHeight);
            }

            return result;
        }

        // Получить список доступных кодов блоков
        private string[] GetAvailableBlockCodes()
        {
            // В будущем можно сканировать blocks.dwg и получать список блоков
            return new[] { "PNSH", "PVSH" };
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            ClearCache();
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}