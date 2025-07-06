# Use a imagem oficial do .NET 9.0 runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use a imagem do SDK para build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ControlePressao/ControlePressao.csproj", "ControlePressao/"]
RUN dotnet restore "ControlePressao/ControlePressao.csproj"
COPY . .
WORKDIR "/src/ControlePressao"
RUN dotnet build "ControlePressao.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ControlePressao.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ControlePressao.dll"]
