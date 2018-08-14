using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using TextFileClassifier.Core;
using TextFileClassifier.Core.Services;

namespace Tests
{
    public class JobStatusStoreTests
    {
        [Fact]
        public async void CanUpdateJobStatus()
        {
            var jobStatusStore = new JobStatusStore(Configuration.StorageConnectionString, Configuration.JobStatusContainerName);
            var jobId = Guid.NewGuid().ToString().Replace("-", String.Empty);
            var job = new JobStatus();

            job.JobId = jobId;
            job.IsComplete = false;
            job.Message = "This is not complete.";
            await jobStatusStore.UpdateStatusAsync(job);
            var job1 = await jobStatusStore.ReadStatusAsync(jobId);
            Assert.False(job1.IsComplete);

            job.IsComplete = true;
            job.Message = "This is now complete!!!";
            await jobStatusStore.UpdateStatusAsync(job);
            var job2 = await jobStatusStore.ReadStatusAsync(jobId);
            Assert.True(job2.IsComplete);
        }

        [Fact]
        public async void CanReadMultipleJobDetails()
        {
            var jobStatusStore = new JobStatusStore(Configuration.StorageConnectionString, Configuration.JobStatusContainerName);
            var jobCount = 10;
            var jobs = new Dictionary<string, JobStatus>();

            for (var i = 0; i < jobCount; i++)
            {
                var jobId = Guid.NewGuid().ToString().Replace("-", String.Empty);
                var job = new JobStatus();
                job.JobId = jobId;
                job.OriginalFileName = $"Some_file_{i}.txt";
                job.IsComplete = false;
                job.JobStartTime = DateTime.UtcNow;
                job.JobEndTime = DateTime.UtcNow.AddMinutes(5);
                job.Message = $"This is job {i}";
                job.Categories = new[] { "category1", "category2", "category3" };
                await jobStatusStore.UpdateStatusAsync(job);
                jobs.Add(jobId, job);
            }

            var listedJobs = await jobStatusStore.ReadAllStatusAsync();

            foreach (var listItem in listedJobs)
            {
                var matchingStatus = jobs[listItem.JobId];
                Assert.Equal(listItem.JobId, matchingStatus.JobId);
                Assert.Equal(listItem.OriginalFileName, matchingStatus.OriginalFileName);
                Assert.Equal(listItem.IsComplete, matchingStatus.IsComplete);
                Assert.Equal(listItem.Message, matchingStatus.Message);
                Assert.Equal(listItem.JobStartTime, matchingStatus.JobStartTime);
                Assert.Equal(listItem.JobEndTime, matchingStatus.JobEndTime);
                Assert.Equal(listItem.Categories[0], matchingStatus.Categories[0]);
                Assert.Equal(listItem.Categories[1], matchingStatus.Categories[1]);
                Assert.Equal(listItem.Categories[2], matchingStatus.Categories[2]);
            }
        }
    }
}