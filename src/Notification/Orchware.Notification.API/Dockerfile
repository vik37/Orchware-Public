# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER $APP_UID
WORKDIR /app
EXPOSE 6050
EXPOSE 6070


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Notification/Orchware.Notification.API/Orchware.Notification.API.csproj", "src/Notification/Orchware.Notification.API/"]
COPY ["src/Common/IntegrationEvents/IntegrationEvents.csproj", "src/Common/IntegrationEvents/"]
RUN dotnet restore "./src/Notification/Orchware.Notification.API/Orchware.Notification.API.csproj"
COPY . .
WORKDIR "/src/src/Notification/Orchware.Notification.API"
RUN dotnet build "./Orchware.Notification.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Orchware.Notification.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Orchware.Notification.API.dll"]