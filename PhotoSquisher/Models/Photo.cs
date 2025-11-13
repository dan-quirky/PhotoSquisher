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
        public required string Path { get; set; }
        public bool Processed_Flag { get; set; } = false;
        public string? Processed_Path { get; set; }
        public bool Failed_Flag { get; set; } = false;
    }
}
