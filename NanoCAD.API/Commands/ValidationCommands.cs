using HostMgd.ApplicationServices;
using HostMgd.EditorInput;
using Teigha.Runtime;
using NanoCAD.API.Services;
using Application = HostMgd.ApplicationServices.Application;

namespace NanoCAD.API.Commands
{
    /// <summary>
    /// Команды для проверки валидации ТИП, ПОЗ
    /// </summary>
    public class ValidationCommands
    {
        // Проверить строку на соответствие формату ТИП
        [CommandMethod("ПРОВТИП", CommandFlags.Modal)]
        [CommandMethod("CHECKTYPE", CommandFlags.Modal)]
        public void CheckTypeDesignation()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            var ed = doc.Editor;

            var result = ed.GetString("\nВведите обозначение типа для проверки (например, TE): ");
            if (result.Status != PromptStatus.OK) return;

            string input = result.StringResult;
            var validation = ValidationService.ValidateTypeDesignation(input);

            if (validation.IsValid)
            {
                ed.WriteMessage($"\n[OK] '{input}' - корректное обозначение типа.");
            }
            else
            {
                ed.WriteMessage($"\n[ОШИБКА] {validation.ErrorMessage}");
            }
        }

        // Проверить строку на соответствие формату ПОЗ
        [CommandMethod("ПРОВПОЗ", CommandFlags.Modal)]
        [CommandMethod("CHECKPOS", CommandFlags.Modal)]
        public void CheckPosition()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            var ed = doc.Editor;

            var result = ed.GetString("\nВведите позиционное обозначение для проверки (например, 1-1): ");
            if (result.Status != PromptStatus.OK) return;

            string input = result.StringResult;
            var validation = ValidationService.ValidatePosition(input);

            if (validation.IsValid)
            {
                int contour = ValidationService.ExtractContourNumber(input);
                int element = ValidationService.ExtractElementNumber(input);
                ed.WriteMessage($"\n[OK] '{input}' - корректное позиционное обозначение.");
                ed.WriteMessage($"\n     Контур: {contour}, Элемент: {element}");
            }
            else
            {
                ed.WriteMessage($"\n[ОШИБКА] {validation.ErrorMessage}");
            }
        }
    }
}