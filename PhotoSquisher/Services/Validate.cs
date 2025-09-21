using PhotoSquisher.Models;
using Ignore;

namespace PhotoSquisher.Services
{
    internal static class Validate
    {
        //TODO generalise this for any table, column, testValue type (not super easy, how would you pass a generic dbset<whattype??> arguement)
        public static bool pathIsUnique(PhotoSquisherDbContext db, string testValue)
        {
            var firstLocalDuplicateQuery =   (
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
        public static bool PathIsNotIgnored(PhotoSquisherDbContext db, string testValue) 
        {
        //https://github.com/goelhardik/ignore

            Ignore.Ignore Ignores = new();
            Ignores.Add(from i in db.IgnorePatterns select i.ignorePattern);
            bool pathIsNotIgnored = Ignores.IsIgnored(testValue) ? false : true;
            if (!pathIsNotIgnored) Console.WriteLine(testValue);
            return pathIsNotIgnored;
            
        }
    }
}
