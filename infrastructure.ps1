param (
    [Parameter()][switch]$Start,
    [Parameter()][switch]$Browse,
    # [Parameter()][switch]$Seq,
    # [Parameter()][switch]$Jaeger,
    [Parameter()][switch]$Nats,
    # [Parameter()][switch]$Smtp,
    [Parameter()][switch]$Stop,
    [Parameter()][switch]$Destroy
)

# $BrowseAll = -not $Seq -and -not $Jaeger -and -not $Nats -and -not $Smtp
$BrowseAll = -not $Nats

function Open-Services {
    # if ($BrowseAll -or $Jaeger) {
    #     Start-Process "http://localhost:16686"
    # }
    if ($BrowseAll -or $Nats) {
        Start-Process "http://localhost:8222"
    }
    # if ($BrowseAll -or $Seq) {
    #     Start-Process "http://localhost:5380"
    # }
    # if ($BrowseAll -or $Smtp) {
    #     Start-Process "http://localhost:6080"
    # }
}

if ($Start) {
    $Command = "docker compose up -d"
    Write-Host $Command -ForegroundColor Green
    Invoke-Expression $Command

    # Write-Host "Jaeger        : http://localhost:16686"
    Write-Host "Nats          : http://localhost:8222"
    # Write-Host "Seq           : http://localhost:5380"
    # Write-Host "Smtp4Dev      : http://localhost:6080"

    if ($Browse) {
        Open-Services
    }

    return
}

if ($Browse) {
    Open-Services
    return
}

if ($Stop) {
    $Command = "docker compose stop"
    Write-Host $Command -ForegroundColor Green
    Invoke-Expression $Command
    return
}

if ($Destroy) {
    $Command = "docker compose down --remove-orphans --rmi local --volumes"
    Write-Host $Command -ForegroundColor Green
    Invoke-Expression $Command
    return
}

$Command = "docker compose ps"
Write-Host $Command -ForegroundColor Green
Invoke-Expression $Command
