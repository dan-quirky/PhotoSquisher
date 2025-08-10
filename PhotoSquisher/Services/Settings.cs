using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PhotoSquisher.Models;

namespace PhotoSquisher.Services
{
    public class Settings(string settingName)
    {
        public static string photoLibraryPath
        {
            get { return getSetting("libraryPath"); }
            set { setSetting("libraryPath", value); }
        }
        public static string outputPath 
        {
            get { return getSetting("outputPath"); }
            set { setSetting("outputPath", value); }
        }
        //public static string? outputPath
        //{
        //    get
        //    {
        //        using PhotoSquisherDbContext db = new();
        //        return (from c in db.Configuration
        //                where c.Config == "outputPath"
        //                select c.Value)
        //                .SingleOrDefault();
        //    }
        //    set
        //    {
        //        using PhotoSquisherDbContext db = new();
        //        var setting = (from c in db.Configuration
        //                       where c.Config == "outputPath"
        //                       select c.Value)
        //                      .SingleOrDefault();
        //        setting = value;
        //        db.SaveChanges();

        //    }

        public static string getSetting(string settingName)
        {
            using PhotoSquisherDbContext db = new();
            return db.Configuration
                .Where(c => c.Config == settingName)
                .SingleOrDefault()
                .Value;

        }
        public static void setSetting(string settingName, string value)
        {
            using PhotoSquisherDbContext db = new();
            Configuration? setting = 
                db.Configuration
                .Where(c => c.Config == settingName)
                .SingleOrDefault();
            setting.Value = value;
            db.SaveChanges();
        }

    }
}
