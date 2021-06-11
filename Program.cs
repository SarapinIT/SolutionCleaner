using System;
using System.IO;
using System.Linq;
using Microsoft.Build.Construction;

namespace SolutionCleaner
{
    internal class Program
    {
        #region - Members -
        private static void Main()
        {
            var (slnDirPath, slnFilePath) = GetValidDirToSln();
            NotIncludedProjCheck(slnDirPath, slnFilePath);
            EmptyFoldersCheck(slnDirPath);

            Console.WriteLine("all done");
        }

        private static void EmptyFoldersCheck(string slnDirPath)
        {
            var topLevelDirectories = Directory.GetDirectories(slnDirPath);
            var directoriesWithNoFiles = topLevelDirectories.Where(o => false == DirectoryContainFilesRecursively(o)).ToArray();

            if (directoriesWithNoFiles.Length == 0)
            {
                Console.WriteLine("no directories with no files");
                return;
            }

            Console.WriteLine("directories with no files");
            foreach (var dir in directoriesWithNoFiles)
            {
                Console.WriteLine(dir);
            }

            Console.WriteLine("delete directories with no files?");

            string response;
            do
            {
                response = Console.ReadLine();
            } while (response != "yes" && response != "no");

            if (response == "yes")
            {
                foreach (var dir in directoriesWithNoFiles)
                {
                    Directory.Delete(dir, true);
                }

                Console.WriteLine("done");
            }
        }

        private static bool DirectoryContainFilesRecursively(string path)
        {
            var files = Directory.GetFiles(path);
            if (files.Length > 0)
                return true;
            
            var directories = Directory.GetDirectories(path);

            foreach (var directory in directories)
            {
                if (DirectoryContainFilesRecursively(directory)) return true;
            }

            return false;
        }
        
        private static void NotIncludedProjCheck(string slnDirPath, string slnFilePath)
        {
            var solution = SolutionFile.Parse(slnFilePath);
            var csprojIncludedToSolution = solution.ProjectsInOrder.Select(o => o.AbsolutePath);
            var allCsprojInFolder = Directory.GetFiles(slnDirPath, "*.csproj", SearchOption.AllDirectories);

            var notIncludedFiles = allCsprojInFolder.Except(csprojIncludedToSolution).ToArray();

            if (notIncludedFiles.Length == 0)
            {
                Console.WriteLine("all csproj files included to solution");
                return;
            }

            Console.WriteLine("csproj files not included to solution:\n");
            foreach (var notIncludedFile in notIncludedFiles)
            {
                Console.WriteLine(notIncludedFile);
            }

            Console.WriteLine("\ndelete directories contains csproj files not included to solution?");
            string response;
            do
            {
                response = Console.ReadLine();
            } while (response != "yes" && response != "no");

            if (response == "yes")
            {
                foreach (var file in notIncludedFiles)
                {
                    var fileInfo = new FileInfo(file);
                    // ReSharper disable once PossibleNullReferenceException
                    fileInfo.Directory.Delete(true);
                }

                Console.WriteLine("done");
            }
        }

        private static (string slnDirPath, string slnFilePath) GetValidDirToSln()
        {
            Console.WriteLine("path to sln folder:");

            while (true)
            {
                var input = Console.ReadLine();
                if (!Directory.Exists(input))
                {
                    Console.WriteLine($"directory '{input}' not exists");
                    continue;
                }

                // ReSharper disable once AssignNullToNotNullAttribute
                var slnFilePath = Directory.GetFiles(input, "*.sln").FirstOrDefault();

                if (slnFilePath == null)
                {
                    Console.WriteLine($"directory '{input}' not contain sln file");
                    continue;
                }

                return (input, slnFilePath);
            }
        }
        #endregion
    }
}