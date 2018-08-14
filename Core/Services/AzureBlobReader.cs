using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace TextFileClassifier.Core.Services
{
    public class AzureBlobReader : IBlobReader
    {
        private CloudStorageAccount cloudStorageAccount;
        private CloudBlobClient blobClient;
        private CloudBlobContainer blobContainer;

        public AzureBlobReader(string storageConnectionInfo, string storageContainerName)
        {
            cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionInfo);
            blobClient = cloudStorageAccount.CreateCloudBlobClient();
            blobContainer = blobClient.GetContainerReference(storageContainerName);
        }

        public async Task<byte[]> ReadFileDataAsync(string blobId)
        {
            await blobContainer.CreateIfNotExistsAsync();
            var blob = blobContainer.GetBlobReference(blobId);
            var dataStream = new MemoryStream();
            await blob.DownloadToStreamAsync(dataStream);
            dataStream.Position = 0;
            return dataStream.ToArray();
        }
    }
}