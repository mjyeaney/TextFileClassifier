using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using TextFileClassifier.Core;
using TextFileClassifier.Core.Services;

namespace Tests
{
    public class AzureSearchIndexTests
    {
        private Random rng = new Random();

        [Fact]
        public async void CanIndexPlainTextDocuments()
        {
            var documentId1 = rng.Next(1000, 10000).ToString();
            var textContent1 = "This is some special content to index again, even though you already did.";
            var documentId2 = rng.Next(1000, 10000).ToString();
            var textContent2 = "In other cases, you may want to look for content that isn't so special";
            var searchText = "special content";
            var searchIndexer = new AzureSearchIndex(Configuration.SearchServiceName, Configuration.SearchAdminKey);

            try 
            {
                await searchIndexer.IndexTextContentAsync(documentId1, textContent1);
                await searchIndexer.IndexTextContentAsync(documentId2, textContent2);
                await searchIndexer.VerifyDocumentAvailable(documentId1, 5);
                await searchIndexer.VerifyDocumentAvailable(documentId2, 5);

                var results = await searchIndexer.QuerySearchIndexAsync(searchText);
                Assert.Equal(2, results.Count);
            } 
            finally 
            {
                 await searchIndexer.RemoveTextContentAsync(documentId1);
                 await searchIndexer.RemoveTextContentAsync(documentId2);
            }
        }

        [Fact]
        public async void CanSearchSingleDocument()
        {
            var documentId = rng.Next(1000, 10000).ToString();
            var textContent = "This is text for a single document, that may contain information about a police report.";
            var searchText = "police report";
            var searchIndexer = new AzureSearchIndex(Configuration.SearchServiceName, Configuration.SearchAdminKey);

            try 
            {
                await searchIndexer.IndexTextContentAsync(documentId, textContent);
                await searchIndexer.VerifyDocumentAvailable(documentId, 5);

                var results = await searchIndexer.QueryDocumentAsync(documentId, searchText);
                Assert.Equal(1, results.Count);
                Assert.Equal(documentId, results.Results.First().Document.DocumentId);
            } 
            finally 
            {
                 await searchIndexer.RemoveTextContentAsync(documentId);
            }
        }

        [Theory]
        [InlineData("+police", 1)]
        [InlineData("+(police discrimination)", 1)]
        [InlineData("+police !abuse", 0)]
        public async void SupportsSearchUsingLuceneSyntax(string searchText, int expectedCount)
        {
            var documentId = rng.Next(1000, 10000).ToString();
            var textContent = "This is a file that may contain a keyword such as police, discrimination, or abuse.";
            var searchIndexer = new AzureSearchIndex(Configuration.SearchServiceName, Configuration.SearchAdminKey);

            try 
            {
                await searchIndexer.IndexTextContentAsync(documentId, textContent);
                await searchIndexer.VerifyDocumentAvailable(documentId, 5);

                var results = await searchIndexer.QueryDocumentAsync(documentId, searchText);
                Assert.Equal(expectedCount, results.Count);
            } 
            finally 
            {
                 await searchIndexer.RemoveTextContentAsync(documentId);
            }
        }

        [Theory]
        [InlineData("\"police contain\"~2", 0)]
        [InlineData("\"police contain\"~6", 1)]
        public async void SupportProximitySearch(string searchText, int expectedResultCount)
        {
            var documentId = rng.Next(1000, 10000).ToString();
            var textContent = "This is a file that may contain a keyword such as police, discrimination, or abuse.";
            var searchIndexer = new AzureSearchIndex(Configuration.SearchServiceName, Configuration.SearchAdminKey);

            try 
            {
                await searchIndexer.IndexTextContentAsync(documentId, textContent);
                await searchIndexer.VerifyDocumentAvailable(documentId, 5);

                var results = await searchIndexer.QueryDocumentAsync(documentId, searchText);
                Assert.Equal(expectedResultCount, results.Count);
            } 
            finally 
            {
                 await searchIndexer.RemoveTextContentAsync(documentId);
            }
        }

        [Theory]
        [InlineData("+WHISTLEBLOWER -amazing -donald", 1)]
        public async void SupportExclusionCriteria(string searchText, int expectedResultCount)
        {
            var documentId = rng.Next(1000, 10000).ToString();
            var textContent = "This phrase will contain WHISTLEBLOWER but not the secret word.";
            var searchIndexer = new AzureSearchIndex(Configuration.SearchServiceName, Configuration.SearchAdminKey);

            try 
            {
                await searchIndexer.IndexTextContentAsync(documentId, textContent);
                await searchIndexer.VerifyDocumentAvailable(documentId, 5);

                var results = await searchIndexer.QueryDocumentAsync(documentId, searchText);
                Assert.Equal(expectedResultCount, results.Count);
            } 
            finally 
            {
                 await searchIndexer.RemoveTextContentAsync(documentId);
            }
        }

        [Theory]
        [InlineData("+(one dog cat)", true)]
        [InlineData("+(multiple dog cat)", true)]
        [InlineData("+(keywords dog cat)", true)]
        public async void SupportMultipleCriteriaWithSingleQuery(string queryText, bool doesMatch)
        {
            var documentId = rng.Next(1000, 10000).ToString();
            var textContent = "This sentence will hit on one word, but the query contains multiple keywords.";
            var searchIndexer = new AzureSearchIndex(Configuration.SearchServiceName, Configuration.SearchAdminKey);

            try 
            {
                await searchIndexer.IndexTextContentAsync(documentId, textContent);
                await searchIndexer.VerifyDocumentAvailable(documentId, 5);

                var results = await searchIndexer.QueryDocumentAsync(documentId, queryText, true);
                Assert.Equal(doesMatch, (results.Count == 1));
            } 
            finally 
            {
                 await searchIndexer.RemoveTextContentAsync(documentId);
            }
        }

        [Fact]
        public async void CanVerifyNonExistentDocument()
        {
            var searchIndexer = new AzureSearchIndex(Configuration.SearchServiceName, Configuration.SearchAdminKey);
            var doesExist = await searchIndexer.VerifyDocumentAvailable("123", 5);
            Assert.False(doesExist);
        }
    }
}