using PhotoSquisher.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

//TODO Create/Delete database are async and should be Tasks. Should we then be /doing/ something with those tasks?
namespace PhotoSquisher.Services
{
    internal static class IndexPhotos
    {
        private static string photoLibraryDirectory = @"C:\Users\Dan\CodeProjects\PhotoSquisher\SamplePhotoLibrary700"; //Can we keep this and other secrets in the db? That feels sensible

        public async static Task<int> ScanPhotos()
        {
            //IDEA
            //EnumerateFiles https://learn.microsoft.com/en-us/dotnet/standard/io/how-to-enumerate-directories-and-files
            /*On first build need to enumerate every file
             * On subsequent scans can ignore anything already in database
             *  What's the best way to do that?
             *  Naively:
             *  For each file
             *  If path not in database
             *      skip
             *      else write
             *  PROBLEMS:   
             *      How to catch deleted files -> scan existing filepaths
             *          Faster way - Rescanned_Flag column. 
             *          On scan start, set all to 0. Read thru file system once, set flag to 1 when file is scanned
             *          On scan end, get all entries with flag = 0
             *          Check whether anything exists at those paths - if no
             *      How to update out of date files -> hashing or metadata
             *      How to avoid long scans - watch file system, with occasional full rescans
             *      
             * TODO 
             * Enumerate sub directories first, be a little kinder on the memory to enum files one subdir at a time. (On the other hand, it's already blazing fast so why bother)
             * Check db for path, if it already exists
             * Implement rescanned_flag for deletions
             * Implement hashing for file updates
             *      
             */
            var ScannedFiles = Directory.EnumerateFiles(photoLibraryDirectory, "*", SearchOption.AllDirectories); 

            using PhotoSquisherDbContext db = new();

            foreach (string currentFile in ScannedFiles)
            {
                //USE PATH CLASS FOR ALL OF THIS
                string relativeFilePath = currentFile.Substring(photoLibraryDirectory.Length + 1);
                //TODO Basefilename, filetype
                Console.WriteLine(currentFile);
                Console.WriteLine(relativeFilePath);
                db.Add(new Photo { Path = relativeFilePath });
            }
            return await db.SaveChangesAsync();

        }
        //todo make this a tools method
        public static void test() { 
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
        public async static Task<bool> CreateDatabaseAsync()
        {
            using var context = new PhotoSquisherDbContext(); //using ensures the dbcontext gets disposed of properly, not sure how tho
            return await context.Database.EnsureCreatedAsync();
        }
        public async static Task<bool> DeleteDatabaseAsync()
        {
            try
            {
                using var context = new PhotoSquisherDbContext();
                return await context.Database.EnsureDeletedAsync();
            }
            catch (System.IO.IOException ex)
            {     
                Console.WriteLine($"Something went wrong: {ex.Message}");
                return false;
            }
        }
        public async static void RebuildDatabase()
        {
            //BUG have observed this not deleting the db without complainign about it, look into it 
            //Todo This should only continue if the previous step didn't fail (i.e. delete fails if db is open in viewer). Sounds like a Task thing.
            try
            {
                Console.WriteLine($"Rebuilding db...");
                Console.WriteLine(Environment.NewLine
                    + "Deleting old db...");
                if (await DeleteDatabaseAsync() != true) throw new InvalidOperationException("Failed to delete database"); 
                Console.WriteLine("Done."
                    + Environment.NewLine
                    + "Creating new db");
                if (await CreateDatabaseAsync() != true ) throw new InvalidOperationException("Failed to create database"); 
                Console.WriteLine("Done.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Something went wrong: {ex.Message}" );
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
