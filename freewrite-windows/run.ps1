# Freewrite Windows - Quick Run Script
Write-Host "Starting Freewrite..." -ForegroundColor Cyan

Set-Location -Path "FreewriteWindows"
dotnet run

Set-Location -Path ".."