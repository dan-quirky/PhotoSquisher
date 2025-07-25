//default namespace is PhotoSquisher, default class is Program, don't need to explcitly call main

//TODO
//[Done, implemented Menu class There's probably a better (dynamic) way to do menus but fine for right now
//Use menu class for main menu too


using PhotoSquisher.Services;
using PhotoSquisher.UI;


bool exit = false;
Console.WriteLine("PhotoSquisher - Copyright 2025 No One In Particular"); Console.WriteLine(Environment.NewLine);
do
{

    Console.WriteLine("Main Menu:");
    Menu m = new Menu(new Dictionary<string, Action>
    {
        {"Info",Info.GetInfo},
        {"Scan Photos",IndexPhotosUI.Run },
        {"Compress Photos", Menu.PlaceholderAction}
    });

    //old menu
    /*
    Console.WriteLine
        ("1 - Info" + Environment.NewLine +
        "2 - Scan & Queue" + Environment.NewLine +
        "3 - Compress Queue" + Environment.NewLine +
        "");
    string userInput = Console.ReadLine(); Console.WriteLine(Environment.NewLine);

    switch (userInput)
    {
        case "1":
            Info.GetInfo(); break;
        case "2":
            IndexPhotosUI.Run(); break;
    */

    
} while (exit != true);

