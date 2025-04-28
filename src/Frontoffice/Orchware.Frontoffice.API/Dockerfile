# Base image for running ASP.NET Core apps
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base  
WORKDIR /app  
EXPOSE 8080  
EXPOSE 8081  

# Build stage  
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build  
ARG BUILD_CONFIGURATION=Release  
WORKDIR /src  
COPY ["src/Frontoffice/Orchware.Frontoffice.API/Orchware.Frontoffice.API.csproj", "src/Frontoffice/Orchware.Frontoffice.API/"]  
RUN dotnet restore "./src/Frontoffice/Orchware.Frontoffice.API/Orchware.Frontoffice.API.csproj"  
COPY . .  
WORKDIR "/src/Frontoffice/Orchware.Frontoffice.API"  
RUN dotnet build "./Orchware.Frontoffice.API.csproj" -c $BUILD_CONFIGURATION -o /app/build  

# Publish stage  
FROM build AS publish  
ARG BUILD_CONFIGURATION=Release  
RUN dotnet publish "./Orchware.Frontoffice.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false  

# Final runtime stage  
FROM base AS final  
WORKDIR /app  
COPY --from=publish /app/publish .  
ENTRYPOINT ["dotnet", "Orchware.Frontoffice.API.dll"]  