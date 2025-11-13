using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoSquisher.Models;

//TODO
//Unwind ui operations (i.e. console.writeline) from services i.e. deletedatabaseasync 

namespace PhotoSquisher.Services
{
    public static class maintainDb
    {
        public async static Task<bool> CreateDatabaseAsync()
        {
            using var db = new PhotoSquisherDbContext(); //using ensures the dbcontext gets disposed of properly
            return await db.Database.EnsureCreatedAsync();
        }
        public async static Task<bool> DeleteDatabaseAsync()
        {
            try
            {
                using var db = new PhotoSquisherDbContext();
                if (db.Database.CanConnect()) { return await db.Database.EnsureDeletedAsync(); }
                else return true;
            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine($"Something went wrong: {ex.Message}");
                return false;
            }
        }
        public async static void RebuildDatabase()
        {
            try
            {

                Console.WriteLine($"Rebuilding db...");
                Console.WriteLine(Environment.NewLine
                    + "Deleting old db...");
                if (await DeleteDatabaseAsync() != true) throw new InvalidOperationException("Failed to delete database");
                Console.WriteLine("Done."
                    + Environment.NewLine
                    + "Creating new db");
                if (await CreateDatabaseAsync() != true) throw new InvalidOperationException("Failed to create database");
                Console.WriteLine("Done.");
                Console.WriteLine("Resetting to default configuration");
                //Add default values
                string defaultPath= AppContext.BaseDirectory;
                string libraryPathDefault = System.IO.Path.Join(defaultPath, "DefaultInput");
                string outputPathDefault = System.IO.Path.Join(defaultPath, "DefaultOutput");
                foreach (string path in new[] {libraryPathDefault, outputPathDefault}) Directory.CreateDirectory(path);
                using PhotoSquisherDbContext db = new();
                db.AddRange([
                    //new Configuration{Config = "libraryPath", Value = @"C:\Users\Dan\CodeProjects\PhotoSquisher\test bits\SamplePhotoLibraryHD" },
                    new Configuration{Config = "libraryPath", Value = libraryPathDefault },
                    new Configuration{Config = "outputPath", Value = outputPathDefault },
                    new IgnorePattern{ignorePattern = @"[\\/]\." }, //regex match any file/folder beginning with "."
                    new IgnorePattern{ignorePattern = @".*\.xmp$" } //regex match any xmp file 
                    ]);
                await db.SaveChangesAsync();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Something went wrong: {ex.Message}");
            }
        }


    }
}
