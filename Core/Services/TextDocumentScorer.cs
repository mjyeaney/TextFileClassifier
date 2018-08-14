using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace TextFileClassifier.Core.Services
{
    public class TextDocumentScorer : IDocumentScorer
    {
        ISearchIndex _searchIndex;

        public TextDocumentScorer(ISearchIndex searchIndex)
        {
            _searchIndex = searchIndex;
        }

        public async Task<string[]> CategorizeDocument(string documentId)
        {
            var categories = new List<string>();
            var hasHit = false;

            hasHit = await checkCategory1(documentId);
            if (hasHit) categories.Add("Category 1");

            hasHit = await checkCategory2(documentId);
            if (hasHit) categories.Add("Category 2");

            return categories.ToArray();
        }

        private async Task<bool> checkCategory1(string documentId)
        {
            Trace.TraceInformation("Executing checkCategory1...");
            var result = await _searchIndex.QueryDocumentAsync(documentId, "+(\"keyword1\" \"keyword2\")", true);
            return (result.Count > 0);
        }

        private async Task<bool> checkCategory2(string documentId)
        {
            Trace.TraceInformation("Executing checkCategory2...");
            var result = await _searchIndex.QueryDocumentAsync(documentId, "+(\"keyword1\" \"keyword2\")", true);
            return (result.Count > 0);
        }
    }
}