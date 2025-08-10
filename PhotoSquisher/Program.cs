//TODO


using PhotoSquisher.Services;
using PhotoSquisher.UI;
using PhotoSquisher.Tools;


bool exit = false;
Console.WriteLine("PhotoSquisher - Copyright 2025 No One In Particular"); Console.WriteLine(Environment.NewLine);
do
{

    Console.WriteLine("Main Menu:");
    new NumberedMenu([
        new("Info",Info.GetInfo),
        new("Scan Photos",IndexPhotosUI.Run),
        new("Settings",SettingsUI.Run),
        new("Exit",() => exit = true),
        new("test",MenuTest.Run),
    ]).Flow();

/*old menu
    do
    {

        Console.WriteLine("Main Menu:");
        new numberedMenu_Dictionary(new Dictionary<string, Action>
    {
        {"Info",Info.GetInfo},
        {"Scan Photos",IndexPhotosUI.Run },
        {"Compress Photos", numberedMenu_Dictionary.PlaceholderAction},
        {"Menu Test",  MenuTest.Run},
        {"Compress Test", ProcessPhoto.readWriteImageTest },
         {"Queue Test", ProcessQueue.Run },
    });
*/


    } while (exit != true);



