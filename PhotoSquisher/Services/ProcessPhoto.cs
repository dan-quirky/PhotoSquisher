using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;
using ImageMagick.Formats;
using PhotoSquisher.UI;
using PhotoSquisher.Tools;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;

//using static System.Net.Mime.MediaTypeNames;
//using Microsoft.Extensions.Options;
//using static System.Net.Mime.MediaTypeNames;


/*TODO
 * Read Image
 * Write Image
 *  Set compression settings
 *  Create dir if not exist
 *
 *Make compression async/multithreaded
 *  
 *         // https://github.com/dlemstra/Magick.NET/tree/main/docs
 */

namespace PhotoSquisher.Services
{
    internal class ProcessPhoto
    {
        static MagickImage? photo; //for threading, probably need seperate instances of this
        static MagickImageInfo? info;


        //todo rewrite to return a promise (task?) 
        public async static Task<bool> Compress(string filePath, string outputPath) 
        {
            bool taskResult = false; 
            try
            {
                Console.WriteLine($"Compressing {filePath}");
                OpenFile(filePath);
                Debug.WriteLine("printing photo properties");
                PsDebug.printProperties(info);

                //Compression Settings
                photo.ColorSpace = ColorSpace.RGB; //Convert to linear RGB for processing https://imagemagick.org/script/formats.php
                double targetResolutionRatio = 12e6 / (info.Width * info.Height); //If image is greater than 12MP, resize to approx 12MP. large performance impact.
                if (targetResolutionRatio < 1)
                {
                    Console.WriteLine("Reducing Resolution...");
                    double scalingFactor = Math.Sqrt(targetResolutionRatio);
                    uint newWidth = (uint)(info.Width * scalingFactor);
                    uint newHeight = (uint)(info.Height * scalingFactor);
                    photo.Resize(newWidth, newHeight); //Resize w x h without changing aspect ratio. Unclear how it decides to fix a particular dimension.
                }
                photo.Quality = 75;
                photo.Settings.SetDefine(MagickFormat.Jpeg, "interlace", "Plane"); // Enable interlacing, improves compression for *reasons*, no obvious quality loss
                photo.ColorSpace = info.ColorSpace; //Convert back to original colourspace (usually sRGB)

                                                                           
                try{ Directory.CreateDirectory( Path.GetDirectoryName(outputPath) ); }
                catch (IOException ex) { }; //do nothing if dir already exists 
                Console.WriteLine($"Writing to {outputPath}");   
                photo.Write(outputPath, MagickFormat.Jpg);
                Console.WriteLine($"Done");
                Console.WriteLine();

                taskResult = true;
            }
            catch (MagickException ex)
            {
                Console.WriteLine($"Skipped {filePath}: {ex.Message} {ex.InnerException}");
            }
            catch (Exception ex)//This generic catch won't be deactivated by the Debugging check. Should probably care about that.
            {
                PsDebug.printCaughtException(ex);
                Console.WriteLine(ex.Message);
            }
            return taskResult;

        }
        internal static void OpenFile(string filePath)
        {
            try
            {
                info = new(filePath);//not idisposable
                photo = new MagickImage(filePath);
            }
            catch (MagickException ex) when (ex.Message.Contains("Unsupported file format"))
            {
                Console.WriteLine(Environment.NewLine + "Invalid file extension, reading file with fallback option");
                FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
                photo = new MagickImage(fs);
                fs.Position = 0;
                info = new(fs);//not idisposable
            }

        }

        public static void readWriteImageTest()
        {
            try
            {
                //read from file
                //string readFilePath = @"C:\Users\Dan\CodeProjects\PhotoSquisher\test bits\astronaut.jpg";
                string readFilePath = @"C:\Users\Dan\CodeProjects\PhotoSquisher\test bits\SamplePhotoLibrary10\AnAlbum\AnotherNestedAlbum\image-57.jpg";
                using var imageFromFile = new MagickImage(readFilePath);
                var info = new MagickImageInfo(readFilePath);
                foreach (var property in info.GetType().GetProperties())
                {
                    Console.WriteLine($"{property.Name}: {property.GetValue(info)}");
                }


                //write to new file
                Console.WriteLine("Writing to new file");
                string writeFilePath = @"C:\Users\Dan\CodeProjects\PhotoSquisher\test bits\ImageMagicOutput\" + "testOutput.jpg";
                if (Path.Exists(writeFilePath))
                {
                    File.Delete(writeFilePath);
                    Console.WriteLine("Deleted existing file. Probably don't need this generally, will just overwrite an existing image anyway");
                }
                imageFromFile.Write(writeFilePath, MagickFormat.Jpeg);

                //https://github.com/dlemstra/Magick.NET/blob/main/samples/Magick.NET.Samples/LosslessCompression.cs
                //https://github.com/dlemstra/Magick.NET/blob/main/src/Magick.NET/Formats/Jpeg/JpegWriteDefines.cs

                //Apply these cli settings
                //magick unoptimized.jpg -sampling-factor 4:2:0  -quality 85 -interlace Plane -gaussian-blur 0.05 -colorspace RGB optimised.jpg


                Console.WriteLine("Applying compression settings");

                imageFromFile.ColorSpace = ColorSpace.RGB; //Convert to linear RGB for processing https://imagemagick.org/script/formats.php
                imageFromFile.Quality = 75; // Set the quality to 75%
                //omitting, jpg lib will use defauts
                //imageFromFile.Settings.SetDefine(MagickFormat.Jpeg, "sampling-factor", "4:2:0"); // Set the sampling factor to 4:2:0
                imageFromFile.Settings.SetDefine(MagickFormat.Jpg, "interlace", "Plane"); // Enable interlacing, improves compression for *reasons*, no obvious quality loss
                imageFromFile.Settings.SetDefine(MagickFormat.Jpg, "garbage", "whynot"); // Q does it complain if you feed it trash? no
                double targetResolutionRatio = 12e6 / (info.Width * info.Height); //If image is greater than 12MP, resize to approx 12MP
                if (targetResolutionRatio < 1)
                {
                    Console.WriteLine("Reducing Resolution...");
                    double scalingFactor = Math.Sqrt(targetResolutionRatio);
                    uint newWidth = (uint)(info.Width * scalingFactor);
                    uint newHeight = (uint)(info.Height * scalingFactor);
                    imageFromFile.Resize(newWidth, newHeight); //Resize w x h without changing aspect ratio. Unclear how it decides to fix a particular dimension.
                }
                //imageFromFile.GaussianBlur(0.05); //Slight blur, improves compression, maybe softens photo too much
                imageFromFile.ColorSpace = info.ColorSpace; //Convert back to original colourspace (usually sRGB)
                imageFromFile.Write(writeFilePath, MagickFormat.Jpg);

                Console.WriteLine(Environment.NewLine + "Output Photo Info");
                info = new MagickImageInfo(writeFilePath);
                foreach (var property in info.GetType().GetProperties())
                {
                    Console.WriteLine($"{property.Name}: {property.GetValue(info)}");
                }


                double inputSize = new FileInfo(readFilePath).Length;
                double outputSize = new FileInfo(writeFilePath).Length;
                double CompressionRatio = outputSize / inputSize;
                Console.WriteLine($"Compression ratio: {CompressionRatio:P2}");

            }
            catch (Exception ex)
            { Console.WriteLine($"Something went wrong.{Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.InnerException}"); }

        }

    }
}
