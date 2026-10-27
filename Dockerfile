FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["WebApplication1ElectronicsStoreApiPractice/ElectronicsStoreApiPractice.csproj", "WebApplication1ElectronicsStoreApiPractice/"]
RUN dotnet restore "WebApplication1ElectronicsStoreApiPractice/ElectronicsStoreApiPractice.csproj"

COPY . .
WORKDIR "/src/WebApplication1ElectronicsStoreApiPractice"
RUN dotnet publish "ElectronicsStoreApiPractice.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ElectronicsStoreApiPractice.dll"]

# SDK-based image used only to apply Entity Framework Core migrations.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS migrations
WORKDIR /src

COPY ["WebApplication1ElectronicsStoreApiPractice/ElectronicsStoreApiPractice.csproj", "WebApplication1ElectronicsStoreApiPractice/"]
RUN dotnet restore "WebApplication1ElectronicsStoreApiPractice/ElectronicsStoreApiPractice.csproj"

COPY . .
RUN dotnet tool install --global dotnet-ef --version 10.0.13
ENV PATH="$PATH:/root/.dotnet/tools"

ENTRYPOINT ["dotnet", "ef", "database", "update", "--project", "/src/WebApplication1ElectronicsStoreApiPractice/ElectronicsStoreApiPractice.csproj"]
