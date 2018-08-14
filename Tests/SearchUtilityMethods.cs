using System;
using System.Threading.Tasks;
using Xunit;
using TextFileClassifier.Core;
using TextFileClassifier.Core.Services;

namespace Tests
{
    public class SearchUtilityMethods
    {
        //[Fact]
        public async Task ClearSearchIndex()
        {
            var searchIndex = new AzureSearchIndex(Configuration.SearchServiceName, Configuration.SearchAdminKey);
            await searchIndex.ClearSearchIndex();
        }
    }
}