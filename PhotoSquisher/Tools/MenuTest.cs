

using PhotoSquisher.Services;
using PhotoSquisher.UI;

namespace PhotoSquisher.Tools
{
    internal class MenuTest
    {
        public static void Run()
        {
            testMenuNoArgs();
            testMenu2Args();
            testYnMenu();
            testNumberedMenu();
        }
        internal static void testMenuNoArgs()
        {
            Console.WriteLine("Menu base class, should continue to the next line");
            new Menu().Flow();
        }
        internal static void testMenu2Args()
        {
            Console.WriteLine("Menu with custom message and action");
            new Menu("Custom Test message. Any key to continue.", Menu.PlaceholderAction).Flow();
        }
        internal static void testNumberedMenu_Dictionary()
        {
            Console.WriteLine("numbered menu, enter a number. Loops on invalid selections. Don't use this one, it uses dictionary unecessarily");
            new numberedMenu_Dictionary(new Dictionary<string, Action>
            {
                { "Continue", ()=>Console.WriteLine("Did continue")},
                { "Main Menu", ()=>{ } },
                { "another option",  ()=>Console.WriteLine("Did another option") }
            });
        }
        internal static void testNumberedMenu()
        {
            Console.WriteLine("numbered menu, enter a number. Loops on invalid selections");
            new NumberedMenu ( new []
            {
                new MenuItem ( "Continue", ()=>Console.WriteLine("Did continue") ),
                new MenuItem ( "Main Menu", ()=>{ } ),
                new MenuItem ( "another option",  ()=>Console.WriteLine("Did another option") ),
            }).Flow();
        }
        internal static void testYnMenu()
        {
            Console.WriteLine("y/n menu, enter y Y n N. Loops on invalid selections");
            new YnMenu("Are you sure you want to do this?", ()=>Console.WriteLine("You've only gone and done it...")).Flow();            
        }
    }
}
