# Base image for running ASP.NET Core apps
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Backoffice/Orchware.Backoffice.API/Orchware.Backoffice.API.csproj", "src/Backoffice/Orchware.Backoffice.API/"]
COPY ["src/Backoffice/Orchware.Backoffice.Application/Orchware.Backoffice.Application.csproj", "src/Backoffice/Orchware.Backoffice.Application/"]
COPY ["src/Backoffice/Orchware.Backoffice.Domain/Orchware.Backoffice.Domain.csproj", "src/Backoffice/Orchware.Backoffice.Domain/"]
COPY ["src/Backoffice/Orchware.Backoffice.Infrastructure/Orchware.Backoffice.Infrastructure.csproj", "src/Backoffice/Orchware.Backoffice.Infrastructure/"]
COPY ["src/Common/DbQueryBuilder/DbQueryBuilder.csproj", "src/Common/DbQueryBuilder/"]
COPY ["src/Common/FileStorage/FileStorage.csproj", "src/Common/FileStorage/"]
RUN dotnet restore "./src/Backoffice/Orchware.Backoffice.API/Orchware.Backoffice.API.csproj"
COPY . .
WORKDIR "/src/src/Backoffice/Orchware.Backoffice.API"
RUN dotnet build "./Orchware.Backoffice.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Orchware.Backoffice.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime stage  
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Orchware.Backoffice.API.dll"]