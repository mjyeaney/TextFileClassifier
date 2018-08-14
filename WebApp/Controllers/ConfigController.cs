using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TextFileClassifier.Web.Models;

namespace TextFileClassifier.Web.Controllers
{
    public class ConfigController : Controller
    {
        const string CONFIG_PREFIX = "TextFileClassifier_";

        private IConfiguration config;

        public ConfigController(IConfiguration configuration)
        {
            config = configuration;
        }

        public IActionResult Index()
        {
            ViewBag.ConfigSettings = config
                .AsEnumerable()
                .Where((kvp) => {
                    return kvp.Key.StartsWith(CONFIG_PREFIX);
                })
                .OrderBy((kvp) => kvp.Key);
                
            return View();
        }
    }
}