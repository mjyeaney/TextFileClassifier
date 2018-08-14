using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.WindowsAzure.Storage;
using TextFileClassifier.Core;
using TextFileClassifier.Core.Services;

namespace Tests
{
    public class FileIndexerIntegrationTests
    {
        const string TEST_BLOB_CONTENTS = "This should match the last item, forcing all queries to run: Ottinger.";

        [Fact]
        public async Task ExecutesFullIntegrationPass()
        {
            var jobStatusStore = new JobStatusStore(Configuration.StorageConnectionString, Configuration.JobStatusContainerName);
            var blobReader = new AzureBlobReader(Configuration.StorageConnectionString, Configuration.FileUploadContainerName);
            var extractor = new TextFileContentExtractor();
            var searchIndex = new AzureSearchIndex(Configuration.SearchServiceName, Configuration.SearchAdminKey);
            var docScorer = new TextDocumentScorer(searchIndex);
            var workflow = new ParsingWorkflow(jobStatusStore, blobReader, extractor, searchIndex, docScorer);

            var blobId = Guid.NewGuid().ToString().Replace("-", String.Empty);
            var jobId = Guid.NewGuid().ToString().Replace("-", String.Empty);
            var blobUri = String.Format("{0}/{1}", jobId, blobId);
            
            var blobDetails = new BlobDetails();
            blobDetails.ContainerName = Configuration.FileUploadContainerName;
            blobDetails.FullBlobPath = blobUri;
            blobDetails.DocumentId = blobId;
            blobDetails.JobId = jobId;

            var job = new JobStatus();
            job.OriginalFileName = "not-real-file.txt";
            job.IsComplete = false;
            job.JobStartTime = DateTime.UtcNow;
            job.JobId = jobId;
            
            await jobStatusStore.UpdateStatusAsync(job);
            await createSampleBlob(blobUri);
            await workflow.ExecuteAsync(blobDetails);

            job = await jobStatusStore.ReadStatusAsync(jobId);
            var categoryCount = job.Categories.Length;
            Assert.Equal(1, categoryCount);
            Assert.Equal("Heavy Hitter", job.Categories[0]);
        }

        private async Task createSampleBlob(string name)
        {
            var account = CloudStorageAccount.Parse(Configuration.StorageConnectionString);
            var client = account.CreateCloudBlobClient();
            var container = client.GetContainerReference(Configuration.FileUploadContainerName);
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlockBlobReference(name);
            await blob.UploadTextAsync(TEST_BLOB_CONTENTS);
        }
    }
}