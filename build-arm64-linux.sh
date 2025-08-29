#!/bin/bash

# Build script for ARM64 Linux Native AOT
echo "Building EmuDiskExplorer for ARM64 Linux with Native AOT..."

# Clean previous builds
dotnet clean

# Remove existing linux-arm64 folder if it exists
if [ -d "linux-arm64" ]; then
    echo "Removing existing linux-arm64 folder..."
    rm -rf linux-arm64
fi

# Publish with Native AOT for ARM64 Linux
dotnet publish -c Release -r linux-arm64 --self-contained true /p:PublishAot=true

# Move publish folder to root as linux-arm64
echo "Moving publish folder to root..."
mv ./bin/Release/net9.0/linux-arm64/publish ./linux-arm64

echo "Build complete! Binary location: ./linux-arm64/"
echo "Copy the 'linux-arm64' folder contents to your ARM64 Linux system."
echo "The main executable is: ./linux-arm64/EmuDiskExplorer"