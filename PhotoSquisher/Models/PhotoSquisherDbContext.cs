using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PhotoSquisher.Models
{
    internal class PhotoSquisherDbContext : DbContext
    {
        // lovingly stolen from ms's tutorial https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=visual-studio

        //TODO: These paths should be set with some other method by the user, and persist in a config file
        private string databaseDirectory = "path";
        private string databaseName = "PhotoSquisher.db";

        //For each table in db, add DbSet<TableClass> TableProperty {get; set;}
        public DbSet<Photo> Photos { get; set; }
        /*TABLES TO IMPLEMENT
         * Compress Queue
         * Secrets
         */

        public string DbPath { get; }

        public PhotoSquisherDbContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, databaseName);
            Console.WriteLine($"PhotoSquisherDbContext connection initialised");
            Console.WriteLine($"DbPath is {DbPath}");
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        //TODO DONE Create database within the app 

    }
}
