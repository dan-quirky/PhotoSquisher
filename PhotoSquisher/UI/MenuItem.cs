using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSquisher.UI
{
    public class MenuItem(string message, Action<object> method, object? arg)
    {
        public MenuItem(string message, Action method) : this(message, _ => method(), null)
        { 
            //if delegate takes no args
            //pass null as arg,cast(?) the action<object> to Action
        } 
        public string Message { get; } = message;
        private Action<object> Method { get; } = method;
        private object Arg { get; } = arg;
        public void Invoke() { Method(Arg); }
    }
}
