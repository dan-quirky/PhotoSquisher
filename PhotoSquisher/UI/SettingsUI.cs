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

        //static (string, Action<string>) photoLibraryPathArgs = 
        //    (
        //    (Func<string>)(_=> return Settings.photoLibraryPath ), 
        //    (Action<string>)(newVal => Settings.photoLibraryPath = newVal)
        //    );
        //static (string, Action<string>) outputPathArgs = ((Func<string>)Settings.outputPath, (Action<string>)(newVal => Settings.outputPath = newVal));
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
                new("Select photo library location", UpdatePath, (string) "libraryPath"),
                //new("Select directory to output compressed photos", UpdatePath_defunct3, outputPathArgs),
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
        static void UpdateLibraryLocation_defunct()
        //Some boiler plate needed because menu is limited to Actions without args 
        //could/should update MenuItem class to use generic delegates / actions w/ arbitrary no. args but not important enought right now. 
        {
            Console.WriteLine(Settings.photoLibraryPath);
            UpdatePath_defunct(() => Settings.photoLibraryPath, (newValue) => Settings.photoLibraryPath = newValue);
        }
        static void UpdateOutputLocation_defunct() //Some boiler plate needed because menu is limited to Actions (no args or return types
        {
            Console.WriteLine(Settings.outputPath);
            UpdatePath_defunct(() => Settings.outputPath, (newValue) => Settings.outputPath = newValue);
        }
        static void UpdatePath_defunct(Func<string> getSetting, Action<string> setSetting) //This is a heinously overcomplicated way of avoiding boilerplate
        {
            bool userAccepted = false;
            string newPath = Console.ReadLine();
            MenuItem mi = new($"Are you sure you want to change {getSetting()} to {newPath}?", () => userAccepted = true);
            new YnMenu(mi).Flow();
            if (userAccepted) setSetting(newPath);
        }
        static void UpdatePath_defunct2(object setting) //replacing multiple boilerplate update setting methods
        {
            string newPath;
            do newPath = Console.ReadLine().Trim(); //Remove lead/trailing whitespace
            while (string.IsNullOrWhiteSpace(newPath));
            MenuItem mi = new($"Are you sure you want to change {setting} to {newPath}?", () => setting = newPath);
            new YnMenu(mi).Flow();
            Console.WriteLine("Updated to " + setting);
            Console.WriteLine(Settings.outputPath);

        }
        static void UpdatePath_defunct3(object args) //replacing multiple boilerplate update setting methods
        {
            Console.WriteLine("checkpoint0");
            try
            {
                (Func<string> currentVal, Action<string> set) = ((Func<string>, Action<string>))args; //cast object args to tuple
                string newVal;
                do newVal = Console.ReadLine().Trim(); //Remove lead/trailing whitespace
                while (string.IsNullOrWhiteSpace(newVal));
                MenuItem mi = new($"Are you sure you want to change {currentVal} to {newVal}?", () => set(newVal));
                new YnMenu(mi).Flow();
                Console.WriteLine(Settings.outputPath); //this is a race condition? - db commit usually hasn't completed before the next line is hit so it looks like the setting wasn't updated properly

            }
            catch (Exception ex)
            {
                PsDebug.printCaughtException(ex);
            }
        }
        static void UpdatePath(object arg) //replacing multiple boilerplate update setting methods
        {
            Console.WriteLine("checkpoint0");
            try
            {
                string setting = (string) arg;
                string currentVal = Settings.getSetting(setting);
                string newVal;
                do newVal = Console.ReadLine().Trim(); //Remove lead/trailing whitespace
                while (string.IsNullOrWhiteSpace(newVal));
                MenuItem mi = new($"Are you sure you want to change {currentVal} to {newVal}?", () => Settings.setSetting(setting, newVal));
                new YnMenu(mi).Flow();
                Console.WriteLine(Settings.getSetting(setting));
            }
            catch (Exception ex)
            {
                PsDebug.printCaughtException(ex);
            }

        }

    }
}
