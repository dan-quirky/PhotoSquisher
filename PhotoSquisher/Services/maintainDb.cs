﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoSquisher.Models;

namespace PhotoSquisher.Services
{
    public static class maintainDb
    {
        public async static Task<bool> CreateDatabaseAsync()
        {
            using var context = new PhotoSquisherDbContext(); //using ensures the dbcontext gets disposed of properly, not sure how tho
            return await context.Database.EnsureCreatedAsync();
        }
        public async static Task<bool> DeleteDatabaseAsync()
        {
            try
            {
                using var context = new PhotoSquisherDbContext();
                return await context.Database.EnsureDeletedAsync();
            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine($"Something went wrong: {ex.Message}");
                return false;
            }
        }
        public async static void RebuildDatabase()
        {
            //BUG have observed this not deleting the db without complainign about it, look into it 
            //Todo This should only continue if the previous step didn't fail (i.e. delete fails if db is open in viewer). Sounds like a Task thing.
            try
            {
                Console.WriteLine($"Rebuilding db...");
                Console.WriteLine(Environment.NewLine
                    + "Deleting old db...");
                if (await DeleteDatabaseAsync() != true) throw new InvalidOperationException("Failed to delete database");
                Console.WriteLine("Done."
                    + Environment.NewLine
                    + "Creating new db");
                if (await CreateDatabaseAsync() != true) throw new InvalidOperationException("Failed to create database");
                Console.WriteLine("Done.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Something went wrong: {ex.Message}");
            }
        }


    }
}
