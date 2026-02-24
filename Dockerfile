# =============================================================================
# Production Dockerfile - Multi-stage, port 8080, optimized for K8s
# =============================================================================
# - ASPNETCORE_URLS=http://+:8080 per requirement (bind all interfaces)
# - No localhost binding
# - Minimal runtime image
# =============================================================================

# ---------- STAGE 1: Restore ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
WORKDIR /src

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

COPY Monitoring.Domain/ ./Monitoring.Domain/
COPY Monitoring.Application/ ./Monitoring.Application/
COPY Monitoring.Infrastructure/ ./Monitoring.Infrastructure/
COPY Monitoring.Host.BlazorUI/ ./Monitoring.Host.BlazorUI/
COPY Monitoring.Host/ ./Monitoring.Host/

RUN dotnet publish Monitoring.Host/Monitoring.Host.csproj \
    -c Release \
    -o /app/publish \
    -p:UseAppHost=false \
    -p:EnableCompressionInSingleFile=false \
    --no-restore

# ---------- STAGE 3: Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Production: bind to 8080 on all interfaces (K8s standard)
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_HTTP_PORTS=8080

# Non-sensitive defaults - override via ConfigMap/Secret in K8s
ENV ConnectionStrings__Redis=redis:6379,abortConnect=false
ENV Jwt__Issuer=MonitoringSystem
ENV Jwt__Audience=MonitoringSystem

EXPOSE 8080

ENTRYPOINT ["dotnet", "Monitoring.Host.dll"]
