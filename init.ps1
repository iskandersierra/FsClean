$Command = "dotnet tool restore"
Write-Host $Command -ForegroundColor Green
Invoke-Expression $Command

$Command = "dotnet restore"
Write-Host $Command -ForegroundColor Green
Invoke-Expression $Command
