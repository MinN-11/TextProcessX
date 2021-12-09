using System;

namespace TextProcess {
    public static class ErrorReport {
        public static string FileName = "";
        public static int LineNumber;

        public static void ReportError(string message) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"Error: ");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine($"{message}\t\tat {FileName}, Line {LineNumber}.");
            Environment.ExitCode = 1;
        }
        
        public static void ReportSevereWarning(string message) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"Warning: ");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine($"{message}\t\tat {FileName}, Line {LineNumber}.");
        }
        
        public static void ReportWarning(string message) {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"Warning: ");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine($"{message}\t\tat {FileName}, Line {LineNumber}.");
        }
        
        public static void ReportToDo(string message) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"TODO: ");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine($"{message}\t\tat {FileName}, Line {LineNumber}.");
        }
    }
}