<#
    Creates the environment variables needed to run locally
#>

### Local settings
[System.Environment]::SetEnvironmentVariable("TextFileClassifier_AzureStorageConnection", "", [System.EnvironmentVariableTarget]::Machine)
[System.Environment]::SetEnvironmentVariable("TextFileClassifier_FileUploadContainer", "", [System.EnvironmentVariableTarget]::Machine)
[System.Environment]::SetEnvironmentVariable("TextFileClassifier_JobStatusContainer", "", [System.EnvironmentVariableTarget]::Machine)
[System.Environment]::SetEnvironmentVariable("TextFileClassifier_SearchServiceName", "", [System.EnvironmentVariableTarget]::Machine)
[System.Environment]::SetEnvironmentVariable("TextFileClassifier_SearchApiKey", "", [System.EnvironmentVariableTarget]::Machine)