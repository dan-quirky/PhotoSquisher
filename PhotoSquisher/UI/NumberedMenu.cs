//utility class for dynamic menus
//TODO
//Create base class interface for menu, to handle numbered and yes/no menus
//Update Action type to be able to handle methods with a return type (even if we just bin the return value)
//Add return to main menu option, and while loop to stay on the same menu until explicitly returned

using System;
using System.Collections.Generic;
using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;

namespace PhotoSquisher.UI
{
    internal class NumberedMenu : Menu
    {
        internal MenuItem[] menuItems { get; set; } //array of menu items
        public NumberedMenu( MenuItem[] menuItems )
        {
            //CONSTRUCTOR
            this.menuItems = menuItems;
        }
        protected override void PrintMenuItems()
        {
            ////Prints numbered list of options
            var enumeratedMenuItems = menuItems.Select((v, i) => ( v, i + 1 ) );
            foreach ( ( MenuItem MenuItem, int position) in enumeratedMenuItems )
            {
                Console.WriteLine($"{position} - {MenuItem.Message}");
            }
        }
        protected override void GetUserSelection()
        {
            base.GetUserSelection();
        }
        protected override void SelectMenuItem()
        {
            //Take user input
            //convert to int, -1 to get index
            //invoke method at that index of menuItems
            try
            {
                Console.Clear();
                //Console.WriteLine($"user input: {userInput}");
                int i = Int32.Parse(userInput.ToString()) - 1;
                Console.WriteLine($"{menuItems[i].Message} selected");
                menuItems[i].Invoke();//Call the method stored in Value of MenuItems at that index
                noValidSelection = false; //will this ever be hit if the method is called above?
            }
            catch (FormatException ex) { Console.Clear(); Console.WriteLine($"Invalid selection: {userInput}");  }
            catch (IndexOutOfRangeException ex) { Console.Clear(); Console.WriteLine($"Invalid selection: {userInput}"); }

        }


    }
    
    internal class numberedMenu_Dictionary ////defunct
    {

        internal Dictionary<string, Action> MenuItems { get; set; } //I don't remember why i didn't just make this a list of tuples
        internal bool noValidSelection = true;
        public numberedMenu_Dictionary(Dictionary<string, Action> menuItems) //Action is datatype (kinda) for a method with no params/returns - look up Generic Delegates
        {
            //CONSTRUCTOR
            //  Sets array of menu items e.g. [Scan, Compress, Rebuild Index, ...]
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
            var enumeratedMenuItems = MenuItems.Select((val, i) => new { V = val, N = i + 1 });
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
                Console.WriteLine($"user input: {userInput}");
                int i = Int32.Parse(userInput) - 1;
                Console.WriteLine($"{MenuItems.ElementAt(i).Key} selected");
                MenuItems.ElementAt(i).Value();//Call the method stored in Value of MenuItems at that index
                noValidSelection = false; //will this ever be hit if the method is called above?
            }
            catch
            {
                Console.WriteLine("Invalid selection"); 
            }

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
