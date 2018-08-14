using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TextFileClassifier.Core;
using TextFileClassifier.Core.Services;
using TextFileClassifier.Web.Models;

namespace TextFileClassifier.Web.Controllers
{
    public class ResultsController : Controller
    {
        private IConfiguration config;
        private IJobStatusStore jobStore;

        public ResultsController(IConfiguration configuration)
        {
            config = configuration;
            jobStore = new JobStatusStore(config["TextFileClassifier_AzureStorageConnection"],
                config["TextFileClassifier_JobStatusContainer"]);
        }

        [HttpGet("Results/{jobId}")]
        public async Task<IActionResult> Index(string jobId)
        {
            try
            {
                var job = await jobStore.ReadStatusAsync(jobId);
                ViewBag.JobDetails = job;
            }
            catch
            {
                ViewBag.ErrorMessage = "Requested job details not found.";
            }
            return View();
        }
    }
}