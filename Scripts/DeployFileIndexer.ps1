$appName = "TextClassifierIndexer"
$resourceGroup = "TextFileClassifierRG"
$publishFolder = "../Publish"
$destinationFolder = "../Artifacts"
$destinationFile = "fileindexer.zip"

# delete any previous publish
if (Test-path $publishFolder)
{
    Remove-Item -Recurse -Force $publishFolder
}

mkdir $publishFolder
$publishFolder = Resolve-Path $publishFolder

if (Test-path $destinationFolder)
{
    Remove-item -Recurse -Force $destinationFolder
}

mkdir $destinationFolder
$destinationFolder = (Resolve-Path $destinationFolder).Path + "\$destinationFile"

Write-Output $destinationFolder

Set-Location (Convert-Path "../FileIndexer")
dotnet publish -c release -o $publishFolder /p:CopyOutputSymbolsToPublishDirectory=false
Add-Type -assembly "system.io.compression.filesystem"
[system.io.compression.zipfile]::CreateFromDirectory($publishFolder, $destinationFolder)

# deploy zip using the kudu zip api
az functionapp deployment source config-zip -n $appName -g $resourceGroup --src $destinationFolder

# cleanup
Remove-Item -Recurse -Force $publishFolder
Set-Location (Convert-Path "../Scripts")