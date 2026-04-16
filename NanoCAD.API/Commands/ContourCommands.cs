using HostMgd.ApplicationServices;
using HostMgd.EditorInput;
using Teigha.Runtime;
using NanoCAD.API.Services;
using Application = HostMgd.ApplicationServices.Application;

namespace NanoCAD.API.Commands
{
    // Команды для управления контурами автоматизации
    public class ContourCommands
    {
        // Показать текущий контур
        [CommandMethod("ИНФОКОН", CommandFlags.Modal)]
        [CommandMethod("INFLOOP", CommandFlags.Modal)]
        public void ShowContourInfo()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            var ed = doc.Editor;

            var service = new ContourService(doc.Database);
            ed.WriteMessage($"\n=== Информация о контурах ===");
            ed.WriteMessage($"\n{service.GetStatusInfo()}");

            var allContours = service.GetAllContours();
            if (allContours.Count > 0)
            {
                ed.WriteMessage($"\n\nВсе контуры:");
                foreach (var kvp in allContours)
                {
                    ed.WriteMessage($"\n  Контур {kvp.Key}: {kvp.Value} элементов");
                }
            }
        }

        // Сменить текущий контур
        [CommandMethod("УСТКОН", CommandFlags.Modal)]
        [CommandMethod("CONTOUR_SET", CommandFlags.Modal)]
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