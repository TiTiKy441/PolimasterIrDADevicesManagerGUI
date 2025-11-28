namespace PolimasterIrDADevicesManagerGUI.Utils
{
    internal class Logger
    {

        public const string DefaultModuleName = "System";

        public static void Log(string message, string module = DefaultModuleName, char logSymbol = '*', ConsoleColor textColor = ConsoleColor.DarkGray)
        {
            Console.ForegroundColor = textColor;
            Console.WriteLine("[{0}][{1}][{2}] {3}", DateTime.Now.ToString(), module, logSymbol, message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Important(string message, string moduleName = DefaultModuleName)
        {
            Log(message, moduleName, '*', ConsoleColor.White);
        }

        public static void Error(string message, string moduleName = DefaultModuleName)
        {
            Log(message, moduleName, '!', ConsoleColor.Red);
        }

        public static void Warn(string message, string moduleName = DefaultModuleName)
        {
            Log(message, moduleName, '!', ConsoleColor.Yellow);
        }

        public static void Enable(string message, string moduleName = DefaultModuleName)
        {
            Log(message, moduleName, '*', ConsoleColor.Green);
        }
    }
}
