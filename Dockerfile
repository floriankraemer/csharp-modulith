# Build and publish the modular monolith Host (ASP.NET Core). Aspire AppHost is for local dev orchestration only.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY Directory.Build.props ./
COPY src/ ./src/

RUN dotnet restore ./src/CSharpModulith.Host/CSharpModulith.Host.csproj
RUN dotnet publish ./src/CSharpModulith.Host/CSharpModulith.Host.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

# Non-root runtime user (no adduser in some aspnet base images; numeric USER is enough).
RUN chown -R 1001:1001 /app

USER 1001:1001

ENTRYPOINT ["dotnet", "CSharpModulith.Host.dll"]
