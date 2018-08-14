using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TextFileClassifier.Core
{
    public interface IJobStatusStore
    {
        Task UpdateStatusAsync(JobStatus newStatus);

        Task<JobStatus> ReadStatusAsync(string JobId);

        Task<IEnumerable<JobStatus>> ReadAllStatusAsync();
    }
}