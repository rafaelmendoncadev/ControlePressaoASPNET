# Use a imagem oficial do .NET 8.0 runtime (mais leve)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080

# Use a imagem do SDK para build
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copy apenas o arquivo de projeto primeiro para cache das dependências
COPY ["ControlePressao/ControlePressao.csproj", "ControlePressao/"]
RUN dotnet restore "ControlePressao/ControlePressao.csproj"

# Copy o resto dos arquivos
COPY . .
WORKDIR "/src/ControlePressao"

# Build otimizado
RUN dotnet build "ControlePressao.csproj" -c Release -o /app/build --no-restore

# Publish otimizado
FROM build AS publish
RUN dotnet publish "ControlePressao.csproj" -c Release -o /app/publish --no-restore \
    --self-contained false \
    --verbosity minimal

# Final stage
FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

# Criar diretório para logs
RUN mkdir -p /app/logs && chmod 755 /app/logs

# Configurar usuário não-root para segurança
RUN addgroup -g 1001 -S appgroup && \
    adduser -S appuser -u 1001 -G appgroup
RUN chown -R appuser:appgroup /app
USER appuser

ENTRYPOINT ["dotnet", "ControlePressao.dll"]
