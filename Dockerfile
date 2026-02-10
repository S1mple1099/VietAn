# ========== BUILD ==========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY MonitoringSystem.sln ./
COPY Monitoring.Domain/ ./Monitoring.Domain/
COPY Monitoring.Application/ ./Monitoring.Application/
COPY Monitoring.Infrastructure/ ./Monitoring.Infrastructure/
COPY Monitoring.Host.BlazorUI/ ./Monitoring.Host.BlazorUI/
COPY Monitoring.Host/ ./Monitoring.Host/

RUN dotnet restore
RUN dotnet publish Monitoring.Host/Monitoring.Host.csproj -c Release -o /app/publish -p:UseAppHost=false

# ========== RUNTIME ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Listen trên mọi interface (0.0.0.0), tránh ERR_EMPTY_RESPONSE
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV ASPNETCORE_URLS=http://0.0.0.0:80
ENV ASPNETCORE_ENVIRONMENT=Development

# Redis trên host (khi chạy Docker Desktop)
ENV ConnectionStrings__Redis=host.docker.internal:6379,abortConnect=false

EXPOSE 80
ENTRYPOINT ["dotnet", "Monitoring.Host.dll"]
