<#
    Starts up the Function and App Service to demo the application.
#>

$rgName = "TextFileClassifierRG"
$webAppName = "TextFileClassifier"
$functionAppName = "TextFileClassifierIndexer"

Write-Output "Starting function app..."
az functionapp start -n $functionAppName -g $rgName

Write-Output "Starting web app..."
az webapp start -n $webAppName -g $rgName