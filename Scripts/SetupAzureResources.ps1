<#

.SYNOPSIS
    Deploys Azure environment need to support running the TextFileClassifier.

#>

$location = "eastus"
$rgName = "TextFileClassifierRG"
$storageAccountName = "textfiledatastore"

function Write-LogMessage($msg)
{
    $now = [DateTime]::Now
    Write-Host "[LOG $now]:: $msg"
}

# Resource Group
Write-LogMessage "Creating resource group..."
$rg = Get-AzureRmResourceGroup -Name $rgName -Location $location -ErrorAction SilentlyContinue
if ($null -eq $rg)
{
    New-AzureRmResourceGroup -Name $rgName -Location $location
}
else
{
    Write-LogMessage "Resource group already exists - skipping."
}

# Storage Account w/Event Grid setup
Write-LogMessage "Creating storage account..."
$storageAccount = Get-AzureRmStorageAccount -ResourceGroupName $rgName -Name $storageAccountName -ErrorAction SilentlyContinue
if ($null -eq $storageAccount)
{
    New-AzureRmStorageAccount -ResourceGroupName $rgName `
                            -Name $storageAccountName `
                            -SkuName Standard_LRS `
                            -Location $location `
                            -Kind StorageV2 `
                            -AccessTier Hot
}
else
{
    Write-LogMessage "Storage account already exists - skipping."
}

# Web App Service Plan
Write-LogMessage "Creating App Service Plan..."
$tier = "Free"
$appPlanName = "TextFileClassifierASP"
New-AzureRMAppServicePlan -Location $location -Tier $tier -ResourceGroupName $rgName -Name $appPlanName

# Web App
Write-LogMessage "Creating Web App deployment..."
$appName = "TextFileClassifier"
New-AzureRmWebApp -ResourceGroupName $rgName -Name $appName -Location $location -AppServicePlan $appPlanName

# Start the web app
Write-LogMessage "Starting web app..."
Start-AzureRmWebApp -ResourceGroupName $rgName -Name $appName

# TODO: Function App

# TODO: Search Instance