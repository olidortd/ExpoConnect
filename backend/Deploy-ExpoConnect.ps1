# ===============================
# ExpoConnect - One Click Deploy
# ===============================

Write-Host "Starting ExpoConnect deploy..." -ForegroundColor Cyan

# Paths
$projectPath = "C:\Users\olido\Desktop\ExpoConnect\ExpoConnect\backend\ExpoConnect.Api"
$publishPath = "C:\apps\ExpoConnect"
$buildConfig = "Release"
$siteName = "ExpoConnect"

# 1) Build + Publish
Write-Host "Building and Publishing project..."
dotnet publish $projectPath -c $buildConfig -o $publishPath

if ($LASTEXITCODE -ne 0) {
    Write-Host "Publish failed. Deployment aborted." -ForegroundColor Red
    exit
}

Write-Host "Publish completed." -ForegroundColor Green

# 2) Restart IIS site
Write-Host "Restarting IIS site $siteName..."
Import-Module WebAdministration

Stop-Website $siteName
Start-Website $siteName

Write-Host " IIS site restarted." -ForegroundColor Green
Write-Host "ExpoConnect deployed successfully!" -ForegroundColor Cyan