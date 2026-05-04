using HostMgd.ApplicationServices;
using HostMgd.EditorInput;
using Teigha.Runtime;
using Application = HostMgd.ApplicationServices.Application;

namespace NanoCAD.API.Commands
{
    /// <summary>
    /// Тестовые команды
    /// </summary>
    public class TestCommands
    {
        // Проверка вывода многострочного текста в консоль nanoCAD
        [CommandMethod("ТЕСТГОСТ", CommandFlags.Modal)]
        [CommandMethod("TESTGOST", CommandFlags.Modal)]
        public void HelloCommand()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;

            doc.Editor.WriteMessage("\n  NanoCAD.API успешно загружен!");
            doc.Editor.WriteMessage("\n  Версия: 0.1.0");
            doc.Editor.WriteMessage("\n  Платформа: .NET 6.0");
        }

        // Проверка вывода координат точки, указанной пользователем, в консоль nanoCAD
        [CommandMethod("ТЕСТТОЧКА", CommandFlags.Modal)]
        [CommandMethod("TESTPOINT", CommandFlags.Modal)]
        public void PointCommand()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;

            var ed = doc.Editor;

            var pointResult = ed.GetPoint("\nУкажите любую точку на чертеже: ");
            if (pointResult.Status == PromptStatus.OK)
            {
                var pt = pointResult.Value;
                ed.WriteMessage($"\nВы указали точку: X={pt.X:F3}, Y={pt.Y:F3}, Z={pt.Z:F3}");
            }
        }
    }
}