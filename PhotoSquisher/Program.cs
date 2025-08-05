//default namespace is PhotoSquisher, default class is Program, don't need to explcitly call main

//TODO
//[Done, implemented Menu class There's probably a better (dynamic) way to do menus but fine for right now
//Use menu class for main menu too


using PhotoSquisher.Services;
using PhotoSquisher.UI;
using PhotoSquisher.Tools;


bool exit = false;
Console.WriteLine("PhotoSquisher - Copyright 2025 No One In Particular"); Console.WriteLine(Environment.NewLine);
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



} while (exit != true);

