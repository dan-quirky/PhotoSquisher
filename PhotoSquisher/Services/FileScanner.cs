using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PhotoSquisher.Models;
using PhotoSquisher.Tools;

namespace PhotoSquisher.Services
{
    //IDEA
    /*On first build need to enumerate every file
     * On subsequent scans can ignore anything already in database
     *  What's the best way to do that?
     *  Naively:
     *  For each file
     *  If path in database
     *      update if unique
     *      skip
     *      else add new record
     * Commit every 100 lines
     * if batch fails, add to failedFiles list
     *      when first pass complete, attempt to rescan and commit one file at a time. Catch fails and log
     *  
     *  PROBLEMS:   
     *      How to catch deleted files -> scan existing filepaths
     *          Faster way - Rescanned_Flag column. 
     *          On scan start, set all to 0. Read thru file system once, set flag to 1 when file is scanned
     *          On scan end, get all entries with flag = 0
     *          Check whether anything exists at those paths - if no, set deleted flag to yes (don't delete record yet, need to remove the compressed photo as weLL)
     *      How to update out of date files -> hashing or metadata
     *      How to avoid long scans - watch file system, with occasional full rescans
     *      
     * TODO 
     * Enumerate sub directories first, maybe a little kinder on the memory to enum files one subdir at a time. (On the other hand, it's already blazing fast so why bother)
     * [DONE unique constraint on added on Path, validation before adding to ChangeTracker ] Check db for path, if it already exists then don't add duplicate
     * Implement rescanned_flag for deletions
     * Implement hashing for file updates
     * Implement .ignore logic, but keep this in db
     * Filesystem watching (won't do this for while, if at all)
     * Convert consoleWritelines to logging file output
     *      
     */


    public class FileScanner //Slightly naively written singleton, PhotoProcessor has a more std implementation
    {
        
        static int batchCountLimit = 1000;

        // This might be causing an System.TypeInitializationException because it's running far too early
        //static string photoLibraryDirectory = new photoLibraryPath().Value;
        //Solution: lazy loading, so nothing happens when the class is first initialised
        //(lifted wholesale from an ai, go figure out what this actually means if it works)
        static string? _photoLibraryDirectory;
        static string photoLibraryDirectory
            => _photoLibraryDirectory ??= new photoLibraryPath().Value;

        Queue<string> scanQueue;
        Queue<string> batchQueue;
        Queue<string> failedQueue;

        public int? QueueCount { get { return scanQueue.Count; } }
        public int? QueueCountInitial { get; } = 0;
        public int ProcessedCount { get; private set; }
        public int FailedCount { get { return failedQueue.Count; } } 
        public int IgnoredCount { get; private set; } = 0;
        public static FileScanner? Instance { get; private set; } 
        //Set this to instance in constructor, to access from elsewhere in 
        //Photoprocessor uses more standard singleton pattern, this was done fairly naively
        public FileScanner()
        {
            //TODO Break scan into chunks, it hangs for a while when starting scan while EnumerateFiles does work
            IEnumerable<string> AllFiles = Directory.EnumerateFiles(photoLibraryDirectory, "*", SearchOption.AllDirectories);  
            scanQueue = new Queue<string>(AllFiles.Count()); //Set capacity to max no.files
            using (PhotoSquisherDbContext db = new())
                foreach (string file in AllFiles)
                    if (!Validate.PathIsIgnored(db, file))
                        scanQueue.Enqueue(file);
                    else IgnoredCount++;
            failedQueue = new Queue<string>();
            QueueCountInitial = QueueCount;
            if (Instance != null) { throw new Exception("Too many instances"); }
            Instance = this;
            Run();
        }
        public async Task Run() 
        {
            await Scan();
            Instance = null; // reset instance when done
        }
        public async Task Scan()
        {
            while (QueueCount > 0)
            {
                Console.WriteLine("New Batch--------------------------------------");
                //await Task.Delay(500);
                ProcessedCount += await ScanBatch();
            }
            //FlagDeletedFiles
            //Console.WriteLine("Fails: ");
            foreach (string fail in failedQueue) Console.WriteLine(fail);
            await RescanFails();
            Console.WriteLine($"Scan complete : {QueueCountInitial} files scanned {Environment.NewLine}{ProcessedCount} files added, {IgnoredCount} ignored, {FailedCount} failed.");

        }
        async Task<int> ScanBatch()
        //returns number of records added
        {
            using PhotoSquisherDbContext db = new();
            //reset batch count + empty batch queue
            batchQueue = new Queue<string>(batchCountLimit); //Enqueue is O(1) if capacity isn't reached, O(n). We know batchQueue will never exceed batchCountLimit so set capacity to that
            int? QueueCountAtLoopStart = QueueCount;
            while (batchQueue.Count < batchCountLimit && batchQueue.Count < QueueCountAtLoopStart) //While there are still items in the queue and batch size hasn't reached limit
            {
                //PsDebug.printVariable(batchQueue.Count);
                //PsDebug.printVariable(batchCountLimit);
                //PsDebug.printVariable(QueueCountAtLoopStart);
                string filePath = scanQueue.Dequeue(); //Remove from queue
                batchQueue.Enqueue(filePath);
                ScanFile(db, filePath);
            }
            try { return await db.SaveChangesAsync(); }
            catch (DbUpdateException ex)
                {
                    //Console.WriteLine($"Commit failed: {ex.Message}{Environment.NewLine}{ex.InnerException}");
                    failedQueue.EnsureCapacity(failedQueue.Count + batchQueue.Count); //better to increase capacity in single op
                    foreach (string fail in batchQueue) failedQueue.Enqueue(fail);
                    return 0; //No lines changed 
                }
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.Message);
                //    return 0;
                //}
        }
        void ScanFile(PhotoSquisherDbContext db, string filePath)
        {
            string relativeFilePath = filePath.Substring(photoLibraryDirectory.Length + 1);
            if (Validate.pathIsUnique(db, relativeFilePath))
            {
                //Console.WriteLine($"{relativeFilePath} is unique.");
                db.Add(new Photo
                {
                    Path = relativeFilePath,
                    Processed_Flag = false
                });
            }
            else
            {
                //Console.WriteLine($"{relativeFilePath} is a duplicate, skipped.");
            }
        }

        async Task RescanFails()
        {
            using PhotoSquisherDbContext db = new();
            string[] fails;
            Queue<string> retryQueue = new(failedQueue); //copy failed queue to retryQueue
            failedQueue = new Queue<string>(retryQueue.Count); //reset failed queue
            while (retryQueue.Count > 0)
            {
                string filePath = retryQueue.Dequeue();
                ScanFile(db, filePath);
                try { ProcessedCount += await db.SaveChangesAsync(); } //commit retries one line at a time
                catch (DbUpdateException ex)
                {
                    //Console.WriteLine($"Commit failed: {ex.Message}{Environment.NewLine}{ex.InnerException}");
                    failedQueue.Enqueue(filePath);
                }

            }
        }

    }

}

