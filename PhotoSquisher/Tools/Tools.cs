using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoSquisher.UI;

namespace PhotoSquisher.Tools
{
    public class PsDebug
    {
        bool debugging = true;

        public static void printProperties(object obj) 
        { Console.WriteLine($"Inspecting {obj} :");
            foreach (var property in obj.GetType().GetProperties())
            {
                string name = property.Name;
                object? value = property.GetValue(obj);
                //Type type = property.GetValue(obj).GetType();
                string type = property.GetValue(obj) is null ? "null" : (property.GetValue(obj).GetType()).ToString();
                 Console.WriteLine($"    {name}: {value} ({type}) ");
            }
        }
        public static void printVariable(object? obj)
        {
            Console.WriteLine($"{nameof(obj)} = {obj} ({obj.GetType()})" );
        }

        public static void printCaughtException(Exception ex)
        {
            Console.WriteLine($"{Environment.NewLine}Something went wrong: {ex.GetType()} {ex.Message}");
            MenuItem mi = new(
                "See full exception detail?",
                () => { Console.WriteLine(ex.InnerException + Environment.NewLine + ex.StackTrace); Console.ReadLine(); }
                );
            new YnMenu(mi).Flow();
            Console.ReadLine();

        }
    }
    public class PsLogger
    {
        public static void writeOutCaughtException(Exception ex)
        {
            string errorLogPath = System.IO.Path.Join(AppContext.BaseDirectory, "logs", $"UnhandledException_{DateTime.Now.ToString("yyyy-MM-dd_HHmmss")}.log");
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(errorLogPath));
            using System.IO.StreamWriter file = new(errorLogPath, append: true);
            file.WriteLine($"{DateTime.Now}: {ex.GetType()} {ex.Message}");
            file.WriteLine(ex.InnerException);
            file.WriteLine(ex.StackTrace);
            file.WriteLine();
        }

        public static void LogLine(string logLine)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Global.logPath));
            using System.IO.StreamWriter file = new(Global.logPath, append: true);
            file.WriteLine(logLine);
        }
        public static void LogLine()
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Global.logPath));
            using System.IO.StreamWriter file = new(Global.logPath, append: true);
            file.WriteLine();
        }
    }
}
