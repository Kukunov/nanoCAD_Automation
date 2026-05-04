using System.Linq;
using HostMgd.ApplicationServices;
using HostMgd.EditorInput;
using Teigha.Runtime;
using NanoCAD.API.Services;
using Application = HostMgd.ApplicationServices.Application;
using Color = Teigha.Colors.Color;

namespace NanoCAD.API.Commands
{
    /// <summary>
    /// Команды генерации и управления цветами контуров автоматизации
    /// </summary>
    public class ContourColorCommands
    {
        private readonly ContourColorService _colorService = new();

        // Автоматическая окраска контуров
        [CommandMethod("ЦВЕТКОНТУРЫ", CommandFlags.Modal)]
        [CommandMethod("COLORCONTOURS", CommandFlags.Modal)]
        public void AutoColorContours() 
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            var ed = doc.Editor;

            // Запрос подтверждения
            var keyOptions = new PromptKeywordOptions(
                "\nАвтоматически назначить цвета контурам? [Да/Нет]: "
            );
            keyOptions.Keywords.Add("Да");
            keyOptions.Keywords.Add("Нет");
            keyOptions.Keywords.Default = "Да";     // Да по умолчанию
            keyOptions.AllowNone = true;

            var keyResult = ed.GetKeywords(keyOptions);
            if (keyResult.Status != PromptStatus.OK) return;

            if (keyResult.StringResult == "Нет")
            {
                ed.WriteMessage("\n[ОТМЕНА] Операция отменена.");
                return;
            }

            // Применяем цвета
            var result = _colorService.ApplyAutoColors(doc.Database);

            if (result.Success)
            {
                ed.WriteMessage($"\n[OK] {result.Message}");
                ed.WriteMessage("\n\nЦвета по контурам:");

                foreach (var kvp in result.ColoredContours.OrderBy(k => k.Key))
                {
                    ed.WriteMessage($"\n  Контур {kvp.Key}: R={kvp.Value.Red}, G={kvp.Value.Green}, B={kvp.Value.Blue}");
                }

                ed.WriteMessage($"\n\nСлой '{ContourColorService.ColorLayerName}' создан.");
                ed.WriteMessage("\nДля отключения цветов: СБРОСЦВЕТ / CLEARCOLORS");
            }
            else
            {
                ed.WriteMessage($"\n[ОШИБКА] {result.Message}");
            }
        }

        // Очистка цветов контуров
        [CommandMethod("СБРОСЦВЕТ", CommandFlags.Modal)]
        [CommandMethod("CLEARCOLORS", CommandFlags.Modal)]
        public void ClearContourColors()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            var ed = doc.Editor;

            var keyOptions = new PromptKeywordOptions(
                "\nСбросить все цвета контуров? [Да/Нет]: "
            );
            keyOptions.Keywords.Add("Да");
            keyOptions.Keywords.Add("Нет");
            keyOptions.Keywords.Default = "Нет";    // Нет по умолчанию
            keyOptions.AllowNone = true;

            var keyResult = ed.GetKeywords(keyOptions);
            if (keyResult.Status != PromptStatus.OK || keyResult.StringResult != "Да") return;

            _colorService.ClearContourColors(doc.Database);
            ed.WriteMessage("\n[OK] Цвета контуров сброшены.");
        }
    }
}