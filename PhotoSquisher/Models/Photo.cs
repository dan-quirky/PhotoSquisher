using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSquisher.Models
{
    public class Photo
    {
        public int PhotoId { get; set; }
        public string Path { get; set; }
        //unsure what this would do
        //public List<Post> Posts { get; } = new();
    }
}
