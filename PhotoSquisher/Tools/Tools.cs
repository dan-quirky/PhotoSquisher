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

        //[Conditional("DEBUG")] //There's probably a cleaner way than decorating every debug metho
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
        //[Conditional("DEBUG")]
        public static void printVariable(object? obj)
        {
            Console.WriteLine($"{nameof(obj)} = {obj} ({obj.GetType()})" );
        }

        //[Conditional("DEBUG")]
        public static void printCaughtException(Exception ex)
        {
            Console.WriteLine($"{Environment.NewLine}Something went wrong: {ex.GetType()} {ex.Message}");
            new YnMenu(
                "See full exception detail?", 
                () => Console.WriteLine(ex.InnerException + Environment.NewLine + ex.StackTrace)
                ).Flow();
        }

    }
}
