param(
    [Parameter()][switch] $Init,
    [Parameter()][switch] $NoInit,
    [Parameter()][switch] $Infrastructure,
    [Parameter()][switch] $NoInfrastructure,
    [Parameter()][switch] $Database,
    [Parameter()][switch] $NoDatabase,
    [Parameter()][switch] $Run,
    [Parameter()][switch] $NoRun
)

if (-not $Init -and -not $Infrastructure -and -not $Database -and -not $Run) {
    $Init = $True
    $Infrastructure = $True
    $Database = $True
    $Run = $True
}

if ($NoInit) { $Init = $False }
if ($NoInfrastructure) { $Infrastructure = $False }
if ($NoDatabase) { $Database = $False }
if ($NoRun) { $Run = $False }

if ($Init) {
    Write-Host "⌛ Checking Tools and Dependencies..." -ForegroundColor Green
    .\init.ps1
}

if ($Infrastructure) {
    Write-Host "⌛ Starting Infrastructure Services..." -ForegroundColor Green
    .\infrastructure.ps1 -Start
}

# if ($Database) {
#     Write-Host "⌛ Checking Database Schema for Changes..." -ForegroundColor Green
#     .\build.ps1 -Database

#     Write-Host "⌛ Updating Database Schema ..." -ForegroundColor Green
#     .\db-publish.ps1 -DBProfile local -Password P4ssword
# }

if ($Run) {
    Write-Host "⌛ Running Applications ..." -ForegroundColor Green
    .\run.ps1
}
