@echo off
echo Iniciando deploy com Docker...

REM Parar container se estiver rodando
echo Parando container existente...
docker stop controlepressao 2>nul
docker rm controlepressao 2>nul

REM Remover imagem anterior
echo Removendo imagem anterior...
docker rmi controlepressao:latest 2>nul

REM Buildar nova imagem
echo Buildando nova imagem...
docker build -t controlepressao:latest .

REM Executar container
echo Executando container...
docker run -d --name controlepressao -p 8080:80 controlepressao:latest

echo Deploy concluído!
echo Aplicação disponível em: http://localhost:8080
pause
