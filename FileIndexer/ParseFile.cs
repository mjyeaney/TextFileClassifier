using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TextFileClassifier.Core;
using TextFileClassifier.Core.Services;

namespace TextFileClassifier.FileIndexer
{
    public static class ParseFile
    {
        static Lazy<ParsingWorkflow> workflowClient = new Lazy<ParsingWorkflow>(() => 
        {
            var jobStatusStore = new JobStatusStore(Configuration.StorageConnectionString, Configuration.JobStatusContainerName);
            var blobReader = new AzureBlobReader(Configuration.StorageConnectionString, Configuration.FileUploadContainerName);
            var extractor = new TextFileContentExtractor();
            var searchIndex = new AzureSearchIndex(Configuration.SearchServiceName, Configuration.SearchAdminKey);
            var documentScorer = new TextDocumentScorer(searchIndex);
            return new ParsingWorkflow(jobStatusStore, blobReader, extractor, searchIndex, documentScorer);
        });

        [FunctionName("ParseFile")]
        public static void Run([EventGridTrigger] EventGridEvent eventData, TraceWriter log)
        {
            try 
            {
                var data = (JObject)eventData.Data;
                var storageDetails = JsonConvert.DeserializeObject<StorageBlobCreatedEventData>(data.ToString());
                log.Info($"Blob created at the following URL: {storageDetails.Url}");

                var parsedUrl = BlobUrlParser.Parse(storageDetails.Url);
                if (parsedUrl.ContainerName == Configuration.FileUploadContainerName)
                {
                    workflowClient.Value.ExecuteAsync(parsedUrl).Wait();
                }
            }
            catch (Exception err)
            {
                log.Error(err.ToString());
            }
        }
    }
}
