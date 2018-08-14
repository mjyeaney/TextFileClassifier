using System;
using System.Threading.Tasks;

namespace TextFileClassifier.Core
{
    public interface IBlobReader
    {
        Task<byte[]> ReadFileDataAsync(string blobId);
    }
}