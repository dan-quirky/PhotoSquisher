//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

using System;
using ImageMagick;
using PhotoSquisher.Services;
using PhotoSquisher.Tools;

namespace PhotoSquisher.UI
{
    /*TODO
     * This is wildly bodged, clean it up
     * implement statuses - Scanning, Compressing, Idle, Broken
     * add basic user guide
     * 
     * 
    */

    internal class Info
    {
        public static void Run() { Console.Clear();  RefreshInfo(); }
        public static void RefreshInfo()
        {
            try
            {
                Console.Write(
    @"It's called Photosquisher because it squishes photos.

Scan Status:
");
                (int, int) scan_pct_cur = (Console.CursorLeft, Console.CursorTop);
                Console.Write(new string(' ', 4));
                Console.Write($" complete - ");
                (int, int) scan_items_cursor = (Console.CursorLeft, Console.CursorTop);
                Console.Write(new string(' ', 7));
                Console.Write(" items remaining");

                Console.WriteLine();
                Console.WriteLine("Compress Queue Status:");
                (int, int) comp_pct_cur = (Console.CursorLeft, Console.CursorTop);
                Console.Write(new string('.', 4));
                Console.Write($" complete - ");
                (int, int) comp_items_cur = (Console.CursorLeft, Console.CursorTop);
                Console.Write(new string(' ', 7));
                Console.Write(" items remaining");
                Console.Write(" Average Compression Ratio is ");
                (int, int) comp_ratio_cur = (Console.CursorLeft, Console.CursorTop);
                Console.Write(new string('.', 11));
                Console.WriteLine();
                Console.WriteLine("Press any key to go back...");
                Console.WriteLine();
                int endLine = Console.CursorTop;
                while (!Console.KeyAvailable)
                {
                    FileScanner? ActiveScan = FileScanner.Instance;
                    if (ActiveScan != null)  
                    {
                        double progress = (double)(ActiveScan.QueueCountInitial - ActiveScan.QueueCount) / (double)ActiveScan.QueueCountInitial;
                        WriteAtCursor(scan_pct_cur, 3, progress.ToString("P0"));
                        WriteAtCursor(scan_items_cursor, 7, ActiveScan.QueueCount.ToString());

                    }
                    if (PhotoProcessor.Instance != null)
                    {
                        double progress = 1 - (double)(PhotoProcessor.Instance.QueueCount) / (double)PhotoProcessor.Instance.QueueCountInitial;
                        string progress_str =  double.IsNaN(progress) ? "0%" : progress.ToString("P0"); 
                        string compressionRatio_str = double.IsNaN(PhotoProcessor.Instance.CompressionRatio) ? "Unavailable" : (PhotoProcessor.Instance.CompressionRatio).ToString("P0");
                        WriteAtCursor(comp_pct_cur, 3, progress_str);
                        WriteAtCursor(comp_items_cur, 7, (PhotoProcessor.Instance.QueueCount).ToString());
                        WriteAtCursor(comp_ratio_cur, 7, compressionRatio_str);

                    }
                    Console.SetCursorPosition(0, endLine);
                    Thread.Sleep(250);
                }

            }
            catch (Exception ex) { PsDebug.writeOutCaughtException(ex); }
        }
        internal static void WriteAtCursor((int left, int top) cursor, int numChars, string msg)
        {
            (int left, int top) = cursor;
            Console.SetCursorPosition(left, top);
            Console.Write(new string(' ', numChars)); // clear line
            Console.SetCursorPosition(left, top);
            Console.Write(msg);
        }
        
    }
}
