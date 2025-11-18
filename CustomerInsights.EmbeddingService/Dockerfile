FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CustomerInsights.EmbeddingService/CustomerInsights.EmbeddingService.csproj", "CustomerInsights.EmbeddingService/"]
RUN dotnet restore "CustomerInsights.EmbeddingService/CustomerInsights.EmbeddingService.csproj"
COPY . .
WORKDIR "/src/CustomerInsights.EmbeddingService"
RUN dotnet build "./CustomerInsights.EmbeddingService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CustomerInsights.EmbeddingService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CustomerInsights.EmbeddingService.dll"]
