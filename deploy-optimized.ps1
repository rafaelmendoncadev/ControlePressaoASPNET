# Script de deploy otimizado para Windows
Write-Host "🚀 Iniciando deploy otimizado..." -ForegroundColor Green

# Limpar arquivos desnecessários
Write-Host "🧹 Limpando arquivos temporários..." -ForegroundColor Yellow
Get-ChildItem -Path . -Recurse -Directory -Name "bin" | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
Get-ChildItem -Path . -Recurse -Directory -Name "obj" | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
Get-ChildItem -Path . -Recurse -File -Name "*.user" | Remove-Item -Force -ErrorAction SilentlyContinue
Get-ChildItem -Path . -Recurse -File -Name "*.db" | Remove-Item -Force -ErrorAction SilentlyContinue
Get-ChildItem -Path . -Recurse -File -Name "*.log" | Remove-Item -Force -ErrorAction SilentlyContinue

# Remover pasta publish anterior se existir
if (Test-Path "publish") {
    Remove-Item -Recurse -Force "publish"
}

# Criar build release otimizado
Write-Host "🔨 Criando build otimizado..." -ForegroundColor Yellow
Set-Location "ControlePressao"

dotnet clean
dotnet restore --no-cache
dotnet build -c Release --no-restore
dotnet publish -c Release -o "..\publish" `
    --no-restore `
    --no-build `
    --self-contained false `
    --verbosity minimal `
    /p:PublishSingleFile=false `
    /p:PublishTrimmed=false

Set-Location ".."

Write-Host "✅ Build otimizado criado em .\publish" -ForegroundColor Green

# Mostrar tamanho do deploy
$publishSize = (Get-ChildItem -Path "publish" -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
Write-Host "📦 Tamanho do deploy: $([math]::Round($publishSize, 2)) MB" -ForegroundColor Cyan

Write-Host "🎉 Deploy pronto!" -ForegroundColor Green
