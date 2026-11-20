using System;
using System.Windows.Forms;

namespace SimGame
{
    /// <summary>
    /// Лабораторна робота №2, частина 2. Варіант 28: гра Sim (гра з повною інформацією).
    /// Комп'ютерний опонент – мінімакс з альфа-бета відсіканнями, 3 рівні складності.
    /// </summary>
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
#if NET5_0_OR_GREATER
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
