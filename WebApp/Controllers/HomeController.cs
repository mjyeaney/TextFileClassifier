using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using TextFileClassifier.Core;
using TextFileClassifier.Core.Services;
using TextFileClassifier.Web.Models;

namespace TextFileClassifier.Web.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration config;
        private IJobStatusStore jobStore;
        private IMemoryCache cache;

        public HomeController(IConfiguration configuration, IMemoryCache memoryCache)
        {
            config = configuration;
            cache = memoryCache;
            jobStore = new JobStatusStore(config["TextFileClassifier_AzureStorageConnection"],
                config["TextFileClassifier_JobStatusContainer"]);
        }

        public async Task<IActionResult> Index()
        {
            var cacheKey = "CACHE_RECENT_ITEMS";
            var results = (List<JobStatus>)null;
            if (!cache.TryGetValue(cacheKey, out results))
            {
                results = (await jobStore.ReadAllStatusAsync()).ToList();
                
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(3));

                cache.Set(cacheKey, results, cacheEntryOptions);
            }
            ViewBag.RecentItems = results;
            return View();
        }

        public IActionResult FileUploader()
        {
            ViewBag.UiState = "Ready";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FileUploader(IFormFile policyFile)
        {
            // NOTE: Can't use real file name...downstream search can't use this as a docid
            var jobId = Guid.NewGuid().ToString().Replace("-", String.Empty);
            var cleanName = Guid.NewGuid().ToString().Replace("-", String.Empty);
            var uploadName = String.Format("{0}/{1}", jobId, cleanName);
            ViewBag.UiState = "Saving";
            ViewBag.JobIdentifier = jobId;

            // Persist job info
            var job = new JobStatus();
            job.JobId = jobId;
            job.JobStartTime = DateTime.UtcNow;
            job.OriginalFileName = Path.GetFileName(policyFile.FileName);
            job.Message = "Saving file";
            job.IsComplete = false;
            await jobStore.UpdateStatusAsync(job);
            
            // Save to blob storage
            var storageAccount = CloudStorageAccount.Parse(config["TextFileClassifier_AzureStorageConnection"]);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(config["TextFileClassifier_FileUploadContainer"]);
            await container.CreateIfNotExistsAsync();
            using (var uploadStream = policyFile.OpenReadStream()){
                await container.GetBlockBlobReference(uploadName).UploadFromStreamAsync(uploadStream);
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> CheckFileStatus(string jobId)
        {
            // Read this status (along with a job ID) from a message store
            var status = await jobStore.ReadStatusAsync(jobId);
            return new JsonResult(status);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
