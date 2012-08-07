using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PicFX
{
    class ConX
    {
        private static DateTime startTime = DateTime.Now;

        private static void TagWrite(string tag, string message, ConsoleColor foreColor)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = foreColor;
            DateTime currentTime = DateTime.Now;
            Console.WriteLine("[{0}:{2:0.00}]{1}", tag, message, (currentTime - startTime).TotalSeconds);
            Console.ForegroundColor = oldColor;
        }

        public static void ErrorWrite(string errorMsg)
        {
            TagWrite("Error", errorMsg, ConsoleColor.Red);
        }

        public static void InfoWrite(string infoMsg)
        {
            TagWrite("Info", infoMsg, ConsoleColor.Cyan);
        }

        public static void FileWrite(string fileStr)
        {
            if (fileStr.StartsWith("("))
                TagWrite("File", fileStr, ConsoleColor.DarkCyan);
            else
                TagWrite("File", fileStr, ConsoleColor.DarkYellow);
        }
    }
}
