using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using TextFileClassifier.Core;
using TextFileClassifier.Core.Services;

namespace TextFileClassifier.BulkUploader
{
    public class ReportTask : IConsoleTask
    {
        private string _jobLogLocation;
        private IJobStatusStore _jobStore;
        private string _reportLocation;

        public ReportTask(IJobStatusStore jobStatusStore, string jobLogLocation, string reportLocation)
        {
            _jobStore = jobStatusStore;
            _jobLogLocation = jobLogLocation;
            _reportLocation = reportLocation;
        }

        public void Execute(Action<string> logCallback)
        {
            logCallback($"Reading job execution log from {_jobLogLocation}");

            var contents = File.ReadAllText(_jobLogLocation);
            var jobIdentifiers = JsonConvert.DeserializeObject<List<String>>(contents);
            var reportLines = new List<string>();

            foreach (var id in jobIdentifiers)
            {
                logCallback($"Reading job details for job {id}");
                var details = _jobStore.ReadStatusAsync(id).Result;

                if (details.Categories != null)
                {
                    foreach (var category in details.Categories)
                    {
                        reportLines.Add($"\"{details.OriginalFileName}\",\"{category}\"");
                    }
                }
                else
                {
                    reportLines.Add($"\"{details.OriginalFileName}\",\"None\"");
                }
            }

            logCallback($"Writing report file to {_reportLocation}");

            using (var report = File.CreateText(_reportLocation))
            {
                report.WriteLine("\"Original Filename\",\"Categories\"");
                foreach (var line in reportLines)
                {
                    report.WriteLine(line);
                }
            }
        }
    }
}