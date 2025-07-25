using System.Diagnostics;
using PhotoSquisher.Services;
using PhotoSquisher.Tools;

namespace PhotoSquisher.UI
{
    internal class IndexPhotosUI //: Menu could inherit from Menu, but i don't see the point here
    {
        public static void Run()
        {
            //Menu m = new Menu(new { "Scan for new photos", "Re-index all photos", "A secret third option" });
            Menu m = new Menu(new Dictionary<string, Action>
            {
                { "Scan for new photos",ScanPhotosUI},
                { "Re-index all photos",Menu.PlaceholderAction},
                { "A secret third option",Menu.FireworksPlaceholderAction},
                { "CRUD Test", Misc.CRUDtest },
                { "Rebuild Database", maintainDb.RebuildDatabase},
            });
        }

        public static void ScanPhotosUI()//menu class can't handle methods with return types for now, although maybe that's a good thing?
        {
            Console.WriteLine($"{IndexPhotos.ScanPhotos().Result} database operations completed."); 
        }
    }
}
