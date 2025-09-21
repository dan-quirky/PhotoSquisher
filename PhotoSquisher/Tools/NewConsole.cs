using System;
using System.Runtime.InteropServices;

//NB don't use this, not Xplatform
//Windows console API https://learn.microsoft.com/en-us/windows/console/console-functions
//https://sourcebae.com/blog/problem-using-allocconsole-and-marshalling/#:~:text=AllocConsole%20%28%29%20generates%20a%20new%20console%20for%20graphical,synchronization%20and%20proper%20declaration%20via%20P%2FInvoke%20is%20essential.
namespace AllocConsoleExample
{
//    class NewConsoleExample
//    {
//        [DllImport("kernel32.dll", SetLastError = true)]
//        static extern bool AllocConsole();

//        [DllImport("kernel32.dll", SetLastError = true)]
//        static extern bool FreeConsole();

//        static void Main(string[] args)
//        {
//            if (AllocConsole())
//            {
//                Console.WriteLine("Console attached successfully!");
//                Console.WriteLine("Press any key to close this console.");
//                Console.ReadKey();

//                FreeConsole();
//            }
//            else
//            {
//                int errorCode = Marshal.GetLastWin32Error();
//                System.Windows.Forms.MessageBox.Show(
//$"AllocConsole failed. Error: { errorCode}"
//                ); } 
        //} 
    //} 
} 