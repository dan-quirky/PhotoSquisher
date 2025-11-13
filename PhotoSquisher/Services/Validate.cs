using PhotoSquisher.Models;
using Ignore;
using System.Text.RegularExpressions;

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
        public static bool PathIsNotIgnored(PhotoSquisherDbContext db, string testValue) //defunct
        {
            //https://github.com/goelhardik/ignore

            Ignore.Ignore Ignores = new();
            IEnumerable<string> ignorePatterns = db.IgnorePatterns.Select(i => i.ignorePattern);
            Ignores.Add(ignorePatterns);
            bool pathIsNotIgnored = Ignores.IsIgnored(testValue) ? false : true;
            if (!pathIsNotIgnored) Console.WriteLine("ignored" + testValue);
            Console.WriteLine("Ignore Patterns: ");
            foreach (var ip in ignorePatterns) Console.WriteLine(ip);
            Console.WriteLine("didn't ignore" + testValue);
            return pathIsNotIgnored;

        }
        public static bool PathIsIgnored(PhotoSquisherDbContext db, string testValue)
        {

            IEnumerable<string> ignorePatterns = db.IgnorePatterns.Select(i => i.ignorePattern);
            foreach (string ip in ignorePatterns)
                if (Regex.IsMatch(testValue, ip))
                {
                    Console.WriteLine("ignored" + testValue);
                    return true;
                }
            //Console.WriteLine("Ignore Patterns: ");
            //foreach (var ip in ignorePatterns) Console.WriteLine(ip);
            //Console.WriteLine("didn't ignore" + testValue);
            return false;

        }
    }
}
