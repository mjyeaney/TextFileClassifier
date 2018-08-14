using System;

namespace TextFileClassifier.Core
{
    public class BlobDetails
    {
        public string ContainerName  { get; set; }

        public string FullBlobPath { get; set; }
        
        public string JobId { get; set; }

        public string DocumentId { get; set; }
    }
}