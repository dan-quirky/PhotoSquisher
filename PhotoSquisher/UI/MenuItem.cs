using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSquisher.UI
{
    internal class MenuItem(string message, Action method)
    {
        public string Message { get; } = message;
        public Action Method { get; } = method;
    }
}
