# ====== STAGE 1: BUILD ======
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy toàn bộ solution
COPY . .

# Restore dependencies
RUN dotnet restore "EV_BatteryChangeStation/EV_BatteryChangeStation.csproj"

# Build và publish ứng dụng
RUN dotnet publish "EV_BatteryChangeStation/EV_BatteryChangeStation.csproj" -c Release -o /app/publish

# ====== STAGE 2: RUNTIME ======
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EV_BatteryChangeStation.dll"]
