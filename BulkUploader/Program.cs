using System;
using TextFileClassifier.Core;
using TextFileClassifier.Core.Services;

namespace TextFileClassifier.BulkUploader
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Not enough arguments - missing \"mode\" flag for \"upload\" or \"report\".");
                Console.WriteLine();
                return;
            }
            
            // Determine which mode we are in (1 or 2):
            var mode = args[0].Split(":")[1].ToUpperInvariant();
            Console.WriteLine($"Current mode: {mode}");

            // Our console task
            var task = (IConsoleTask)null;

            // 1. Upload Pass
            if (mode == "UPLOAD")
            {
                if (args.Length != 4)
                {
                    Console.WriteLine("Not enough arguments - missing \"sourceDir\", \"maxFiles\", and \"historyFile\" parameters.");
                    Console.WriteLine();
                    Console.WriteLine("Usage: BulkUploader.exe /mode:upload {source directory} {maximum number of files} {location to save job output}");
                    Console.WriteLine();
                    return;
                }
                var sourceDir = args[1];
                var maxFiles = Int32.Parse(args[2]);
                var historyFileLocation = args[3];

                task = new UploadTask(new JobStatusStore(Configuration.StorageConnectionString, Configuration.JobStatusContainerName), 
                                    sourceDir, 
                                    maxFiles, 
                                    historyFileLocation);
            }

            if (mode == "REPORT")
            {
                if (args.Length != 3)
                {
                    Console.WriteLine("Not enough arguments - missing \"jobLogLocation\" and \"reportLocation\" parameters.");
                    Console.WriteLine();
                    Console.WriteLine("Usage: BulkUploader.exe /mode:report {jobLogLocation} {reportLocation}");
                    Console.WriteLine();
                    return;
                }
                var jobLogLocation = args[1];
                var reportLocation = args[2];

                task = new ReportTask(new JobStatusStore(Configuration.StorageConnectionString, Configuration.JobStatusContainerName), 
                                    jobLogLocation, 
                                    reportLocation);
            }

            if (task != null)
            {
                task.Execute((message) =>
                {
                    Console.WriteLine($"LOG {DateTime.Now.ToString("s")}: {message}");
                });
            }
        }
    }
}
