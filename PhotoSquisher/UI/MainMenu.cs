using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoSquisher.Services;
using PhotoSquisher.Tools;

using PhotoSquisher.Services;
using PhotoSquisher.Tools;
using PhotoSquisher.UI;


namespace PhotoSquisher.UI
{
    internal static class MainMenu
    {

        internal static void Run() 
        {
            bool exit = false;


            Console.WriteLine("PhotoSquisher - Copyright 2025 No One In Particular"); 
            Console.WriteLine(Environment.NewLine);
        do
        {

            Console.WriteLine("Main Menu:");
            new NumberedMenu([
                new ("Info",Info.Run),
                new ("Scan Photos",FileScannerUI.Run),
                new ("Process Photos", CompressPhotos),
                new ("Settings",SettingsUI.Run),
                new ("Exception Test", () => throw new Exception() ),
                new ("Exit",() => exit = true),
            ]).Flow();

            static async void CompressPhotos()
            {
                try
                {
                    await Task.Run(() => PhotoProcessor.Instance.StartQueue(new CancellationToken()));
                    Console.WriteLine("Compressing photos, see Info for progress");
                    //Info.Run();
                }
                catch (Exception ex)
                {
                    PsLogger.writeOutCaughtException(ex);
                }
            }


            } while (exit != true) ;

        }


}
}


