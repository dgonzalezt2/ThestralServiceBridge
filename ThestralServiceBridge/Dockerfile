FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ThestralServiceBridge/ThestralServiceBridge.csproj", "ThestralServiceBridge/"]
COPY ["ThestralServiceBridge.Infrastructure/ThestralServiceBridge.Infrastructure.csproj", "ThestralServiceBridge.Infrastructure/"]
COPY ["ThestralServiceBridge.Domain/ThestralServiceBridge.Domain.csproj", "ThestralServiceBridge.Domain/"]
RUN dotnet restore "ThestralServiceBridge/ThestralServiceBridge.csproj"
COPY . .
WORKDIR "/src/ThestralServiceBridge"
RUN dotnet build "ThestralServiceBridge.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ThestralServiceBridge.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ThestralServiceBridge.dll"]
