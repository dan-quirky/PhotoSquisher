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
        // bones stolen from ms's tutorial https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=visual-studio

        private string databaseName = "PhotoSquisher.db";

        //For each table in db, add DbSet<TableClass> TableProperty {get; set;}
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Configuration> Configuration { get; set; }
        public DbSet<IgnorePattern> IgnorePatterns { get; set; }
        
        /*TABLES TO IMPLEMENT
         * Compress Queue
         * Secrets
         */

        public string DbPath { get; }

        public PhotoSquisherDbContext()
        {
            //QUESTION is local app data the best place to chuck the DB?
            // The following configures EF to create a Sqlite database file in the
            // special "local" folder for your platform
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, databaseName);
            Console.WriteLine($"PhotoSquisherDbContext connection initialised");
            Console.WriteLine($"DbPath is {DbPath}");
        }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        //https://learn.microsoft.com/en-us/ef/core/modeling/indexes?tabs=fluent-api
        //https://learn.microsoft.com/en-us/ef/ef6/modeling/code-first/conventions/custom
        //Override the default schema EFCore guesses at
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //unique constraint on photo path
            modelBuilder.Entity<Photo>()
                .HasIndex(p => p.Path)
                .IsUnique();
            //set primary key on Configuration config
            modelBuilder.Entity<Configuration>()
                .HasKey(t => t.Config);
            modelBuilder.Entity<IgnorePattern>()
                .HasKey(t => t.ignorePattern);

        }
    }
}
