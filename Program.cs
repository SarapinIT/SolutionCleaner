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
        }

        private static void NotIncludedProjCheck(string slnDirPath, string slnFilePath)
        {
            var solution = SolutionFile.Parse(slnFilePath);
            var csprojIncludedToSolution = solution.ProjectsInOrder.Select(o => o.AbsolutePath);
            var allCsprojInFolder = Directory.GetFiles(slnDirPath, "*.csproj", SearchOption.AllDirectories);

            var notIncludedFiles = allCsprojInFolder.Except(csprojIncludedToSolution).ToArray();

            if (notIncludedFiles.Length == 0)
            {
                Console.WriteLine("nothing to clean");
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