using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace TextFileClassifier.Core.Services
{
    public class AzureSearchIndex : ISearchIndex
    {
        const string INDEX_NAME = "textclassificationdata";
        private static Object _syncRoot = new Object();
        private SearchServiceClient searchClient;
        private SearchIndexClient indexClient;

        public AzureSearchIndex(string searchServiceName, string adminApiKey)
        {
            searchClient = new SearchServiceClient(searchServiceName, new SearchCredentials(adminApiKey));

            if (!searchClient.Indexes.Exists(INDEX_NAME))
            {
                lock (_syncRoot)
                {
                    var definition = new Index()
                    {
                        Name = INDEX_NAME,
                        Fields = FieldBuilder.BuildForType<TextDocument>()
                    };

                    searchClient.Indexes.CreateAsync(definition).Wait();
                }
            }

            indexClient = new SearchIndexClient(searchServiceName, INDEX_NAME, new SearchCredentials(adminApiKey));
        }

        public async Task<bool> VerifyDocumentAvailable(string documentId, int maxFailures)
        {
            var worker = new Func<Task<bool>>(async () =>
            {
                var parameters = new SearchParameters();
                parameters.IncludeTotalResultCount = true;
                parameters.SearchMode = SearchMode.All;
                parameters.QueryType = QueryType.Full;
                parameters.Filter = $"DocumentId eq '{documentId}'";
                var results = await indexClient.Documents.SearchAsync<TextDocument>("*", parameters);
                return (results.Count == 1);
            });

            var failureCount = 0;
            var result = false;

            while (failureCount < maxFailures)
            {
                result = await worker();

                if (!result)
                {
                    failureCount++;
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        public async Task IndexTextContentAsync(string documentId, string textContent)
        {
            var doc = new TextDocument();
            doc.DocumentId = documentId;
            doc.Content = textContent;
            var batch = IndexBatch.Upload(new TextDocument[] { doc });
            await indexClient.Documents.IndexAsync(batch);
        }

        public async Task<DocumentSearchResult<TextDocument>> QuerySearchIndexAsync(string searchText, bool anyKeyword = false)
        {
            var parameters = new SearchParameters();
            parameters.IncludeTotalResultCount = true;
            if (anyKeyword)
            {
                parameters.SearchMode = SearchMode.Any;
            }
            else
            {
                parameters.SearchMode = SearchMode.All;
            }
            parameters.QueryType = QueryType.Full;
            return await indexClient.Documents.SearchAsync<TextDocument>(searchText, parameters);
        }

        public async Task<DocumentSearchResult<TextDocument>> QueryDocumentAsync(string documentId, string searchText, bool anyKeyword = false)
        {
            var parameters = new SearchParameters();
            parameters.IncludeTotalResultCount = true;
            if (anyKeyword)
            {
                parameters.SearchMode = SearchMode.Any;
            }
            else
            {
                parameters.SearchMode = SearchMode.All;
            }
            parameters.QueryType = QueryType.Full;
            parameters.Filter = $"DocumentId eq '{documentId}'";
            return await indexClient.Documents.SearchAsync<TextDocument>(searchText, parameters);
        }

        public async Task RemoveTextContentAsync(string documentId)
        {
            var batch = IndexBatch.Delete("DocumentId", new[] { documentId });
            await indexClient.Documents.IndexAsync(batch);
        }

        public async Task ClearSearchIndex()
        {
            if (searchClient.Indexes.Exists(INDEX_NAME))
            {
                await searchClient.Indexes.DeleteAsync(INDEX_NAME);
            }
        }
    }
}