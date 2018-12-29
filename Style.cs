using System;

namespace version_control_system
{
    static class Style
    {
        public static void FileNotChangedColor()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
        }

        public static void FileChangedColor()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
        }
        public static void Default()
        {
            Console.ResetColor();
        }
    }
}