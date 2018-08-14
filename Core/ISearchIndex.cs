using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Search.Models;

namespace TextFileClassifier.Core
{
    public interface ISearchIndex
    {
        Task<bool> VerifyDocumentAvailable(string documentId, int maxFailures);

        Task IndexTextContentAsync(string documentId, string textContent);

        Task<DocumentSearchResult<TextDocument>> QuerySearchIndexAsync(string searchText, bool anyKeyword = false);

        Task<DocumentSearchResult<TextDocument>> QueryDocumentAsync(string documentId, string searchText, bool anyKeyword = false);

        Task RemoveTextContentAsync(string documentId);

        Task ClearSearchIndex();
    }
}