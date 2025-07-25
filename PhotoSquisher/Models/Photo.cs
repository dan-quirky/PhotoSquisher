using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PhotoSquisher.Models
{
    public class Photo
    {
        public int PhotoId { get; set; }
        public string Path { get; set; }
        //public List<Post> Posts { get; } = new(); //This would add the photoid foreign key to the Posts table? or something similar anything
    }
}
