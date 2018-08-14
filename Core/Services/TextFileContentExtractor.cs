using System;
using System.Text;

namespace TextFileClassifier.Core.Services
{
    public class TextFileContentExtractor : IContentExtractor
    {
        public string ExtractTextContents(byte[] data)
        {
            // Simple case for UTF8 text files...expland to other IContentExtractors as needed.
            return Encoding.UTF8.GetString(data); 
        }
    }
}