using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PhotoSquisher.Models;

namespace PhotoSquisher.Services
{
    internal static class Validate
    {
        //TODO generalise this for any table, column, testValue type (not super easy, how would you pass a generic dbset<whattype??> arguement)
        public static bool pathIsUnique(PhotoSquisherDbContext db, string testValue)
        {
            var firstLocalDuplicateQuery = (
            from p in db.Photos.Local //.Local to check uncommitted changes for dupes
            where p.Path == testValue
            select p.Path
            ).FirstOrDefault(); //Return first row where path matches the test value, or null if no matches

            var firstDuplicateQuery = (
            from p in db.Photos
            where p.Path == testValue
            select p.Path
            ).FirstOrDefault(); //Return first row where path matches the test value, or null if no matches

            return firstDuplicateQuery is null && firstLocalDuplicateQuery is null;
        }
    }
}
