#!/bin/bash

# Script de deploy otimizado para Azure
echo "🚀 Iniciando deploy otimizado..."

# Limpar arquivos desnecessários
echo "🧹 Limpando arquivos temporários..."
find . -name "bin" -type d -exec rm -rf {} + 2>/dev/null || true
find . -name "obj" -type d -exec rm -rf {} + 2>/dev/null || true
find . -name "*.user" -delete 2>/dev/null || true
find . -name "*.db" -delete 2>/dev/null || true
find . -name "*.log" -delete 2>/dev/null || true

# Criar build release otimizado
echo "🔨 Criando build otimizado..."
cd ControlePressao
dotnet clean
dotnet restore --no-cache
dotnet build -c Release --no-restore
dotnet publish -c Release -o ../publish \
    --no-restore \
    --no-build \
    --self-contained false \
    --verbosity minimal \
    /p:PublishSingleFile=false \
    /p:PublishTrimmed=false

echo "✅ Build otimizado criado em ../publish"
echo "📦 Tamanho do deploy:"
du -sh ../publish

echo "🎉 Deploy pronto!"
