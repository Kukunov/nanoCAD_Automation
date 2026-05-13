using HostMgd.ApplicationServices;
using HostMgd.EditorInput;
using NanoCAD.API;
using NanoCAD.API.Forms;
using NanoCAD.API.Services;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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

            MainForm.ShowOrActivate();
            ed.WriteMessage("\n[OK] Панель GOST Automation открыта.");
        }
    }
}