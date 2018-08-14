using System;
using TextFileClassifier.Core.Services;

namespace TextFileClassifier.Core
{
    public static class Configuration
    {
        public static string StorageConnectionString 
        { 
            get
            {
                return Environment.GetEnvironmentVariable("TextFileClassifier_AzureStorageConnection");
            }
        }

        public static string FileUploadContainerName
        {
            get 
            {
                return Environment.GetEnvironmentVariable("TextFileClassifier_FileUploadContainer");
            }
        }

        public static string JobStatusContainerName
        {
            get 
            {
                return Environment.GetEnvironmentVariable("TextFileClassifier_JobStatusContainer");
            }
        }

        public static string SearchServiceName
        {
            get
            {
                return Environment.GetEnvironmentVariable("TextFileClassifier_SearchServiceName");
            }
        }

        public static string SearchAdminKey
        {
            get 
            {
                return Environment.GetEnvironmentVariable("TextFileClassifier_SearchApiKey");
            }
        }
    }
}