/*TODO
 * if started as service, don't run ui
 */
using PhotoSquisher.Services;
using PhotoSquisher.Tools;
using PhotoSquisher.UI;


bool exit = false;
Console.WriteLine("PhotoSquisher - Copyright 2025 No One In Particular"); Console.WriteLine(Environment.NewLine);
do
{

    Console.WriteLine("Main Menu:");
    new NumberedMenu([
        new("Info",Info.Run),
        new("Scan Photos",FileScannerUI.Run),
        new("Process Photos", compressphotostest),
        new("Settings",SettingsUI.Run),
        new("test", SettingsUI.UpdateSettingUI, new photoLibraryPath() ),
        new("Exit",() => exit = true),
    ]).Flow();

    static async void compressphotostest()
    {
        try
        {
            await Task.Run(() => PhotoProcessor.Instance.StartQueue(new CancellationToken() ));
            Console.WriteLine("Compressing photos, see Info for progress");
            Info.Run();
        }
        catch (Exception ex)
        {
            PsDebug.printCaughtException(ex);
        }
    }



} while (exit != true);



