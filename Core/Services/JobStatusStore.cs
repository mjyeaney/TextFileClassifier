using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace TextFileClassifier.Core.Services
{
    public class JobStatusStore : IJobStatusStore
    {
        private CloudStorageAccount cloudStorageAccount;
        private CloudBlobClient blobClient;
        private CloudBlobContainer blobContainer;
        
        public JobStatusStore(string storageConnectionInfo, string storageContainerName)
        {
            cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionInfo);
            blobClient = cloudStorageAccount.CreateCloudBlobClient();
            blobContainer = blobClient.GetContainerReference(storageContainerName);
        }

        public async Task UpdateStatusAsync(JobStatus newStatus)
        {
            await writePayloadToStorage(newStatus.JobId, newStatus);
        }

        public async Task<JobStatus> ReadStatusAsync(string JobId)
        {
            await blobContainer.CreateIfNotExistsAsync();
            var blob = blobContainer.GetBlobReference(JobId);
            var stream = new MemoryStream();
            await blob.DownloadToStreamAsync(stream);
            return JsonConvert.DeserializeObject<JobStatus>(Encoding.UTF8.GetString(stream.ToArray()));
        }

        public async Task<IEnumerable<JobStatus>> ReadAllStatusAsync()
        {
            await blobContainer.CreateIfNotExistsAsync();

            // NOTE: No continuation token, so only reading a single page (max 100 results).
            var list = await blobContainer.ListBlobsSegmentedAsync(null, false, BlobListingDetails.Metadata, 100, null, null, null);
            var results = new List<JobStatus>();

            foreach (var item in list.Results)
            {
                var status = new JobStatus();
                var blob = ((CloudBlob)item);
                status.JobId = blob.Name;
                status.OriginalFileName = blob.Metadata["OriginalFileName"];
                status.JobStartTime = DateTime.Parse(blob.Metadata["JobStartTime"], null, DateTimeStyles.RoundtripKind);
                status.JobEndTime = DateTime.Parse(blob.Metadata["JobEndTime"], null, DateTimeStyles.RoundtripKind);
                status.IsComplete = bool.Parse(blob.Metadata["IsComplete"]);
                
                if (blob.Metadata.ContainsKey("Message"))
                {
                    status.Message = blob.Metadata["Message"];
                }

                if (blob.Metadata.ContainsKey("Categories"))
                {
                    status.Categories = blob.Metadata["Categories"].Split(',');
                }

                results.Add(status);
            }

            return results;
        }

        private async Task writePayloadToStorage(string jobId, JobStatus status)
        {
            await blobContainer.CreateIfNotExistsAsync();
            var blob = blobContainer.GetBlockBlobReference(jobId);

            var payload = JsonConvert.SerializeObject(status);
            await blob.UploadTextAsync(payload);
            
            blob.Metadata["OriginalFileName"] = status.OriginalFileName;
            blob.Metadata["JobStartTime"] = status.JobStartTime.ToString("o");
            blob.Metadata["JobEndTime"] = status.JobEndTime.ToString("o");
            blob.Metadata["IsComplete"] = status.IsComplete.ToString(CultureInfo.InvariantCulture);

            if (!String.IsNullOrEmpty(status.Message))
            {
                blob.Metadata["Message"] = status.Message;
            }

            if (status.Categories != null)
            {
                blob.Metadata["Categories"] = String.Join(",", status.Categories);
            }
            
            await blob.SetMetadataAsync();
        }
    }
}