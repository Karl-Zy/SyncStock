# Close Visual Studio first, then run:  powershell -ExecutionPolicy Bypass -File fix-csproj.ps1
$csproj = Join-Path $PSScriptRoot "SyncStock\SyncStock.csproj"
$fixed  = Join-Path $PSScriptRoot "SyncStock\SyncStock.csproj.fix"

if (-not (Test-Path $fixed)) {
    Write-Error "Missing SyncStock.csproj.fix"
    exit 1
}

try {
    Copy-Item -Path $fixed -Destination $csproj -Force
    Write-Host "OK: SyncStock.csproj updated (merge markers removed)."
}
catch {
    Write-Host "FAILED: Close Visual Studio completely, then run this script again."
    Write-Host $_.Exception.Message
    exit 1
}
