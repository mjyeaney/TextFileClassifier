using System;
using System.Text;
using Xunit;
using TextFileClassifier.Core;
using TextFileClassifier.Core.Services;

namespace Tests
{
    public class TextFileContentExtractorTests
    {
        [Fact]
        public void CanExtractUTF8TextContents()
        {
            var sampleTextFileContent = "This is just some sample text that could be in a file.";
            var extractor = new TextFileContentExtractor();
            var extractedContent = extractor.ExtractTextContents(Encoding.UTF8.GetBytes(sampleTextFileContent));
            Assert.Equal(sampleTextFileContent, extractedContent);
        }
    }
}