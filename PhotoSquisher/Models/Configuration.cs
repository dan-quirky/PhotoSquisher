using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSquisher.Models
{
    //to store app settings
    //have you just reinvented the dictionary?
    //it's possible
    //should eliminate need for a seperate config/secrets file though.
    public class Configuration
    {
        public int ConfigId { get; set; }
        public string Config { get; set; }
        public string Value { get; set; }

        public Configuration() { }
        public Configuration(string k, string v)
        {
            Config = k;
            Value = v;
        }
    }
}
