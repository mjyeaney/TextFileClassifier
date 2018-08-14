<#
    Shuts down the Function and App Service to demo the application.
#>

$rgName = "TextFileClassifierRG"
$webAppName = "TextFileClassifier"
$functionAppName = "TextFileClassifierIndexer"

Write-Output "Shutting down function app..."
az functionapp stop -n $functionAppName -g $rgName

Write-Output "Shutting down web app..."
az webapp stop -n $webAppName -g $rgName