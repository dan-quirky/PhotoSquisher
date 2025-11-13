using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using PhotoSquisher.Models;
using PhotoSquisher.Services;
using PhotoSquisher.Tools;

namespace PhotoSquisher.Services 
{
    //rewrite of ProcessQueue_static explicitly as singleton
    public sealed class PhotoProcessor : IQueueHandler //don't think this will be useful here
    {
        //START singleton pattern:
        private static readonly Lazy<PhotoProcessor> lazy = new(() => new PhotoProcessor());
        public static PhotoProcessor Instance { get { return lazy.Value; } } //Use this to access outside of other methods. Instance will be created if it doesn't currently exist.
        private PhotoProcessor() { } //empty private constructor
         //END Singleton pattern


        string readPathBase;
        string outputPathBase;
        IQueryable<Photo>? unprocessedPhotos;

        public int QueueCount //This feels overcomplicated tbh
        {
            get
            {
                try { return unprocessedPhotos?.Count() ?? 0; }  //ternary + null coalescing operators to make this null-safe (google it some more)
                catch (ObjectDisposedException ex) { return 0; }
            }
        }
        public int? QueueCountInitial { get; private set; } = 0;
        public int ProcessedCount { get; private set; } = 0;
        public int FailedCount { get; private set; } = 0;
        public int IgnoredCount { get; private set; } = 0;
        private int sumOutputSize = 0;
        private int sumInputSize = 0;
        public double CompressionRatio { get { return (double)sumOutputSize / (double)sumInputSize; } }


        private async Task<bool> Process(int photoId)
        {
            using PhotoSquisherDbContext db = new(); //give process its own db instance to avoid race condition
            //set up read/write paths
            Photo photo = db.Photos.Where(p => p.PhotoId == photoId).Single();
            string readPath = Path.Join(readPathBase, photo.Path);
            string outputPathFilename = Path.GetFileNameWithoutExtension(photo.Path) + "_Squished.jpg";
            string outputPathRelative = Path.Join(Path.GetDirectoryName(photo.Path), outputPathFilename);
            string outputPath = Path.Join(outputPathBase, outputPathRelative);
            if (!Path.Exists(readPath)) //skip if path doesn't exist
            {
                PsLogger.LogLine($"Couldn't locate {readPath}, skipped.");
                return false;
            }
            PsLogger.LogLine($"Attempting to compress {photo.Path}");

            bool ProcessedSucessfully = await SquishPhoto.Compress(readPath, outputPath); //compress the photo

            if (ProcessedSucessfully) //if sucessful, update flag and path in db
            {
                photo.Processed_Flag = true;
                photo.Processed_Path = outputPathRelative;
                await db.SaveChangesAsync();

                sumInputSize += (int)new FileInfo(readPath).Length;
                sumOutputSize += (int) new FileInfo(outputPath).Length;


                return true;
            }
            else
            {
                PsLogger.LogLine($"Couldn't compress {readPath}, skipped.");
                photo.Processed_Flag = true;
                photo.Failed_Flag = true;
                await db.SaveChangesAsync();
                return false;
            }
        }

        public async Task StartQueue(CancellationToken cT) //Todo add cancellation
        {

            using PhotoSquisherDbContext db = new();
            //Get queue and settings from db when queue is restarted
            unprocessedPhotos = db.Photos.Where(p => p.Processed_Flag == false);
            if (!unprocessedPhotos.Any())
            {
                PsLogger.LogLine("No unprocessed photos remaining");
                return;
            }
             
            readPathBase = new photoLibraryPath().Value;
            outputPathBase = new outputPath().Value;
            QueueCountInitial = QueueCount;
            await foreach (Photo photo in unprocessedPhotos
                .AsAsyncEnumerable()
                .WithCancellation(cT)
                )
            {
                cT.ThrowIfCancellationRequested();
                await Process(photo.PhotoId);
            }

            
        }

        //TEST parallel queues? multiple await calls

    }
}
