@echo off
echo Iniciando deploy para IIS...

REM Navegar para o diretório do projeto
cd /d "%~dp0ControlePressao"

REM Limpar e buildar o projeto
echo Limpando projeto...
dotnet clean

echo Buildando projeto...
dotnet build --configuration Release

echo Publicando projeto...
dotnet publish --configuration Release --output ../publish

echo Deploy concluído!
echo Arquivos publicados em: %~dp0publish
pause
