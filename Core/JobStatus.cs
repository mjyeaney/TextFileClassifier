using System;

namespace TextFileClassifier.Core
{
    public class JobStatus
    {
        public string JobId { get; set; }
        public string OriginalFileName { get; set; }
        public DateTime JobStartTime { get; set; }
        public DateTime JobEndTime { get; set; }
        public bool IsComplete { get; set; }
        public string Message { get; set; }
        public string[] Categories { get; set; }
    }
}