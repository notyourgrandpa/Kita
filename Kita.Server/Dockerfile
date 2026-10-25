FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["Kita.Server/Kita.Server.csproj", "Kita.Server/"]
COPY ["Kita.Application/Kita.Application.csproj", "Kita.Application/"]
COPY ["Kita.Domain/Kita.Domain.csproj", "Kita.Domain/"]
COPY ["Kita.Infrastructure/Kita.Infrastructure.csproj", "Kita.Infrastructure/"]

RUN dotnet restore "Kita.Server/Kita.Server.csproj" -p:DockerBuild=true

COPY . .

WORKDIR "/src/Kita.Server"

RUN dotnet publish "Kita.Server.csproj" -c Release -o /app/publish -p:DockerBuild=true

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

HEALTHCHECK --interval=10s --timeout=5s --start-period=30s --retries=5 \
  CMD curl --fail --silent http://localhost:8080/api/ready || exit 1

ENTRYPOINT ["dotnet", "Kita.Server.dll"]