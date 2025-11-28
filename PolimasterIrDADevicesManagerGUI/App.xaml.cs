using PolimasterIrDADevicesManagerGUI.Device.Protocols;
using PolimasterIrDADevicesManagerGUI.GUI;
using PolimasterIrDADevicesManagerGUI.IrDA;
using PolimasterIrDADevicesManagerGUI.Utils;
using System.Runtime.InteropServices;
using System.Windows;

namespace PolimasterIrDADevicesManagerGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32", SetLastError = true)]
        public static extern void FreeConsole();

        public App()
        {
            //AllocConsole();
            Logger.Important("Process launched!");
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            IrDAPortManager.StartScan();
        }

        private void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            Logger.Important("Process exiting...");
            IrDAPortManager.Stop();
            //FreeConsole();
        }
    }
}
