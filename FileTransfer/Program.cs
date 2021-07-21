using System;
using System.IO;
using Microsoft.VisualBasic.Devices;
using System.Security.Permissions;

namespace FileTransfer
{
    class Program
    {
        static void Main(string[] args)
        {
            Computer myPC = new Computer();
            string source01 = @"X:\";
            string source02 = @"Y:\";
            string dest = @"Z:\";



            var source01ListOfFileNames = Directory.EnumerateFiles(source01);
            var source02ListOfFileNames = Directory.EnumerateFiles(source02);
            var source01ListOfDirectories = Directory.GetDirectories(source01);
            var source02ListOfDirectories = Directory.GetDirectories(source02);

            //Transfer FILES to DEST
            foreach (var item in source01ListOfFileNames)
            {
                System.IO.File.Move(item, dest + item.Substring(item.LastIndexOf('\\') + 1), true);
            }

            foreach (var item in source02ListOfFileNames)
            {
                System.IO.File.Move(item, dest + item.Substring(item.LastIndexOf('\\') + 1), true);
            }

            //Transfer DIRECTORIES to DEST. This is using VB
            foreach (var item in source01ListOfDirectories)
            {
                //myPC.FileSystem.MoveDirectory(item, dest);
            }

            foreach (var item in source02ListOfDirectories)
            {
                if (!item.Contains("$RECYCLE") && !item.Contains("System Volume Information"))
                {
                    Console.WriteLine($"Location {item} - Dest {dest + item.Substring(item.LastIndexOf('\\') + 1) }");
                    myPC.FileSystem.MoveDirectory(item, dest + item.Substring(item.LastIndexOf('\\') + 1));
                }


            }

            //generate a bunch of files to move around
            //for (int i = 0; i < 5; i++)
            //{
            //    System.IO.File.Create(dirY + $"jsFile{i}.js");
            //    System.IO.File.Create(dirX + $"htmlFile{i}.html");
            //}

            //create a bunch of folders
            //for (int i = 0; i < 10; i++)
            //{
            //    Directory.CreateDirectory(dirX + $"folder{i}");
            //}

            


        }
    }
}
