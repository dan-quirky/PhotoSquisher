/*TODO
if started as service, don't run ui  
Implement service launcher
 */

using System.Linq.Expressions;
using PhotoSquisher.Models;
using PhotoSquisher.Services;
using PhotoSquisher.Tools;
using PhotoSquisher.UI;


using (PhotoSquisherDbContext db = new() )
{
    if (!db.Database.CanConnect()) maintainDb.RebuildDatabase();
}


//To be set with command line argument
bool startAsService = false;

if (startAsService)
{
    while (true)
    {
        PsLogger.LogLine("PhotoSquisher service running");
        ServiceLauncher.ServiceStart();
        await Task.Delay((int)60e3);

    }
}
else
{
    try { MainMenu.Run(); }
    catch (Exception ex) { PsLogger.writeOutCaughtException(ex); }
}

public static class Global
{
    public static string logPath = System.IO.Path.Join(AppContext.BaseDirectory, "logs", $"PhotoSquisher_{DateTime.Now.ToString("yyyy-MM-dd_HHmmss")}.log");
}

/*
 * BUGS
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



