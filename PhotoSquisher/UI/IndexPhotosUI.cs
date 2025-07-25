using System.Diagnostics;
using PhotoSquisher.Services;

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
                { "CRUD Test", IndexPhotos.CRUDtest },
                { "Rebuild Database", IndexPhotos.RebuildDatabase},
            });
        }

        public static void DeleteDatabaseUI()
        {
            //todo This should be a y/n menu class
            Console.WriteLine("Delete everything? I wouldn't if i were you...       y / n");
            char userInput = Console.ReadKey().KeyChar;Console.WriteLine(Environment.NewLine);
            if (userInput == 'y')
            {
                IndexPhotos.DeleteDatabaseAsync();
            }
            else { Console.WriteLine("invalid"); }
        }
        public static void ScanPhotosUI()//menu can't handle methods with return types for now, maybe that's a good thing though
        {
            Console.WriteLine(IndexPhotos.ScanPhotos()); 
        }
    }
}
