FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8880
EXPOSE 8881

ENV LOG_FOLDER=/logs
ENV LOG_LEVEL=Information
ENV CONFIG_PATH=/app/config
ENV Serilog__MinimumLevel=${LOG_LEVEL

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
ENTRYPOINT ["dotnet", "Proxarr.Api.dll"]