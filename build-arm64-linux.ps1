# Build script for ARM64 Linux Native AOT
Write-Host "Building EmuDiskExplorer for ARM64 Linux with Native AOT..." -ForegroundColor Green

# Clean previous builds
dotnet clean

# Remove existing linux-arm64 folder if it exists
if (Test-Path "linux-arm64") {
    Write-Host "Removing existing linux-arm64 folder..." -ForegroundColor Yellow
    Remove-Item -Recurse -Force "linux-arm64"
}

# Publish with Native AOT for ARM64 Linux
dotnet publish -c Release -r linux-arm64 --self-contained true /p:PublishAot=true

# Move publish folder to root as linux-arm64
Write-Host "Moving publish folder to root..." -ForegroundColor Yellow
Move-Item ".\bin\Release\net9.0\linux-arm64\publish" ".\linux-arm64"

Write-Host "Build complete! Binary location: .\linux-arm64\" -ForegroundColor Green
Write-Host "Copy the 'linux-arm64' folder contents to your ARM64 Linux system." -ForegroundColor Yellow
Write-Host "The main executable is: .\linux-arm64\EmuDiskExplorer" -ForegroundColor Yellow