using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoSquisher.Services;
using PhotoSquisher.Tools;


namespace PhotoSquisher.UI
{
    public static class SettingsUI
    {

        public static void Run()
        {
            Console.Clear();
            Console.WriteLine("     Settings    ");
            SettingsMainMenu();
            Console.WriteLine("Returning to Main Menu");
        }
        public static void SettingsMainMenu()
        {
            new NumberedMenu([
                new("Select photo library location", UpdateSettingUI, new photoLibraryPath() ),
                new("Select directory to output compressed photos", UpdateSettingUI, new outputPath() ),
                new("Rebuild Database", RebuildDatabaseUI),
                ]).Flow();
        }
        static void RebuildDatabaseUI()
        {
            bool userAccepted = false;
            Console.WriteLine("This will completely delete the existing database, and reset settings to default");
            MenuItem mi = new("Continue?", () => userAccepted = true);
            new YnMenu(mi).Flow();
            if (userAccepted) { maintainDb.RebuildDatabase(); }
        }
        public static void UpdateSettingUI(object arg) 
            //replacing multiple boilerplate update setting methods
            //takes any derived Setting class as arg
            //arg is object because ManuItem only takes an Action or Action<Object>.
            //upcast arg back to base Setting class in method body
        {
                Setting s = (Setting) arg; 
                string currentVal = s.Value;
                string newVal;
                do newVal = Console.ReadLine().Trim(); //Remove lead/trailing whitespace
                while (string.IsNullOrWhiteSpace(newVal));
                MenuItem mi = new($"Are you sure you want to change {currentVal} to {newVal}?", () => s.Value = newVal);
                new YnMenu(mi).Flow();
                Console.WriteLine(s.Value);            
        }

    }
}
