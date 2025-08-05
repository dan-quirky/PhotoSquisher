using System.Diagnostics;
using PhotoSquisher.Services;
using PhotoSquisher.Tools;
using PhotoSquisher.UI;
using static PhotoSquisher.UI.NumberedMenu;

namespace PhotoSquisher.UI
{
    internal class IndexPhotosUI //: Menu could inherit from Menu, but i don't see the point here
    {
        public static void Run()
        {
            new NumberedMenu(new[]
            {
                new MenuItem ( "Scan for new photos",ScanPhotosUI),
                new MenuItem("Re-index all photos",numberedMenu_Dictionary.PlaceholderAction),
                new MenuItem(  "A secret third option", numberedMenu_Dictionary.FireworksPlaceholderAction),
                new MenuItem(  "CRUD Test", Misc.CRUDtest ),
                new MenuItem("Rebuild Database", maintainDb.RebuildDatabase),
            }).Flow();
        }

        public static void ScanPhotosUI()//menu class can't handle methods with return types for now, although maybe that's a good thing?
        {
            Console.WriteLine($"{IndexPhotos.ScanPhotos().Result} database operations completed.");
            
        }
    }
}
