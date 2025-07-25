using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoSquisher.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;


//TODO this is mainly garbage I don't want to rewrite. Either make is generically useful or delete it.
namespace PhotoSquisher.Tools
{
    internal class Misc
    {
        //todo make this a tools method
        public static void test()
        {
            Console.WriteLine("This works fine");
            List<string> ScannedFiles = ["this", "works", "fine"];
            foreach (string currentFile in ScannedFiles)
            {
                var type = currentFile.GetType();
                var properties = type.GetProperties();
                Console.WriteLine($"{Environment.NewLine} Inspecting {currentFile} ( {type.Name} )");
                foreach (var prop in properties)
                {
                    string valueString;
                    try
                    {
                        var value = prop.GetValue(currentFile);
                        valueString = value?.ToString() ?? "null";
                    }
                    catch (Exception ex)
                    {
                        valueString = $"(Couldn't read: {ex.GetType().Name})";
                    }

                    Console.WriteLine($"• {prop.Name} ({prop.PropertyType.Name}) → {valueString}");
                }
            }
        }


        //CRUD Methods test
        public async static void CRUDtest() //need async to run awaits. Generally the return type should be Task for promises but void works
        {
            using var db = new PhotoSquisherDbContext();

            async void printAllPhotos()
            {
                Console.WriteLine("Querying for all photos");
                foreach (var row in await db.Photos.ToListAsync())
                {
                    var properties = row.GetType().GetProperties();
                    string readoutLine = "";
                    foreach (var p in properties)
                    {
                        readoutLine += $"{p.Name}: {p.GetValue(row)}    ";
                    }
                    Console.WriteLine(readoutLine);
                };
                Console.WriteLine(Environment.NewLine);
            };


            // Note: This sample requires the database to be created before running.
            Console.WriteLine($"Database path: {db.DbPath}.");

            //Output all photos in DB
            printAllPhotos();

            // Create
            Console.WriteLine("Inserting a new photo");
            db.Add(new Photo { Path = "/this/could/be/a/path" });
            await db.SaveChangesAsync(); //commit //why is it awaiting an async, rather than just using a blocking method?

            //Output all photos in DB
            printAllPhotos();

            // Read
            Console.WriteLine("Querying for a phpto");
            var photoToUpdate = await db.Photos //gives a dbSet<Photo> rep'ing the Photo table
                .OrderByDescending(p => p.PhotoId) //latest id 
                .FirstAsync(); //first in the list

            //Output all photos in DB
            printAllPhotos();

            // Update
            Console.WriteLine("Updating the photo");
            photoToUpdate.Path = "/another/path/here";
            await db.SaveChangesAsync();

            //Output all photos in DB
            printAllPhotos();

            // Delete
            Console.WriteLine("Delete the photo");
            db.Remove(photoToUpdate);
            await db.SaveChangesAsync();

            //Output all photos in DB
            printAllPhotos();
        }



    }
}
