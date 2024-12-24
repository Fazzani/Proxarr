FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8880

EXPOSE $ASPNETCORE_HTTP_PORTS

ENV LOG_FOLDER=/logs
ENV LOG_LEVEL=Information
ENV CONFIG_PATH=/app/config
ENV Serilog__MinimumLevel=$LOG_LEVEL

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Proxarr.Api/Proxarr.Api.csproj", "Proxarr.Api/"]
RUN dotnet restore "./Proxarr.Api/Proxarr.Api.csproj"
COPY ./src .
WORKDIR "/src/Proxarr.Api"
RUN dotnet build "./Proxarr.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Proxarr.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final

WORKDIR /app
VOLUME ${LOG_FOLDER}/
VOLUME ${CONFIG_PATH}/
COPY --from=publish /app/publish .

USER root

RUN apt-get update && apt-get install -y --no-install-recommends \
  curl \
  && rm -rf /var/lib/apt/lists/*

USER $APP_UID

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 CMD curl -f http://localhost:$ASPNETCORE_HTTP_PORTS/health || exit 1

ENTRYPOINT ["dotnet", "Proxarr.Api.dll"]