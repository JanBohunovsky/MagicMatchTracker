FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project file into the container's `/src` directory (WORKDIR).
COPY ["src/MagicMatchTracker.csproj", "."]
RUN dotnet restore "./MagicMatchTracker.csproj"

# Copy the rest of the source code from local `src` directory into the WORKDIR.
COPY ["src/.", "."]
RUN dotnet build "./MagicMatchTracker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MagicMatchTracker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MagicMatchTracker.dll"]