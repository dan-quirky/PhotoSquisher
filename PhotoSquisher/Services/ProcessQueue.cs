using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PhotoSquisher.Models;
using PhotoSquisher.Tools;

namespace PhotoSquisher.Services
{
    //TODO (once it works at all)
    //not hugely OOP but maybe thats fine? Is there any advantage to making this an initialisable queue object which can be binned when done?
    //also a lot of moving parts, need to make this slightly more robust so e.g. an error compressing the photos doesn't mess up the db update
    //Could/should also be async/multithreaded
    internal class ProcessQueue
    {
        static string outputPathBase { get; set; }
        
            
        public static async void Run()
        {
            try
            {
                /* 
                 select all rows in Photos where processedFlag != 1
                  for each photo
                   set flag to 2 (pending, should catch this before it gets written to db tho
                   call CompressPhoto with photo.path, outputpath
                     if sucessful
                       set flag to 1 
                       set processedPath to outputpath
                 end for each
                 commit
                */
                using PhotoSquisherDbContext db = new();
                string? outputPathBase = (from c in db.Configuration
                                          where c.Config == "outputPath"
                                          select c.Value)
                                     .SingleOrDefault();
                string? readPathBase = (from c in db.Configuration
                                        where c.Config == "libraryPath"
                                        select c.Value)
                                     .SingleOrDefault();


                var unprocessedPhotos = db.Photos.Where(p => p.Processed_Flag == false);
                foreach (var photo in unprocessedPhotos)
                {
                    string readPath = Path.Join(readPathBase, photo.Path);

                    string outputPathFilename = Path.GetFileNameWithoutExtension(photo.Path) + "_Squished.jpg";
                    string outputPathRelative = Path.Join( Path.GetDirectoryName(photo.Path), outputPathFilename );
                    string outputPath = Path.Join(outputPathBase, outputPathRelative);

                    //check properties                
                    PsDebug.printProperties(photo);
                    PsDebug.printVariable(outputPath);
                    PsDebug.printVariable(readPath);

                    //Validate Path exists
                    if (!Path.Exists(readPath)) 
                    {
                        Console.WriteLine($"Couldn't locate {readPath}, skipped.");
                        continue;
                    }
                    //Call compression method
                    bool compressTask = await ProcessPhoto.Compress(readPath, outputPath);
                    //Validate sucessful compression
                    if (compressTask != true)
                    {
                        Console.WriteLine($"Couldn't compress {readPath}, skipped.");
                        continue;
                    }
                    Console.WriteLine("Saving Changes...");
                    //update db
                    photo.Processed_Flag = true;
                    photo.Processed_Path = outputPathRelative;
                    //
                    await db.SaveChangesAsync();
                    Console.WriteLine("Done");
                }
            }
            catch (Exception ex) { Console.WriteLine("something broke" + ex.Message); PsDebug.printCaughtException(ex); } 



        }


    }
}
