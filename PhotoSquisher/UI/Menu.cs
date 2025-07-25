//utility class for dynamic menus
//TODO
//Create base class interface for menu, to handle numbered and yes/no menus
//Update Action type to be able to handle methods with a return type (even if we just bin the return value)

using System;
using System.Collections.Generic;
using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;

namespace PhotoSquisher.UI
{
    internal class Menu
    {

        internal Dictionary<string, Action> MenuItems { get;  set; } //get set unnecessary if internal? probably
        internal bool noValidSelection = true;
        public Menu(Dictionary<string, Action> menuItems) //Action is datatype (kinda) for a method with no params/returns - look up Generic Delegates
        {
            //CONSTRUCTOR
            //  Sets array of menu items e.g. [Scan, Compress, Rebuild Index, ...]
            //TODO update to an object of Menu Item / Method pairs
            MenuItems = menuItems;
            MenuFlow();
        }
        public void MenuFlow()
        {
            //Print menu items
            //get user selection
            //if valid, go to linked method
            //if invalid, print invalid and go back to menu
            do 
            {
                PrintMenuItems();
                string userSelection = Console.ReadLine();
                SelectMenuItem(userSelection);
            } while (noValidSelection);
        }
        public void PrintMenuItems()
        {
            ////Get a list 
            ////Prints numbered list of options
            var enumeratedMenuItems = MenuItems.Select((val, i) => new {V = val,  N = i+1});
            foreach (var menuItem in enumeratedMenuItems)
            {
                Console.WriteLine($"{menuItem.N} - {menuItem.V.Key}");
            }
        }
        public void SelectMenuItem(string userInput)
        {
            //Take user input
            //convert to int, -1 to get index
            //invoke method at that index of menuItems
            try
            {
                int i = Int32.Parse(userInput) - 1;
                Console.WriteLine($"{ MenuItems.ElementAt(i).Key} selected");
                MenuItems.ElementAt(i).Value();//Call the method stored in Value of MenuItems at that index
                noValidSelection = false; //will this ever be hit if the method is called above?
            }
            //FIX this isn't being hit if input is invalid, just prints the menu again
            catch { Console.WriteLine("Invalid"); } 

        }

        public static void PlaceholderAction()
        {
            Console.WriteLine("This option doesn't exist yet. Returning to main menu.");
        }

        public static void FireworksPlaceholderAction()
        {
            Console.WriteLine("This option doesn't exist yet, but at least there's some fireworks. Returning to main menu." + Environment.NewLine +
                @"               *    *
   *         '       *       .  *   '     .           * *
                                                               '
       *                *'          *          *        '
   .           *               |               /
               '.         |    |      '       |   '     *
                 \*        \   \             /
       '          \     '* |    |  *        |*                *  *
            *      `.       \   |     *     /    *      '
  .                  \      |   \          /               *
     *'  *     '      \      \   '.       |
        -._            `                  /         *
  ' '      ``._   *                           '          .      '
   *           *\*          * .   .      *
*  '        *    `-._                       .         _..:='        *
             .  '      *       *    *   .       _.:--'
          *           .     .     *         .-'         *
   .               '             . '   *           *         .
  *       ___.-=--..-._     *                '               '
                                  *       *
                *        _.'  .'       `.        '  *             *
     *              *_.-'   .'            `.               *
                   .'                       `._             *  '
   '       '                        .       .  `.     .
       .                      *                  `
               *        '             '                          .
     .                          *        .           *  *
             *        .                                    '
"
                );

        }


    }
}
