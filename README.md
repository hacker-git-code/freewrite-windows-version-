# Freewrite for Windows

A simple, open-source Windows app to freewrite.

This is the Windows version of the [Freewrite macOS app](https://www.freewrite.io/).

## Features

- **Clean writing interface** - Distraction-free writing with a minimal UI
- **Auto-save** - Your work is automatically saved every second
- **Multiple fonts** - Choose between Segoe UI, Arial, and Consolas
- **Font sizes** - Adjust text size from 16px to 26px
- **Timer** - 15-minute countdown timer to keep you focused
- **History** - View and access all your previous entries
- **Dark mode** - Toggle between light and dark themes
- **Entry management** - Create new entries, each saved as a separate markdown file

## Requirements

- Windows 10 or later
- .NET 8.0 Runtime

## Quick Start

### Option 1: Run Directly (Requires .NET 8.0 SDK)

1. Install [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) if not installed

2. Clone/download this repository

3. Open PowerShell in the cd location of `freewrite-windows\FreewriteWindows` folder

4. Run this single command:
   ```powershell
   dotnet run
   ```

``this would run the app directly from the source code``


if you wanna make a standalone executable .exe file use this code 

```dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true 

```

this would make a exe in the ```bin\Release\net8.0-windows\win-x64\publish\ folder```

exe file is ready to use
