using System;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using TextFileClassifier.Core;
using TextFileClassifier.Core.Services;

namespace Tests
{
    public class AzureBlobReaderTests
    {
        const string BLOB_TEST_CONTAINER_NAME = "tests";
        const string TEST_BLOB_CONTENTS = "This is a test blob";

        [Fact]
        public async void CanReadBlobContents()
        {
            var sampleBlobName = Guid.NewGuid().ToString().Replace("-", String.Empty);
            var blob = (CloudBlockBlob)null;

            try 
            {
                var account = CloudStorageAccount.Parse(Configuration.StorageConnectionString);
                var client = account.CreateCloudBlobClient();
                var container = client.GetContainerReference(BLOB_TEST_CONTAINER_NAME);
                await container.CreateIfNotExistsAsync();

                // Create sample blob
                blob = container.GetBlockBlobReference(sampleBlobName);
                await blob.UploadTextAsync(TEST_BLOB_CONTENTS);

                // Read blob using BlobReader
                var blobReader = new AzureBlobReader(Configuration.StorageConnectionString, BLOB_TEST_CONTAINER_NAME);
                var blobData = await blobReader.ReadFileDataAsync(sampleBlobName);
                Assert.Equal(TEST_BLOB_CONTENTS, Encoding.UTF8.GetString(blobData));
            } 
            finally 
            {
                // Cleanup from test
                if (blob != null)
                {
                    await blob.DeleteAsync();
                }
            }
        }   
    }
}
