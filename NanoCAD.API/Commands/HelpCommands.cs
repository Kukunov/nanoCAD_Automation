using HostMgd.ApplicationServices;
using HostMgd.EditorInput;
using Teigha.Runtime;
using NanoCAD.API.Data;
using Application = HostMgd.ApplicationServices.Application;

namespace NanoCAD.API.Commands
{
    /// <summary>
    /// Команды вызова справки
    /// </summary>
    public class HelpCommands
    {
        // Вывод текста справки по надстройке
        [CommandMethod("ГОСТПОМОЩЬ", CommandFlags.Modal)]
        [CommandMethod("GOSTHELP", CommandFlags.Modal)]
        public void ShowHelp()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            var ed = doc.Editor;

            // Выводим справку одной строкой с разделителями
            ed.WriteMessage("\n" + string.Join("\n", HelpText.Content));
        }
    }
}