# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Use the official .NET 8 runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install SQLite and ICU libraries (required for SQLite database and globalization)
RUN apt-get update && apt-get install -y \
    sqlite3 \
    libicu-dev \
    libicu72 \
    icu-devtools \
    && rm -rf /var/lib/apt/lists/*

# Copy the published app
COPY --from=build /app/out .

# Create directory for SQLite database
RUN mkdir -p /app/data

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:$PORT
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true
ENV LC_ALL=C.UTF-8
ENV LANG=C.UTF-8

# Expose port (Railway will set the PORT environment variable)
EXPOSE $PORT

# Run the application
ENTRYPOINT ["dotnet", "ControlePressao.dll"]