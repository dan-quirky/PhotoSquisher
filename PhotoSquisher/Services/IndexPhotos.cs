using PhotoSquisher.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

//TODO Create/Delete database are async and should be Tasks. Should we then be /doing/ something with those tasks?
namespace PhotoSquisher.Services
{
    internal static class IndexPhotos
    {
        private static string photoLibraryDirectory = @"C:\Users\Dan\CodeProjects\PhotoSquisher\test bits\SamplePhotoLibrary700"; //Can we keep this and other secrets in the db? That feels sensible

        public async static Task<int> ScanPhotos()
        {
            //IDEA
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
             * [DONE unique constraint on added on Path, validation before adding to ChangeTracker ] Check db for path, if it already exists then don't add duplicate
             * Implement rescanned_flag for deletions
             * Implement hashing for file updates
             * Filesystem watching (won't do this for while, if at all)
             *      
             */
            IEnumerable<string> ScannedFiles = Directory.EnumerateFiles(photoLibraryDirectory, "*", SearchOption.AllDirectories); 

            using PhotoSquisherDbContext db = new();

            foreach (string filePath in ScannedFiles)
            {
                string relativeFilePath = filePath.Substring(photoLibraryDirectory.Length + 1);
                if (Validate.pathIsUnique(db, relativeFilePath))
                {
                    Console.WriteLine($"{relativeFilePath} is unique.");
                    db.Add(new Photo { Path = relativeFilePath });
                }
                else Console.WriteLine($"{relativeFilePath} is a duplicate, skipped.");

            }

            ////view what EF is planning to commit
            //db.ChangeTracker.DetectChanges();
            //Console.WriteLine(db.ChangeTracker.DebugView.LongView);

            //try to commit, catch db error (e.g. from any constraint violation that I forgot to handle)
            try
            { 
                return await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) 
            { 
                Console.WriteLine($"Commit failed: {ex.Message}{Environment.NewLine}{ex.InnerException}"); 
                return 0; //No lines changed 
            }
           

        }

    }
}
