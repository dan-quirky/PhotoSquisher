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
        IQueryable<Photo> unprocessedPhotos;

        public int QueueCount { get { return unprocessedPhotos?.Count() ?? 0; } } //ternary + null coalescing operators to make this null-safe (google it some more) 
        public int? QueueCountInitial { get; private set; } = 0;
        public int ProcessedCount { get; private set; } = 0;
        public int FailedCount { get; private set; } = 0;
        public int IgnoredCount { get; private set; } = 0;
        private int sumOutputSize = 0;
        private int sumInputSize = 0;
        public double CompressionRatio { get { return (double)sumOutputSize / (double)sumInputSize; } }


        private async Task<bool> Process(PhotoSquisherDbContext db, Photo photo) //FIXME queue will probably loop forever if processed flags aren't set
        {
            //set up read/write paths
            string readPath = Path.Join(readPathBase, photo.Path);
            string outputPathFilename = Path.GetFileNameWithoutExtension(photo.Path) + "_Squished.jpg";
            string outputPathRelative = Path.Join(Path.GetDirectoryName(photo.Path), outputPathFilename);
            string outputPath = Path.Join(outputPathBase, outputPathRelative);
            if (!Path.Exists(readPath)) //skip if path doesn't exist
            {
                Console.WriteLine($"Couldn't locate {readPath}, skipped.");
                return false;
            }
            Console.WriteLine($"Attempting to compress {photo.Path}");

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
                Console.WriteLine($"Couldn't compress {readPath}, skipped.");
                return false;
            }
        }

        public async Task StartQueue(CancellationToken cT) //Todo add cancellation
        {

            using PhotoSquisherDbContext db = new();
            //Get queue and settings from db when queue is restarted
            unprocessedPhotos = db.Photos.Where(p => p.Processed_Flag == false);
            readPathBase = new photoLibraryPath().Value;
            outputPathBase = new outputPath().Value;
            QueueCountInitial = QueueCount;
            await foreach (
                Photo photo in unprocessedPhotos
                .AsAsyncEnumerable()
                .WithCancellation(cT)
                )
            {
                cT.ThrowIfCancellationRequested();
                await Process(db, photo);
            } 
        }

        //TEST parallel queues? multiple await calls

    }
}
