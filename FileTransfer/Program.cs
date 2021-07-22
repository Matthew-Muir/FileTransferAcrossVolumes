﻿using System;
using System.IO;
using Microsoft.VisualBasic.Devices;
using System.Security.Permissions;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using System.Linq;

namespace FileTransfer
{
    class Program
    {
        static void Main(string[] args)
        {
            Computer myPC = new Computer();
            string source01 = @"X:\";
            string source02 = @"Y:\";
            string dest = @"Z:\long - Copy\aaaaaaaaaaaaaaaaaaaa\bbbbbbbbbbbbbbbbbbbb\cccccccccccccccccccc\dddddddddddddddddddd\eeeeeeeeeeeeeeeeeeee\ffffffffffffffffffff\gggggggggggggggggggg\hhhhhhhhhhhhhhhhhhhh\iiiiiiiiiiiiiiiiiiii\jjjjjjjjjjjjjjjjjjjj\kkkkkkkkkkkkkkkkkkkk\";



            //Transfer FILES to DEST. If file already exists in the destination. Then transfered file is appended with date time stamp down to milliseconds to guarntee uniqueness.
            FileTransfer(source01, dest);
            FileTransfer(source02, dest);

            //Transfer DIRECTORIES to DEST. This is using VB
            DirectoryTransfer(source01, dest, myPC);
            DirectoryTransfer(source02, dest, myPC);

        }
        public static void FileTransfer(string source, string destination)
        {
            var filesInSource = Directory.EnumerateFiles(source);


            foreach (var item in filesInSource)
            {
                try
                {
                    System.IO.File.Move(item, destination + item.Substring(item.LastIndexOf('\\') + 1), false);
                }
                catch (System.IO.IOException)
                {
                    //catch file already exists. Most up to date file will be the file that ends up in the dest.
                    var filename = item.Substring(item.LastIndexOf('\\'));
                    var sourceFile = new FileInfo(item);
                    var destFile = new FileInfo(destination + filename);

                    if (destFile.LastWriteTimeUtc < sourceFile.LastWriteTimeUtc)
                    {
                        System.IO.File.Move(item, destination + filename, true);
                    }
                    else
                    {
                        System.IO.File.Delete(item);
                    }
                }
            }
        }

        public static void DirectoryTransfer(string source, string destination, Computer myPC)
        {
            List<string> listOfDirectories = Directory.GetDirectories(source).ToList();
            listOfDirectories.Remove(listOfDirectories.Find(x => x.Contains("$RECYCLE.BIN")));
            listOfDirectories.Remove(listOfDirectories.Find(x => x.Contains("System Volume Information")));

            foreach (var item in listOfDirectories)
            {
                try
                {
                    myPC.FileSystem.MoveDirectory(item, destination + item.Substring(item.LastIndexOf('\\') + 1), true);
                }
                catch (System.IO.IOException)
                {
                    Directory.Delete(destination + item.Substring(item.LastIndexOf('\\') + 1), true);
                    myPC.FileSystem.MoveDirectory(item, destination.Substring(0, destination.IndexOf('\\') + 1) + item.Substring(item.LastIndexOf('\\') + 1), true);
                }
            }
        }
    }
}
