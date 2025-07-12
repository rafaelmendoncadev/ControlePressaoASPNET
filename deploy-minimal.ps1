#!/usr/bin/env powershell

# Deploy Minimal - Ultra-optimized build script for Azure
Write-Host "Starting ultra-minimal deployment build..." -ForegroundColor Green

# Clean previous builds
if (Test-Path "publish") { Remove-Item -Path "publish" -Recurse -Force }
if (Test-Path "ControlePressao-Minimal.zip") { Remove-Item -Path "ControlePressao-Minimal.zip" -Force }

# Build and publish with maximum optimization
Write-Host "Building project with maximum optimization..." -ForegroundColor Yellow
dotnet publish ControlePressao/ControlePressao.csproj `
    -c Release `
    -o publish `
    --self-contained false `
    /p:PublishSingleFile=false `
    /p:PublishReadyToRun=false `
    /p:DebugType=None `
    /p:DebugSymbols=false `
    /p:Optimize=true

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Remove unnecessary files more aggressively
Write-Host "Removing unnecessary files..." -ForegroundColor Yellow

# Remove debug files
Get-ChildItem -Path "publish" -Filter "*.pdb" -Recurse | Remove-Item -Force
Get-ChildItem -Path "publish" -Filter "*.xml" -Recurse | Remove-Item -Force
Get-ChildItem -Path "publish" -Filter "*.deps.json" -Recurse | Remove-Item -Force

# Remove development files
if (Test-Path "publish/appsettings.Development.json") { Remove-Item "publish/appsettings.Development.json" -Force }

# Remove language/culture folders (more aggressive)
$culturesToRemove = @(
    "ar", "bg", "cs", "da", "de", "el", "es", "et", "fa", "fi", "fr", "he", "hi", "hr", "hu", "id", "it", "ja", "ko", "lt", "lv", "ms", "nb", "nl", "pl", "pt", "pt-BR", "ro", "ru", "sk", "sl", "sr", "sv", "th", "tr", "uk", "vi", "zh-Hans", "zh-Hant"
)

foreach ($culture in $culturesToRemove) {
    if (Test-Path "publish/$culture") {
        Remove-Item -Path "publish/$culture" -Recurse -Force
        Write-Host "Removed culture folder: $culture" -ForegroundColor Gray
    }
}

# Remove specific large libraries that might not be needed
$unnecessaryFiles = @(
    "Microsoft.CodeAnalysis*.dll",
    "Microsoft.Build*.dll",
    "Microsoft.DiaSymReader*.dll",
    "Microsoft.VisualStudio*.dll",
    "Microsoft.DotNet.Scaffolding*.dll",
    "dotnet-aspnet-codegenerator-design.dll",
    "Humanizer.dll"
)

foreach ($pattern in $unnecessaryFiles) {
    Get-ChildItem -Path "publish" -Filter $pattern -Recurse | Remove-Item -Force -ErrorAction SilentlyContinue
    Write-Host "Removed files matching: $pattern" -ForegroundColor Gray
}

# Remove ref folder if it exists (reference assemblies)
if (Test-Path "publish/ref") {
    Remove-Item -Path "publish/ref" -Recurse -Force
    Write-Host "Removed ref folder" -ForegroundColor Gray
}

# Remove any remaining .pdb files
Get-ChildItem -Path "publish" -Filter "*.pdb" -Recurse | Remove-Item -Force -ErrorAction SilentlyContinue

# Keep only essential files
Write-Host "Keeping only essential files..." -ForegroundColor Yellow

# Create a temporary folder with only essential files
$tempDir = "publish-temp"
if (Test-Path $tempDir) { Remove-Item -Path $tempDir -Recurse -Force }
New-Item -ItemType Directory -Path $tempDir | Out-Null

# Copy essential files
$essentialFiles = @(
    "ControlePressao.exe",
    "ControlePressao.dll",
    "ControlePressao.runtimeconfig.json",
    "appsettings.json",
    "appsettings.Production.json",
    "web.config",
    "app.db"
)

foreach ($file in $essentialFiles) {
    if (Test-Path "publish/$file") {
        Copy-Item "publish/$file" "$tempDir/$file"
        Write-Host "Copied essential file: $file" -ForegroundColor Gray
    }
}

# Copy essential .NET runtime files
$runtimeFiles = Get-ChildItem -Path "publish" -Filter "*.dll" | Where-Object { 
    $_.Name -like "Microsoft.AspNetCore*" -or 
    $_.Name -like "Microsoft.Extensions*" -or 
    $_.Name -like "Microsoft.EntityFrameworkCore*" -or
    $_.Name -like "Microsoft.Data.Sqlite*" -or
    $_.Name -like "System.*" -or
    $_.Name -like "Microsoft.Bcl*" -or
    $_.Name -like "Microsoft.NET.StringTools*"
}

foreach ($file in $runtimeFiles) {
    Copy-Item $file.FullName "$tempDir/$($file.Name)"
    Write-Host "Copied runtime file: $($file.Name)" -ForegroundColor Gray
}

# Copy wwwroot folder
if (Test-Path "publish/wwwroot") {
    Copy-Item -Path "publish/wwwroot" -Destination "$tempDir/wwwroot" -Recurse
    Write-Host "Copied wwwroot folder" -ForegroundColor Gray
}

# Replace publish with temp
Remove-Item -Path "publish" -Recurse -Force
Rename-Item -Path $tempDir -NewName "publish"

# Check final size
$publishSize = (Get-ChildItem -Path "publish" -Recurse | Measure-Object -Property Length -Sum).Sum
$publishSizeMB = [math]::Round($publishSize / 1MB, 2)
Write-Host "Final publish folder size: $publishSizeMB MB" -ForegroundColor Cyan

# Create minimal zip
Write-Host "Creating minimal deployment zip..." -ForegroundColor Yellow
Compress-Archive -Path "publish/*" -DestinationPath "ControlePressao-Minimal.zip" -CompressionLevel Optimal -Force

# Show final results
$zipSize = (Get-ChildItem "ControlePressao-Minimal.zip").Length
$zipSizeMB = [math]::Round($zipSize / 1MB, 2)
Write-Host "Minimal deployment package created: ControlePressao-Minimal.zip ($zipSizeMB MB)" -ForegroundColor Green

# List contents for verification
Write-Host "`nDeployment package contents:" -ForegroundColor Yellow
Get-ChildItem -Path "publish" -Recurse | Select-Object Name, @{Name="Size(KB)";Expression={[math]::Round($_.Length/1KB,1)}} | Sort-Object Name

Write-Host "`nDeployment optimization complete!" -ForegroundColor Green
Write-Host "Upload ControlePressao-Minimal.zip to Azure." -ForegroundColor Green
