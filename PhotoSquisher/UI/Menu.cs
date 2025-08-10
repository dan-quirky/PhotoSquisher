using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSquisher.UI
{
    internal class Menu
    {
        protected MenuItem menuItem { get; set; } //Action is class for a method with no params/returns - look up Generic Delegates
        protected bool noValidSelection = true;
        protected char userInput;
        public Menu() :  this( new("Press any key to continue...", defaultAction) ) { }//default arguments are a bit funny with methods. Inherit full constructor instead to set default values 

        public Menu(MenuItem menuItem)
        {
            this.menuItem = menuItem;
            //MenuFlow();  
            /*
             * So can't put menuflow here or it breaks derived classes. 
             * Turns out polymorphism doesn't work in a constructor, so the base printMenuItems etc are secretly called instead of the overridden ones
             * kick off menuflow manually when you create the class
             */
        }
        public void Flow()
        {
            do
            {
                PrintMenuItems();
                GetUserSelection();
                SelectMenuItem();
            }
            while (noValidSelection);
        }
        protected virtual void PrintMenuItems()
        {
            Console.WriteLine(menuItem.Message);
        }
        protected virtual void GetUserSelection()
        {
            userInput = Console.ReadKey().KeyChar;
            Console.WriteLine();
        }
        protected virtual void SelectMenuItem()
        {
            menuItem.Invoke();
            noValidSelection = false;
        }
        protected static void defaultAction() {}

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
