using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Renamer
{
    internal class Program
    {
        private const string TemplateName = "Bitsie.Shop";

        // Do not touch these directories during rename
        private static List<string> ExcludedDirectories
        {
            get
            {
                return new List<string>
                {
                    ".git",
                    "bin",
                    "obj",
                    "Libs",
                    "BuildSystem"
                };
            }
        }

        // Do not touch these extensions during rename
        private static List<string> ExcludedExtensions
        {
            get
            {
                return new List<string>
                {
                    ".exe",
                    ".suo",
                    ".dll",
                    ".pdb"
                };
            }
        }

        static void Main(string[] args)
        {
            Console.Write(@"All files, text and assembly references within this " +
                          @"solution will be renamed to the project name you enter below. " +
                          @"I.e. entering ""Foo"" will rename projects to Foo.Web, Foo.Domain, etc.).");
            Console.Write("\n\n");
            string projectName = "";

            Regex fileNameReg = new Regex(@"^[A-Za-z\.]+$");
            while (String.IsNullOrEmpty(projectName))
            {
                Console.Write("New Project Name: ");
                string tempName = Console.ReadLine();
                if (fileNameReg.IsMatch(tempName))
                {
                    projectName = tempName;
                } else
                {
                    Console.Write("Invalid project name. Must be one word with no numbers or special characters.\n\n");
                }
            }

            Console.WriteLine("Please wait, scanning files and directories...");
            var root = new DirectoryInfo(@"../");

            Rename(root, projectName);

            Console.WriteLine("Rename complete. Press any key to exit...");
            Console.ReadKey();
        }

        static void Rename(DirectoryInfo root, string projectName)
        {
            var directories = root.GetDirectories();
            RenameFiles(root, projectName);
            foreach (DirectoryInfo dir in directories)
            {
                // Recurse into sub-directories if not excluded
                if (!ExcludedDirectories.Contains(dir.Name))
                {
                    Rename(dir, projectName);
                }
            }
            RenameFolders(root, projectName);
        }

        static void RenameFolders(DirectoryInfo root, string projectName)
        {
            var directories = root.GetDirectories();
            foreach (DirectoryInfo dir in directories)
            {
                // Rename if matches and is not exluced
                if (dir.Name.Contains(TemplateName) && !ExcludedDirectories.Contains(dir.Name))
                {
                    var path = dir.Parent.FullName;
                    var newName = Path.Combine(path, dir.Name.Replace(TemplateName, projectName));
                    Console.WriteLine("Renaming " + newName);
                    try
                    {
                        Directory.Move(dir.FullName, newName);
                    } catch(Exception ex)
                    {
                        Console.WriteLine("FAILED: Could not rename " + dir.FullName + ". "
                            + ex.Message);
                    }
                }
            }
        }

        static void RenameFiles(DirectoryInfo root, string projectName)
        {
            var files = root.GetFiles("*.*");
            foreach (var file in files)
            {
                // Skip if excluded extension
                if (ExcludedExtensions.Contains(file.Extension)) continue;

                // Rename contents
                string content = "";
                
                try
                {
                    content = File.ReadAllText(file.FullName);
                    content = content.Replace(TemplateName, projectName);
                    File.WriteAllText(file.FullName, content);
                } catch(Exception ex)
                {
                    Console.WriteLine("FAILED: Unable to read file: " + file.FullName + ". "
                        + ex.Message);
                    continue;
                }

                // Rename File
                if (file.Name.Contains(TemplateName))
                {
                    var path = Path.GetDirectoryName(file.FullName);
                    var newName = Path.Combine(path, file.Name.Replace(TemplateName, projectName));
                    Console.WriteLine("Renaming " + newName);
                    try
                    {
                        File.Move(file.FullName, newName);
                    } catch(Exception ex)
                    {
                        Console.WriteLine("FAILED: Could not rename " + file.FullName + "." 
                            + ex.Message);
                    }
                }
            }
        }
    }
}
