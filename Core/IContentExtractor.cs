using System;

namespace TextFileClassifier.Core
{
    public interface IContentExtractor
    {
        string ExtractTextContents(byte[] data);
    }
}