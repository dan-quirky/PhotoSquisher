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



/*
 * BUGS
 * 
 * Running scan and process simultaneously can cause
 *  InvalidOperationException: A second operation started on this context before a previous operation completed. Any instance members are not guaranteed to be thread safe.
 * Fix: ?? Easy option is block one from launching while the other is running BUT they should be be capable of running in parallel (and are capable of doing it for a while)
 *      Really need a way to block/queue multiple dbcontext instances
 *      Don't run them in parallel in UI (upsetting), and when running a service two will never run in parallel.
 *      
 *      
 *      
 * When sitting on info with photoprocessor running: 
 * 
 *      Something went wrong: System.InvalidOperationException A second operation was started on this context instance before a previous operation completed. This is usually caused by different threads concurrently using the same instance of DbContext. For more information on how to avoid threading issues with DbContext, see https://go.microsoft.com/fwlink/?linkid=2097913.
See full exception detail?
y/n ?

Fix: potentially caused by race condition in PhotoProcessor.Instance.StartQueue, solved by giving Process call it's own db instance. Originally the same db instance was being used to enumerate Process loop, as was being using within Process method. Occassionally Process would try to write something while data was still streaming in foreach (or I'm wrong, but it hasn't happened again).

 */



