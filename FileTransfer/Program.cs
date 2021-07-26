using System;
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
            //Main Menu for entering data into the program.
            Console.WriteLine("Welcome to File Transfer App\nThis program will move the contents of two sources into a single destination");
            Console.WriteLine(@"Please Enter the absolute path of your first source. C:\foldername\");
            string source01 = Console.ReadLine();
            Console.WriteLine("Enter the absolute path of your second source");
            string source02 = Console.ReadLine();
            Console.WriteLine("Enter the absolute path of your destination");
            string dest = Console.ReadLine();
            if (dest[dest.Length-1] != '\\')
            {
                dest += "\\";
            }
            var decision = new System.ConsoleKeyInfo();

            //Verifies that the provided directories exist before allowing to continue.
            if (Directory.Exists(source01) && Directory.Exists(source02) && Directory.Exists(dest))
            {
                Console.WriteLine($"Your files and directories from {source01} and {source02} will be moved to {dest}\nPress C to continue or Q to abort");
                decision = Console.ReadKey();
            }
            else
            {
                Console.WriteLine("\nOne or more directories do not exist. Aborting Program.");
                System.Environment.Exit(0);
            }

            //User decides to attempt file transfer
            if (decision.KeyChar == 'c')
            {
                Console.WriteLine("\nVerifying information provided");

                if (source01.Length > 3 || source02.Length > 3)
                {
                    var enumerationOption = new EnumerationOptions();
                    enumerationOption.RecurseSubdirectories = true;
                    enumerationOption.ReturnSpecialDirectories = false;
                    var src01FileList = Directory.GetFiles(source01, "*.*", enumerationOption);
                    var src02FileList = Directory.GetFiles(source02, "*.*", enumerationOption);
                    var sizeOfDataTransfer = 0.00;

                    foreach (var file in src01FileList)
                    {
                        sizeOfDataTransfer += new FileInfo(file).Length / 1e+9;
                    }

                    foreach (var file in src02FileList)
                    {
                        sizeOfDataTransfer += new FileInfo(file).Length / 1e+9;
                    }

                    if (sizeOfDataTransfer >= new DriveInfo(dest.Substring(0, 3)).AvailableFreeSpace / 1e+9)
                    {
                        Console.WriteLine("\nNot enough free space available. Aborting operation.");
                        System.Environment.Exit(0);
                    }
                }
                else
                {
                    List<DriveInfo> listOfDrives = DriveInfo.GetDrives().ToList();
                    var srcDrive01 = listOfDrives.Where(x => x.Name == source01).ToList()[0];
                    var srcDrive02 = listOfDrives.Where(x => x.Name == source02).ToList()[0];
                    var destDrive = listOfDrives.Where(x => x.Name == dest.Substring(0,3)).ToList()[0];
                    var sizeOfDataTransferGB = ((srcDrive01.TotalSize - srcDrive01.TotalFreeSpace) + (srcDrive02.TotalSize - srcDrive02.TotalFreeSpace)) / 1e+9;
                    var sizeOfSpaceAvailableGB = destDrive.AvailableFreeSpace / 1e+9;

                    if (sizeOfDataTransferGB >= sizeOfSpaceAvailableGB)
                    {
                        Console.WriteLine("\nNot enough free space available. Aborting operation.");
                        System.Environment.Exit(0);
                    }
                }
                    
                    Computer myPC = new Computer();

                //both sources are Root
                if (source01.Length == 3 && source02.Length == 3)
                {
                    Console.WriteLine("\nFile transfer beginning...");
                    //Transfer FILES to DEST. If file already exists in the destination. Then transfered file is appended with date time stamp down to milliseconds to guarantee uniqueness.
                    FileTransfer(source01, dest);
                    FileTransfer(source02, dest);

                    //Transfer DIRECTORIES to DEST. This is using VB
                    DirectoryTransfer(source01, dest, myPC);
                    DirectoryTransfer(source02, dest, myPC);

                    Console.WriteLine("\nTransfer Complete...");
                }
                //both sources are directories
                else if (source01.Length > 3 && source02.Length > 3)
                {
                    Console.WriteLine("\nFile transfer beginning...");

                    //Transfer DIRECTORIES to DEST. This is using VB
                    SingleDirectoryTransfer(source01, dest, myPC);
                    SingleDirectoryTransfer(source02, dest, myPC);

                    Console.WriteLine("\nTransfer Complete...");
                }
                //source 1 is a root and source 2 is a directory
                else if (source01.Length == 3 && source02.Length > 3)
                {
                    Console.WriteLine("\nFile transfer beginning...");
                    FileTransfer(source01, dest);
                    DirectoryTransfer(source01, dest, myPC);
                    SingleDirectoryTransfer(source02, dest, myPC);
                    Console.WriteLine("\nTransfer Complete...");
                }
                //source 1 is directory and source 2 is root
                else if (source01.Length > 3 && source02.Length == 3)
                {
                    Console.WriteLine("\nFile transfer beginning...");
                    FileTransfer(source02, dest);
                    SingleDirectoryTransfer(source01, dest, myPC);
                    DirectoryTransfer(source02, dest, myPC);
                    Console.WriteLine("\nTransfer Complete...");
                }
                //Console.WriteLine("\nFile transfer beginning...");
                //        //Transfer FILES to DEST. If file already exists in the destination. Then transfered file is appended with date time stamp down to milliseconds to guarantee uniqueness.
                //        FileTransfer(source01, dest);
                //        FileTransfer(source02, dest);

                //        //Transfer DIRECTORIES to DEST. This is using VB
                //        DirectoryTransfer(source01, dest, myPC);
                //        DirectoryTransfer(source02, dest, myPC);

                //        Console.WriteLine("\nTransfer Complete...");

            }
            //User decides to abort file transfer
            else
            {
                Console.WriteLine("\nAborting operation");
                System.Environment.Exit(0);
            }


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

        public static void SingleDirectoryTransfer(string source, string destination, Computer myPC)
        {

                try
                {
                    myPC.FileSystem.MoveDirectory(source, destination + source.Substring(source.LastIndexOf('\\') + 1), true);
                }
                catch (System.IO.IOException)
                {
                    Directory.Delete(destination + source.Substring(source.LastIndexOf('\\') + 1), true);
                    myPC.FileSystem.MoveDirectory(source, destination.Substring(0, destination.IndexOf('\\') + 1) + source.Substring(source.LastIndexOf('\\') + 1), true);
                }
            }
        }



    }

