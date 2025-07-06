@echo off
echo Executando aplicacao ControlePressao...
dotnet build
if %errorlevel% neq 0 (
    echo Erro ao compilar!
    pause
    exit /b %errorlevel%
)
echo.
echo Iniciando aplicacao...
dotnet run --urls="http://localhost:5000"
pause 