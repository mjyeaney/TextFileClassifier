using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using TextFileClassifier.Core;
using TextFileClassifier.Core.Services;

namespace TextFileClassifier.BulkUploader
{
    public class UploadTask : IConsoleTask
    {
        private IJobStatusStore _jobStore;
        private string _srcDir;
        private int _maxFiles;
        private string _jobHistoryLocation;
        private CloudBlobContainer _uploadContainer;

        public UploadTask(IJobStatusStore jobStatusStore,
                            string sourceDirectory, 
                            int maxFiles, 
                            string jobHistoryFileLocation)
        {
            _jobStore = jobStatusStore;
            _srcDir = sourceDirectory;
            _maxFiles = maxFiles;
            _jobHistoryLocation = jobHistoryFileLocation;

            var storageAccount = CloudStorageAccount.Parse(Configuration.StorageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            _uploadContainer = blobClient.GetContainerReference(Configuration.FileUploadContainerName);
            _uploadContainer.CreateIfNotExistsAsync().Wait();
        }

        public void Execute(Action<string> logCallback)
        {
            var jobIdentifiers = new List<string>();
            var fileList = Directory.GetFiles(_srcDir).Take(_maxFiles);

            logCallback($"Read {fileList.Count()} files...");

            foreach (var file in fileList)
            {
                var thisJob = new JobStatus();
                thisJob.JobId = Guid.NewGuid().ToString().Replace("-", String.Empty);
                thisJob.JobStartTime = DateTime.UtcNow;
                thisJob.OriginalFileName = Path.GetFileName(file);
                _jobStore.UpdateStatusAsync(thisJob).Wait();
                jobIdentifiers.Add(thisJob.JobId);

                var fileName = Guid.NewGuid().ToString().Replace("-", String.Empty);
                var uploadName = String.Format("{0}/{1}", thisJob.JobId, fileName);

                logCallback($"Original filename {file}...");
                logCallback($"New filename {fileName}...");
                logCallback($"Upload name {uploadName}...");

                using (var fileData = File.OpenRead(file))
                {
                    // Save to blob storage
                    _uploadContainer
                        .GetBlockBlobReference(uploadName)
                        .UploadFromStreamAsync(fileData)
                        .Wait();
                }
            }

            logCallback($"Writing job history to {_jobHistoryLocation}");

            using (var historyFile = File.CreateText(_jobHistoryLocation))
            {       
                historyFile.WriteAsync(JsonConvert.SerializeObject(jobIdentifiers)).Wait();
            }
        }
    }
}