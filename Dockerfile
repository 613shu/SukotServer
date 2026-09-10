# Multi-stage build: the SDK image compiles/publishes the app, then the much
# smaller ASP.NET runtime image is what actually ships and runs on Render.
# Render has no native .NET runtime (confirmed against Render's own docs,
# Sept 2026) - Docker is the only supported way to deploy a .NET app there,
# regardless of the course's "Docker not required" framing, which is about
# graded course content, not about what a specific host needs to run .NET.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy just the .csproj files first and restore, so Docker can cache the
# (slow) restore layer and skip it on rebuilds where only .cs files changed.
COPY SukotSystemCore/SukotSystemCore.csproj SukotSystemCore/
COPY SukotSystemData/SukotSystemData.csproj SukotSystemData/
COPY SukotSystemService/SukotSystemService.csproj SukotSystemService/
COPY SukotSystemApi/SukotSystemApi.csproj SukotSystemApi/
RUN dotnet restore SukotSystemApi/SukotSystemApi.csproj

# Now copy everything else and publish. Release config, no separate build
# step needed - publish does its own build.
COPY . .
RUN dotnet publish SukotSystemApi/SukotSystemApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render sets $PORT and expects the container to listen on it - Program.cs
# already reads $PORT itself, so nothing Docker-specific is needed here for
# that; EXPOSE is documentation only, doesn't affect what Render actually uses.
EXPOSE 8080

ENTRYPOINT ["dotnet", "SukotSystemApi.dll"]
