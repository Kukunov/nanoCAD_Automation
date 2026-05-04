using System;
using System.Windows.Forms;
using NanoCAD.UI.Forms;

namespace NanoCAD.UI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // «апускаем форму как главное окно приложени€
            Application.Run(new MainForm());
        }
    }
}