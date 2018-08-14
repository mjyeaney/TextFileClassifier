using System;
using System.Threading.Tasks;

namespace TextFileClassifier.Core
{
    public interface IDocumentScorer
    {
        Task<string[]> CategorizeDocument(string documentId);
    }
}