using System.Diagnostics;
using System.IO;
using System.Reflection;
using HostMgd.ApplicationServices;
using HostMgd.EditorInput;
using Teigha.Runtime;
using Application = HostMgd.ApplicationServices.Application;

namespace NanoCAD.API.Commands
{
    /// <summary>
    /// Команды вызова UI надстройки
    /// </summary>
    public class UiCommands
    {
        [CommandMethod("ГОСТОКНО", CommandFlags.Modal)]
        [CommandMethod("GOSTWINDOW", CommandFlags.Modal)]
        public void ShowGostPanel()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            var ed = doc.Editor;

            try
            {
                // Проверяем, есть ли уже запущенный процесс
                var existingProcesses = Process.GetProcessesByName("NanoCAD.UI");

                if (existingProcesses.Length > 0)
                {
                    // Проверяем, отвечает ли процесс
                    foreach (var proc in existingProcesses)
                    {
                        try
                        {
                            if (proc.Responding)
                            {
                                ed.WriteMessage("\n[OK] Панель GOST Automation уже запущена и отвечает.");
                                ActivateWindow(proc.MainWindowHandle);
                                return;
                            }
                            else
                            {
                                // Процесс завис — убиваем
                                ed.WriteMessage("\n[INFO] Найден зависший процесс панели. Перезапускаем...");
                                proc.Kill();
                                proc.WaitForExit(3000);
                            }
                        }
                        catch
                        {
                            // Процесс уже завершается
                        }
                    }
                }

                // Запускаем новый экземпляр
                string uiPath = FindUIExecutable();

                if (string.IsNullOrEmpty(uiPath) || !File.Exists(uiPath))
                {
                    ed.WriteMessage($"\n[ОШИБКА] Не найден файл NanoCAD.UI.exe");
                    ed.WriteMessage($"\nПапка поиска: {Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}");
                    return;
                }

                var startInfo = new ProcessStartInfo
                {
                    FileName = uiPath,
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Normal,
                    WorkingDirectory = Path.GetDirectoryName(uiPath)
                };

                Process.Start(startInfo);

                ed.WriteMessage($"\n[OK] Панель GOST Automation запущена.");
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage($"\n[ОШИБКА] {ex.Message}");
            }
        }

        private string FindUIExecutable()
        {
            string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

            // Ищем в той же папке, что и DLL
            string uiPath = Path.Combine(assemblyDir, "NanoCAD.UI.exe");
            if (File.Exists(uiPath))
                return uiPath;

            // Ищем в подпапках при разработке
            string[] relativePaths = new[]
            {
                "../NanoCAD.UI/NanoCAD.UI.exe",
                "../../NanoCAD.UI/bin/Debug/net6.0-windows/NanoCAD.UI.exe",
                "../../NanoCAD.UI/bin/Release/net6.0-windows/NanoCAD.UI.exe"
            };

            foreach (string relPath in relativePaths)
            {
                string fullPath = Path.GetFullPath(Path.Combine(assemblyDir, relPath));
                if (File.Exists(fullPath))
                    return fullPath;
            }

            return string.Empty;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_RESTORE = 9;

        private void ActivateWindow(IntPtr handle)
        {
            ShowWindow(handle, SW_RESTORE);
            SetForegroundWindow(handle);
        }
    }
}