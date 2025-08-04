# Script para build e teste local do Docker
# Execute este script antes de fazer deploy na Railway

Write-Host "🐳 Iniciando build do Docker..." -ForegroundColor Green

# Build da imagem Docker
docker build -t controle-pressao:latest .

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Build concluído com sucesso!" -ForegroundColor Green
    Write-Host "🚀 Para testar localmente, execute:" -ForegroundColor Yellow
    Write-Host "docker run -p 8080:8080 --name controle-pressao-test controle-pressao:latest" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "📱 Acesse: http://localhost:8080" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "🛑 Para parar: docker stop controle-pressao-test" -ForegroundColor Red
    Write-Host "🗑️ Para remover: docker rm controle-pressao-test" -ForegroundColor Red
} else {
    Write-Host "❌ Erro no build do Docker!" -ForegroundColor Red
    exit 1
}