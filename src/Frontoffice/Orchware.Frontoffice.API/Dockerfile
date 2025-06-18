# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Frontoffice/Orchware.Frontoffice.API/Orchware.Frontoffice.API.csproj", "src/Frontoffice/Orchware.Frontoffice.API/"]
COPY ["src/Common/DbQueryBuilder/DbQueryBuilder.csproj", "src/Common/DbQueryBuilder/"]
COPY ["src/Common/FileStorage/FileStorage.csproj", "src/Common/FileStorage/"]
COPY ["src/Common/Utilities/Utilities.csproj", "src/Common/Utilities/"]
RUN dotnet restore "./src/Frontoffice/Orchware.Frontoffice.API/Orchware.Frontoffice.API.csproj"
COPY . .
WORKDIR "/src/src/Frontoffice/Orchware.Frontoffice.API"
RUN dotnet build "./Orchware.Frontoffice.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Orchware.Frontoffice.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ./src/Frontoffice/Orchware.Frontoffice.API/Files ./Files
ENTRYPOINT ["dotnet", "Orchware.Frontoffice.API.dll"]