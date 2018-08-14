<#
    Creates the environment variables needed to run the web app on Azure App Services
#>

$rgName = "TextFileClassifierRG"
$webAppName = "TextFileClassifier"

### Azure WebApp settings
az webapp config appsettings set -n $webAppName -g $rgName --settings "TextFileClassifier_AzureStorageConnection="
az webapp config appsettings set -n $webAppName -g $rgName --settings "TextFileClassifier_FileUploadContainer="
az webapp config appsettings set -n $webAppName -g $rgName --settings "TextFileClassifier_JobStatusContainer="
az webapp config appsettings set -n $webAppName -g $rgName --settings "TextFileClassifier_SearchServiceName="
az webapp config appsettings set -n $webAppName -g $rgName --settings "TextFileClassifier_SearchApiKey="
