# Consultez https://aka.ms/customizecontainer pour savoir comment personnaliser votre conteneur de débogage et comment Visual Studio utilise ce Dockerfile pour générer vos images afin d'accélérer le débogage.

# Cet index est utilisé lors de l'exécution à partir de VS en mode rapide (par défaut pour la configuration de débogage)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

# Fixed port configuration - always expose 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Cette phase est utilisée pour générer le projet de service
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["learnify.ai.api/learnify.ai.api.csproj", "learnify.ai.api/"]
RUN dotnet restore "./learnify.ai.api/learnify.ai.api.csproj"
COPY . .
WORKDIR "/src/learnify.ai.api"
RUN dotnet build "./learnify.ai.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Cette étape permet de publier le projet de service à copier dans la phase finale
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./learnify.ai.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Cette phase est utilisée en production ou lors de l'exécution à partir de VS en mode normal (par défaut quand la configuration de débogage n'est pas utilisée)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Ensure fixed port is used
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "learnify.ai.api.dll"]