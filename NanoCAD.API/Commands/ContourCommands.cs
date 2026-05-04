using System.IO;
using System.Text;
using System.Text.Json;
using HostMgd.ApplicationServices;
using HostMgd.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Runtime;
using NanoCAD.API.Services;
using Application = HostMgd.ApplicationServices.Application;

namespace NanoCAD.API.Commands
{
    /// <summary>
    /// Команды и методы для управления контурами автоматизации
    /// </summary>
    public class ContourCommands
    {
        // Показать сводку по контурам и позициям
        [CommandMethod("ИНФОКОН", CommandFlags.Modal)]
        [CommandMethod("INFOLOOP", CommandFlags.Modal)]
        public void ShowContourInfo()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            var ed = doc.Editor;

            var service = new ContourService(doc.Database);
            ed.WriteMessage($"\n=== Информация о контурах ===");
            ed.WriteMessage($"\nАктивный контур: {service.GetCurrentContour()}");
            ed.WriteMessage($"\nСледующая вставка: {service.GetNextPositionPreview()}");

            // Сводка по контурам (только непустые)
            var allContours = service.GetAllContours();
            var activeContours = allContours
                .Where(kvp => kvp.Value > 0)
                .OrderBy(kvp => kvp.Key)
                .ToList();

            if (activeContours.Count > 0)
            {
                int totalElements = activeContours.Sum(kvp => kvp.Value);

                ed.WriteMessage($"\n\nУчтённые контуры:");
                foreach (var kvp in activeContours)
                {
                    ed.WriteMessage($"\n  Контур {kvp.Key}: последняя вставка {kvp.Key}-{kvp.Value}");
                }
                ed.WriteMessage($"\n\nВсего учтённых элементов: {totalElements}");
            }
            else
            {
                ed.WriteMessage($"\n\nНет учтённых контуров.");
            }
        }

        // Сменить текущий контур
        [CommandMethod("УСТКОН", CommandFlags.Modal)]
        [CommandMethod("SETLOOP", CommandFlags.Modal)]
        public void SetCurrentContour()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            var ed = doc.Editor;

            var service = new ContourService(doc.Database);

            ed.WriteMessage($"\nТекущий контур: {service.GetCurrentContour()}");

            var result = ed.GetInteger("\nВведите номер нового контура: ");
            if (result.Status != PromptStatus.OK) return;

            int newContour = result.Value;
            if (newContour < 1)
            {
                ed.WriteMessage("\n[ОШИБКА] Номер контура должен быть больше 0");
                return;
            }

            service.SetCurrentContour(newContour);
            ed.WriteMessage($"\n[OK] Текущий контур изменён на {newContour}");
            ed.WriteMessage($"\n{service.GetStatusInfo()}");
        }

        // Сбросить счётчик текущего контура
        [CommandMethod("СБРОСКОН", CommandFlags.Modal)]
        [CommandMethod("RESETLOOP", CommandFlags.Modal)]
        public void ResetContour()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            var ed = doc.Editor;

            var service = new ContourService(doc.Database);
            int currentContour = service.GetCurrentContour();

            var keyOptions = new PromptKeywordOptions($"\nСбросить счётчик контура {currentContour}? [Да/Нет]: ");
            keyOptions.Keywords.Add("Да");
            keyOptions.Keywords.Add("Нет");
            keyOptions.AllowNone = true;

            var keyResult = ed.GetKeywords(keyOptions);
            if (keyResult.Status != PromptStatus.OK || keyResult.StringResult != "Да")
            {
                ed.WriteMessage("\n[ОТМЕНА] Сброс отменён");
                return;
            }

            service.ResetCurrentContour();
            ed.WriteMessage($"\n[OK] Счётчик контура {currentContour} сброшен");
        }
    }
}