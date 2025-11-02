# Freewrite Windows - Quick Install Script
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "  Freewrite for Windows Installer" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

# Check if .NET SDK is installed
Write-Host "Checking for .NET SDK..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: .NET SDK not found!" -ForegroundColor Red
    Write-Host "Please install .NET 8.0 SDK from: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}
Write-Host "Found .NET SDK version: $dotnetVersion" -ForegroundColor Green
Write-Host ""

# Navigate to project directory
Set-Location -Path "FreewriteWindows"

# Build the application
Write-Host "Building Freewrite..." -ForegroundColor Yellow
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:DebugType=None -p:DebugSymbols=false

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "Installation successful!" -ForegroundColor Green
    Write-Host ""
    
    $exePath = ".\bin\Release\net8.0-windows\win-x64\publish\Freewrite.exe"
    $desktopPath = [Environment]::GetFolderPath("Desktop")
    
    # Ask if user wants to create desktop shortcut
    $createShortcut = Read-Host "Create desktop shortcut? (Y/N)"
    if ($createShortcut -eq "Y" -or $createShortcut -eq "y") {
        $WshShell = New-Object -comObject WScript.Shell
        $Shortcut = $WshShell.CreateShortcut("$desktopPath\Freewrite.lnk")
        $Shortcut.TargetPath = (Resolve-Path $exePath).Path
        $Shortcut.WorkingDirectory = Split-Path (Resolve-Path $exePath).Path
        $Shortcut.Description = "Freewrite - Simple writing app"
        $Shortcut.Save()
        Write-Host "Desktop shortcut created!" -ForegroundColor Green
    }
    
    Write-Host ""
    Write-Host "Executable location:" -ForegroundColor Cyan
    Write-Host "  $(Resolve-Path $exePath)" -ForegroundColor White
    Write-Host ""
    
    # Ask if user wants to run the app
    $run = Read-Host "Launch Freewrite now? (Y/N)"
    if ($run -eq "Y" -or $run -eq "y") {
        Start-Process (Resolve-Path $exePath).Path
    }
} else {
    Write-Host ""
    Write-Host "Build failed. Please check the errors above." -ForegroundColor Red
}

# Return to original directory
Set-Location -Path ".."
Write-Host ""
Read-Host "Press Enter to exit"