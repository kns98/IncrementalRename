using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0 || args.Length > 2)
        {
            Console.WriteLine("Usage: [directoryPath] <dryrun>");
            return;
        }

        string directoryPath = args[0];
        
        // Check for the valid dryrun argument
        if (args.Length == 2 && !(args[1].ToLower() == "yes" || args[1].ToLower() == "no"))
        {
            Console.WriteLine("Invalid argument for dryrun. Use 'yes' or 'no'.");
            return;
        }

        bool isDryRun = args.Length > 1 && args[1].ToLower() == "yes";

        List<FileInfo> files1 = new List<FileInfo>();
        DirSearchAndRename(directoryPath, files1, Guid.NewGuid().ToString(), isDryRun);

        List<FileInfo> files2 = new List<FileInfo>();
        DirSearchAndRename(directoryPath, files2, "", isDryRun);
    }

    static void DirSearchAndRename(string directoryPath, List<FileInfo> files, string prefix, bool isDryRun)
    {
        try
        {
            foreach (string filePath in Directory.GetFiles(directoryPath))
            {
                files.Add(new FileInfo(filePath));
            }

            foreach (string dirPath in Directory.GetDirectories(directoryPath))
            {
                DirSearchAndRename(dirPath, files, prefix, isDryRun);
            }

            RunPass(files, prefix, isDryRun);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    static void RunPass(List<FileInfo> files, string prefix, bool isDryRun)
    {
        for (int i = 0; i < files.Count; i++)
        {
            FileInfo file = files[i];
            string dirname = Path.GetDirectoryName(file.FullName);
            string destFileName = prefix + i + file.Extension;
            string destFilePath = Path.Combine(dirname, destFileName);

            if (isDryRun)
            {
                Console.WriteLine($"[DRY RUN] Would rename: {file.FullName} to {destFilePath}");
            }
            else
            {
                try
                {
                    File.Move(file.FullName, destFilePath);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        files.Clear();
    }
}
