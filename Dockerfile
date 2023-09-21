FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["GatewayPing.csproj", "./"]
RUN dotnet restore "GatewayPing.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "GatewayPing.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GatewayPing.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GatewayPing.dll"]
