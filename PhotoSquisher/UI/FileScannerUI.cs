using PhotoSquisher.Services;
using PhotoSquisher.Tools;
namespace PhotoSquisher.UI
{
    internal class FileScannerUI //: Menu could inherit from Menu, but i don't see the point here
    {
        public static void Run()
        {
            new NumberedMenu(new[]
            {
                new MenuItem("Scan for new photos, instance class",ScanPhotosUI),
                new MenuItem("Scan for new photos, instance class in background",ScanPhotosBackground),
                new MenuItem("Re-index all photos",numberedMenu_Dictionary.PlaceholderAction),
                new MenuItem("A secret third option", numberedMenu_Dictionary.FireworksPlaceholderAction),
                new MenuItem("Go back", Console.Clear )
            }).Flow();
        }

        public static void ScanPhotosUI_staticDefunct()//menu class can't handle methods with return types for now, although maybe that's a good thing?
        {
            Console.WriteLine($"{FileScan_static.Scan().Result} database operations completed.");

        }
        public static void ScanPhotosBackground()//menu class can't handle methods with return types for now, although maybe that's a good thing?
        {
            _ = Task.Run(async() => { new FileScanner(); }); //TODO mute console writes, output logs
            Console.WriteLine("Scanning for photos in the background, see Info for scan progress");
            Thread.Sleep(500);
            //FIX Jumping straight to info hits a ObjectDisposedException at least sometimes. No clue why.
            //Info.Run();
        }

        public static void ScanPhotosUI()
        {
            //todo catch multiple instance exception
            try { new FileScanner(); }//Creates new scan queue and begins scanning
            catch (Exception ex) { PsDebug.printCaughtException(ex); }

        }



    }
}
