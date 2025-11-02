# Freewrite Windows - Build Script
Write-Host "Building Freewrite for Windows..." -ForegroundColor Cyan
Write-Host ""

# Navigate to project directory
Set-Location -Path "FreewriteWindows"

# Build the application
Write-Host "Publishing standalone executable..." -ForegroundColor Yellow
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "Build successful!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Executable location:" -ForegroundColor Cyan
    Write-Host "  .\FreewriteWindows\bin\Release\net8.0-windows\win-x64\publish\Freewrite.exe" -ForegroundColor White
    Write-Host ""
    Write-Host "To create a desktop shortcut:" -ForegroundColor Cyan
    Write-Host "  1. Navigate to the publish folder" -ForegroundColor White
    Write-Host "  2. Right-click Freewrite.exe" -ForegroundColor White
    Write-Host "  3. Select 'Send to' -> 'Desktop (create shortcut)'" -ForegroundColor White
    Write-Host ""
    
    # Ask if user wants to run the app
    $run = Read-Host "Do you want to run Freewrite now? (Y/N)"
    if ($run -eq "Y" -or $run -eq "y") {
        Start-Process ".\bin\Release\net8.0-windows\win-x64\publish\Freewrite.exe"
    }
} else {
    Write-Host ""
    Write-Host "Build failed. Please check the errors above." -ForegroundColor Red
}

# Return to original directory
Set-Location -Path ".."