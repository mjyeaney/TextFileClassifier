<#
    Creates the environment variables needed to run the function on Azure
#>

$rgName = "TextFileClassifierRG"
$functionAppName = "TextFileClassifierIndexer"

### Azure Function settings
az functionapp config appsettings set -n $functionAppName -g $rgName --settings "TextFileClassifier_AzureStorageConnection="
az functionapp config appsettings set -n $functionAppName -g $rgName --settings "TextFileClassifier_FileUploadContainer="
az functionapp config appsettings set -n $functionAppName -g $rgName --settings "TextFileClassifier_JobStatusContainer="
az functionapp config appsettings set -n $functionAppName -g $rgName --settings "TextFileClassifier_SearchServiceName="
az functionapp config appsettings set -n $functionAppName -g $rgName --settings "TextFileClassifier_SearchApiKey="