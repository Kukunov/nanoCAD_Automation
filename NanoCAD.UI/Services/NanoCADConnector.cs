using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using NanoCAD.UI.Models;

namespace NanoCAD.UI.Services
{
    // Сервис для взаимодействия с nanoCAD
    public class NanoCADConnector
    {
        private readonly UIModel _model;

        // WinAPI для управления окнами
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        private const int SW_RESTORE = 9;
        private const int SW_SHOW = 5;

        public NanoCADConnector(UIModel model)
        {
            _model = model;
        }

        #region Вспомогательные методы

        // Получить процесс nanoCAD
        private Process? GetNanoCADProcess()
        {
            var processes = Process.GetProcessesByName("nanoCAD");

            // Диагностика
            System.Diagnostics.Debug.WriteLine($"[NanoCADConnector] Найдено процессов nanoCAD: {processes.Length}");

            if (processes.Length == 0)
            {
                // Попробуем альтернативные имена процесса
                string[] alternativeNames = { "nanoCAD", "nCad", "nc22", "nc23", "nc24", "nc25" };

                foreach (string name in alternativeNames)
                {
                    processes = Process.GetProcessesByName(name);
                    if (processes.Length > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"[NanoCADConnector] Найден процесс по имени: {name}");
                        break;
                    }
                }
            }

            return processes.FirstOrDefault();
        }

        // Проверить, запущен ли nanoCAD
        public bool IsNanoCADRunning()
        {
            return GetNanoCADProcess() != null;
        }

        // Активировать окно nanoCAD (развернуть и перевести фокус)
        private void ActivateNanoCADWindow()
        {
            var process = GetNanoCADProcess();
            if (process == null)
                throw new InvalidOperationException("nanoCAD не запущен.");

            IntPtr handle = process.MainWindowHandle;

            // Если окно свёрнуто — разворачиваем
            if (IsIconic(handle))
            {
                ShowWindow(handle, SW_RESTORE);
            }
            else
            {
                ShowWindow(handle, SW_SHOW);
            }

            // Переводим фокус
            SetForegroundWindow(handle);
        }

        // Отправить нажатия клавиш в активное окно
        private void SendKeysWithDelay(string keys, int delayMs = 300)
        {
            SendKeys.SendWait(keys);
            System.Threading.Thread.Sleep(delayMs);
        }

        #endregion

        #region Публичные методы

        // Обновить состояние из nanoCAD
        public async Task RefreshStateAsync()
        {
            await Task.Run(() =>
            {
                var process = GetNanoCADProcess();
                if (process == null)
                {
                    _model.CurrentDrawing = "nanoCAD не запущен";
                    _model.NotifyStateChanged();
                    return;
                }

                // TODO: В будущем заменить на реальный вызов API nanoCAD
                // для получения:
                // - Имени текущего чертежа
                // - Текущего контура из XData
                // - Счётчика элементов

                // Имитация данных
                _model.CurrentDrawing = process.MainWindowTitle ?? "Текущий чертёж";
                _model.NotifyStateChanged();

                // Эти значения будут обновляться через команды nanoCAD
                // Пока используем сохранённые в модели
            });
        }

        // Установить текущий контур в nanoCAD
        public async Task SetCurrentContourAsync(int contour)
        {
            await Task.Run(() =>
            {
                ActivateNanoCADWindow();

                // Отправляем команду смены контура
                SendKeysWithDelay("УСТКОН{ENTER}", 400);
                SendKeysWithDelay($"{contour}{{ENTER}}", 400);
            });
        }

        // Сбросить счётчик контура в nanoCAD
        public async Task ResetContourAsync(int contour)
        {
            await Task.Run(() =>
            {
                ActivateNanoCADWindow();

                // Отправляем команду сброса
                SendKeysWithDelay("СБРОСКОН{ENTER}", 400);

                // Подтверждаем сброс
                SendKeysWithDelay("Да{ENTER}", 400);
            });
        }

        // Вставить блок в nanoCAD
        public async Task InsertBlockAsync()
        {
            await Task.Run(() =>
            {
                ActivateNanoCADWindow();

                // Определяем команду в зависимости от типа блока
                string command = _model.SelectedBlockCode switch
                {
                    "PNSH" => "ПНЩ",
                    "PVSH" => "ПВЩ",
                    _ => "ПНЩ"
                };

                // Отправляем команду вставки
                SendKeysWithDelay($"{command}{{ENTER}}", 500);

                // Вводим ТИП
                SendKeysWithDelay($"{_model.TypeDesignation}{{ENTER}}", 400);

                // Вводим ПОЗ
                string position = $"{_model.CurrentContour}-{_model.ElementNumber}";
                SendKeysWithDelay($"{position}{{ENTER}}", 400);

                // После этого nanoCAD запросит точку вставки
                // Пользователь укажет её мышью или введёт координаты
            });
        }

        // Получить информацию о блоках на чертеже (для будущего использования)
        public async Task<string> GetBlockStatisticsAsync()
        {
            await Task.Run(() =>
            {
                ActivateNanoCADWindow();

                // Отправляем команду получения статистики
                SendKeysWithDelay("ИНФОКОН{ENTER}", 1000);
            });

            // TODO: Получить результат из nanoCAD
            return "Статистика получена";
        }

        // Экспортировать статистику контуров
        public async Task ExportContourStatisticsAsync(string format = "TXT")
        {
            await Task.Run(() =>
            {
                ActivateNanoCADWindow();

                // Отправляем команду экспорта
                SendKeysWithDelay("ЭКСПОРТКОН{ENTER}", 500);
                SendKeysWithDelay($"{format}{{ENTER}}", 500);
                SendKeysWithDelay("{ENTER}", 500); // Путь по умолчанию
            });
        }

        // Показать панель ГОСТ в nanoCAD (если была скрыта)
        public async Task ShowGostPanelAsync()
        {
            await Task.Run(() =>
            {
                ActivateNanoCADWindow();
                SendKeysWithDelay("ГОСТПАНЕЛЬ{ENTER}", 500);
            });
        }

        #endregion


    }
}