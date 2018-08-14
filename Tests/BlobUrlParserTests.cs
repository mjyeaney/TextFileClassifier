using System;
using Xunit;
using TextFileClassifier.Core;
using TextFileClassifier.Core.Services;

namespace Tests
{
    public class BlobUrlParserTests
    {
        [Fact]
        public void CorrectlyParsesSimpleUrl()
        {
            var simpleUrl = "https://my-storage-account.blob.core.windows.net/container/someJob/someDoc.txt";
            var details = BlobUrlParser.Parse(simpleUrl);

            Assert.Equal("container", details.ContainerName);
            Assert.Equal("someJob", details.JobId);
            Assert.Equal("someDoc.txt", details.DocumentId);
            Assert.Equal("someJob/someDoc.txt", details.FullBlobPath);
        }

        [Fact]
        public void CanHandleIncorrectlyFormattedUrl()
        {
            var simpleUrl = "https://my-storage-account.blob.core.windows.net/container/someDoc.txt";
            var details = BlobUrlParser.Parse(simpleUrl);

            Assert.Equal("container", details.ContainerName);
            Assert.Equal(String.Empty, details.JobId);
            Assert.Equal(String.Empty, details.DocumentId);
            Assert.Equal("someDoc.txt", details.FullBlobPath);
        }
    }
}