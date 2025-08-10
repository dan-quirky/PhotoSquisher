using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSquisher.UI
{
    internal class YnMenu(MenuItem menuItem) : Menu(menuItem) //"Primary Constructor", looks fun
    {

        protected override void PrintMenuItems() 
        {
            base.PrintMenuItems();
            Console.Write("y/n ?    ");            
        }
        protected override void SelectMenuItem()
        {

            //switch (Char.ToLower(userInput))
            switch (userInput)
            {
                case 'y':
                    base.SelectMenuItem(); break;
                case 'n':
                    noValidSelection = false; break;
                default:
                    Console.WriteLine("Invalid selection"); break;
            }
        }
    }
}
