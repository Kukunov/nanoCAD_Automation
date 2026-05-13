using System.IO;
using System.Reflection;

namespace NanoCAD.API.Helpers
{
    /// <summary>
    /// Вспомогательный класс для определения пути к файлу блоков
    /// </summary>
    public static class PathHelper
    {
        private static string? _blocksFilePath;

        // Получить путь к файлу blocks.dwg (находится в папке с DLL)
        public static string GetBlocksFilePath()
        {
            if (_blocksFilePath == null)
            {
                string assemblyPath = Assembly.GetExecutingAssembly().Location;
                string assemblyDir = Path.GetDirectoryName(assemblyPath) ?? string.Empty;
                _blocksFilePath = Path.Combine(assemblyDir, "blocks.dwg");
            }
            return _blocksFilePath;
        }

        // Проверить существование файла blocks.dwg
        public static bool BlocksFileExists()
        {
            return File.Exists(GetBlocksFilePath());
        }
    }
}