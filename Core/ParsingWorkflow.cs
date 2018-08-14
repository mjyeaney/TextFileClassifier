using System;
using System.Threading.Tasks;
using TextFileClassifier.Core;

namespace TextFileClassifier.Core
{
    public class ParsingWorkflow
    {
        private IJobStatusStore statusStore;
        private IBlobReader blobReader;
        private IContentExtractor extractor;
        private ISearchIndex searchIndex;
        private IDocumentScorer docScorer;

        public ParsingWorkflow(IJobStatusStore statusStore, 
                                IBlobReader blobReader, 
                                IContentExtractor extractor, 
                                ISearchIndex searchIndex,
                                IDocumentScorer documentScorer)
        {
            this.statusStore = statusStore;
            this.blobReader = blobReader;
            this.extractor = extractor;
            this.searchIndex = searchIndex;
            this.docScorer = documentScorer;
        }

        public async Task ExecuteAsync(BlobDetails blobInfo)
        {
            // Read status info
            var job = await statusStore.ReadStatusAsync(blobInfo.JobId);

            // Retreive blob
            job.Message = "Reading file data...";
            job.JobStartTime = DateTime.UtcNow;
            await statusStore.UpdateStatusAsync(job);
            var fileData = await blobReader.ReadFileDataAsync(blobInfo.FullBlobPath);

            // Extract contents
            job.Message = "Extracting contents...";
            await statusStore.UpdateStatusAsync(job);
            var content = extractor.ExtractTextContents(fileData);

            // Push into search indexer
            job.Message = "Pushing contents into search index...";
            await statusStore.UpdateStatusAsync(job);
            await searchIndex.IndexTextContentAsync(blobInfo.DocumentId, content);

            // Ensure the document is availbale before attempting to run scoring
            var doesExist = await searchIndex.VerifyDocumentAvailable(blobInfo.DocumentId, 5);
            if (doesExist)
            {
                // Score document
                job.Message = "Scoring document...";
                await statusStore.UpdateStatusAsync(job);
                var categories = await docScorer.CategorizeDocument(blobInfo.DocumentId);

                // Mark workflow complete
                job.Categories = categories;
                job.IsComplete = true;
                job.JobEndTime = DateTime.UtcNow;
                job.Message = "Workflow complete!!!";
                await statusStore.UpdateStatusAsync(job);
            }
        }
    }
}