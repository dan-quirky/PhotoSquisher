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
    //base class for any settings
    public class Setting(string settingName)
    {
        string settingName = settingName;
        public string Value
        {
            get { return getSetting(); }
            set { setSetting(value); }
        }
        string getSetting()
        {
            using PhotoSquisherDbContext db = new();
            return db.Configuration
                .Where(c => c.Config == settingName)
                .SingleOrDefault()
                .Value;

        }
        void setSetting(string value)
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
    public class photoLibraryPath() : Setting("libraryPath") { }
    public class outputPath() : Setting("outputPath") { }

}
