using System;
using TextFileClassifier.Core;

namespace TextFileClassifier.Core
{
    public class BlobUrlParser
    {
        public static BlobDetails Parse(string blobUrl)
        {
            // Implement and verify tests pass
            var uri = new Uri(blobUrl);
            var containerName = uri.Segments[1].Replace("/", String.Empty);
            var jobId = String.Empty;
            var documentId = String.Empty;
            var fullBlobPath = String.Empty;

            if (uri.Segments.Length == 4)
            {
                jobId = uri.Segments[2].Replace("/", String.Empty);
                documentId = uri.Segments[3];
                fullBlobPath = String.Format("{0}/{1}", jobId, documentId);
            }
            else
            {
                fullBlobPath = uri.Segments[2];
            }
            
            return new BlobDetails()
            {
                ContainerName = containerName,
                JobId = jobId,
                DocumentId = documentId,
                FullBlobPath = fullBlobPath
            };
        }
    }
}