using System;

namespace TextFileClassifier.BulkUploader
{
    public interface IConsoleTask
    {
        void Execute(Action<string> logCallback);
    }
}