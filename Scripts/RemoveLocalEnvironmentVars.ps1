<#
    Creates the environment variables needed to run locally
#>

### Local settings
[System.Environment]::SetEnvironmentVariable("TextFileClassifier_AzureStorageConnection", $null, [System.EnvironmentVariableTarget]::Machine)
[System.Environment]::SetEnvironmentVariable("TextFileClassifier_FileUploadContainer", $null, [System.EnvironmentVariableTarget]::Machine)
[System.Environment]::SetEnvironmentVariable("TextFileClassifier_JobStatusContainer", $null, [System.EnvironmentVariableTarget]::Machine)
[System.Environment]::SetEnvironmentVariable("TextFileClassifier_SearchServiceName", $null, [System.EnvironmentVariableTarget]::Machine)
[System.Environment]::SetEnvironmentVariable("TextFileClassifier_SearchApiKey", $null, [System.EnvironmentVariableTarget]::Machine)