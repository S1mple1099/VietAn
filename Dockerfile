# =============================================================================
# Multi-stage Dockerfile - Production .NET 8
# Tối ưu layer cache: copy .csproj trước, restore, rồi mới copy source
# =============================================================================

# ---------- STAGE 1: Restore ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
WORKDIR /src

# Copy solution và .csproj trước để tận dụng cache (restore chỉ chạy lại khi thay đổi)
COPY MonitoringSystem.sln ./
COPY Monitoring.Domain/*.csproj ./Monitoring.Domain/
COPY Monitoring.Application/*.csproj ./Monitoring.Application/
COPY Monitoring.Infrastructure/*.csproj ./Monitoring.Infrastructure/
COPY Monitoring.Host.BlazorUI/*.csproj ./Monitoring.Host.BlazorUI/
COPY Monitoring.Host/*.csproj ./Monitoring.Host/

RUN dotnet restore

# ---------- STAGE 2: Build ----------
FROM restore AS build
WORKDIR /src

# Copy toàn bộ source (restore đã xong ở layer trước)
COPY Monitoring.Domain/ ./Monitoring.Domain/
COPY Monitoring.Application/ ./Monitoring.Application/
COPY Monitoring.Infrastructure/ ./Monitoring.Infrastructure/
COPY Monitoring.Host.BlazorUI/ ./Monitoring.Host.BlazorUI/
COPY Monitoring.Host/ ./Monitoring.Host/

RUN dotnet publish Monitoring.Host/Monitoring.Host.csproj \
    -c Release \
    -o /app/publish \
    -p:UseAppHost=false \
    --no-restore

# ---------- STAGE 3: Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Production settings
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV ASPNETCORE_URLS=http://0.0.0.0:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Connection strings - override bằng env khi chạy (docker-compose)
ENV ConnectionStrings__DefaultConnection=Data Source=/data/app.db
ENV ConnectionStrings__Redis=redis:6379,abortConnect=false

EXPOSE 80

ENTRYPOINT ["dotnet", "Monitoring.Host.dll"]
